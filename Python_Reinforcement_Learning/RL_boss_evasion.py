import os
import time
import numpy as np
import pandas as pd
from stable_baselines3 import PPO
import gym

STATE_FILE = 'state.txt'
ACTION_FILE = 'action.txt'
PPO_MODEL_PATH = 'ppo_boss_model.zip'

STATE_COLS = [
    'BossX', 'BossY', 'BossVX', 'BossVY',
    'PlayerX', 'PlayerY',
    'projX', 'projY', 'projVX', 'projVY',
    'projType', 'dirX', 'dirY', 'threat', 'BossHP'
]

# 보스 환경 예시: state와 reward만 받는 더미 환경 (실전 환경은 직접 구현 필요)
class BossAvoidEnv(gym.Env):
    def __init__(self):
        super().__init__()
        self.observation_space = gym.spaces.Box(low=-10, high=10, shape=(15,), dtype=np.float32)
        self.action_space = gym.spaces.Box(low=-2, high=2, shape=(2,), dtype=np.float32)
        self.state = np.zeros(15, dtype=np.float32)
        self.timestep = 0
        self.max_steps = 50

    def reset(self):
        self.state = np.random.uniform(-1, 1, size=15).astype(np.float32)
        self.timestep = 0
        return self.state

    def step(self, action):
        self.state[:2] += action
        self.state = np.clip(self.state, -10, 10)
        self.timestep += 1
        threat = self.state[13]
        boss_hp = self.state[14]
        avoid_reward = 1.0 if threat < 0.5 else -1.0
        hp_shaping = 0.5 if (avoid_reward > 0 and boss_hp < 30.0) else 0.0
        reward = avoid_reward + hp_shaping
        done = self.timestep >= self.max_steps
        info = {}
        return self.state, reward, done, info

def parse_state_line(line):
    values = list(map(float, line.strip().split(',')))
    return np.array(values, dtype=np.float32)

def write_action(action, action_file=ACTION_FILE):
    with open(action_file, 'w') as f:
        f.write(f"{action[0]:.3f},{action[1]:.3f}")

# PPO 모델 전역 로드 or 생성
if os.path.exists(PPO_MODEL_PATH):
    print("[INFO] Loading PPO model ...")
    agent = PPO.load(PPO_MODEL_PATH)
    print("[INFO] PPO model loaded.")
else:
    print("[INFO] PPO model not found, creating new model ...")
    agent = PPO("MlpPolicy", BossAvoidEnv(), verbose=1)

def main_trajectory(df):
    # 전체 trajectory 학습 + 마지막 state로 예측
    print(f"[INFO] main_trajectory() called with {len(df)} steps")

    # 1. trajectory를 환경에서 학습 (여기서는 매우 간단한 예시)
    # 실제로는 custom 환경을 Gym 스타일로 만들어야 PPO 학습이 동작함
    # 아래 코드는 imitation learning이나 custom replay buffer를 쓰지 않는 가장 단순한 PPO online 학습 예시입니다.

    # trajectory 기반 학습(한 episode)
    obs = df[STATE_COLS].to_numpy(dtype=np.float32)
    rewards = df['reward'].to_numpy(dtype=np.float32)
    dones = df['done'].to_numpy(dtype=bool)

    # 간단하게 Gym 환경에서 step을 반복
    env = BossAvoidEnv()
    state = env.reset()
    for i in range(len(obs)):
        # PPO의 policy로 행동을 선택
        action, _ = agent.predict(state, deterministic=True)
        next_state, reward, done, _ = env.step(action)
        # reward는 실제 trajectory의 reward로 overwrite
        reward = rewards[i]
        # PPO는 내부적으로 rollout buffer로 자동 학습
        if done or (i == len(obs)-1):
            state = env.reset()
        else:
            state = next_state

    agent.learn(total_timesteps=100)  # rollout buffer에 쌓인 경험으로 업데이트 (100은 최소값, 실제론 더 많게 설정 가능)
    agent.save(PPO_MODEL_PATH)

    # 마지막 state로 행동 예측
    last_state = obs[-1]
    action, _ = agent.predict(last_state, deterministic=True)
    write_action(action)
    print(f"[INFO] Action saved: {action[0]:.3f}, {action[1]:.3f}")

def watch_file_loop(state_file=STATE_FILE, check_interval=1):
    last_mtime = None
    print(f"[INFO] Start watching {state_file}")

    while True:
        try:
            current_mtime = os.path.getmtime(state_file)
        except FileNotFoundError:
            print(f"[WARN] File not found: {state_file}")
            time.sleep(check_interval)
            continue

        if last_mtime is None:
            last_mtime = current_mtime

        if current_mtime != last_mtime:
            print(f"[INFO] File modified: {state_file}")
            try:
                # 전체 trajectory를 DataFrame으로 읽기
                df = pd.read_csv(state_file)
                if len(df) < 1:
                    print("[WARN] No data in state.txt")
                    last_mtime = current_mtime
                    time.sleep(check_interval)
                    continue
                main_trajectory(df)  # RL 학습 및 예측
            except Exception as e:
                print(f"[ERROR] Failed to parse trajectory or run model: {e}")
            last_mtime = current_mtime

        time.sleep(check_interval)

if __name__ == "__main__":
    watch_file_loop()
