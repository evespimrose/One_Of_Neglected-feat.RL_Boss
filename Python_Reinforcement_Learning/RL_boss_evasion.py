import os
import time
import numpy as np
import pandas as pd
from stable_baselines3 import PPO
import gym

BASE_DIR = r'C:/Users/ice31/AppData/LocalLow/DefaultCompany/One_Of_Neglected/'
STATE_FILE = os.path.join(BASE_DIR, 'state.txt')
ACTION_FILE = os.path.join(BASE_DIR, 'action.txt')
PPO_MODEL_PATH = 'ppo_boss_model.zip'

STATE_COLS = [
    'BossX', 'BossY', 'BossVX', 'BossVY',
    'PlayerX', 'PlayerY',
    'projX', 'projY', 'projVX', 'projVY',
    'projType', 'dirX', 'dirY', 'threat', 'BossHP'
]

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
        print("[DEBUG] Env.reset() called, state:", self.state)
        return self.state

    def step(self, action):
        print(f"[DEBUG] Env.step() called - state: {self.state}, action: {action}")
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
        print(f"[DEBUG] Env.step() result - next_state: {self.state}, reward: {reward}, done: {done}")
        return self.state, reward, done, info

def parse_state_line(line):
    values = list(map(float, line.strip().split(',')))
    return np.array(values, dtype=np.float32)

def write_action(action, action_file=ACTION_FILE):
    with open(action_file, 'w') as f:
        f.write(f"{action[0]:.3f},{action[1]:.3f}")
    print(f"[INFO] Action written to {action_file}: {action[0]:.3f}, {action[1]:.3f}")

if os.path.exists(PPO_MODEL_PATH):
    print("[INFO] Loading PPO model ...")
    agent = PPO.load(PPO_MODEL_PATH)
    print("[INFO] PPO model loaded.")
else:
    print("[INFO] PPO model not found, creating new model ...")
    agent = PPO("MlpPolicy", BossAvoidEnv(), verbose=1)
    print("[INFO] PPO model created.")

def main_trajectory(df):
    print(f"[INFO] main_trajectory() called with {len(df)} steps")
    obs = df[STATE_COLS].to_numpy(dtype=np.float32)
    rewards = df['reward'].to_numpy(dtype=np.float32)
    dones = df['done'].to_numpy(dtype=bool)
    env = BossAvoidEnv()
    state = env.reset()
    for i in range(len(obs)):
        print(f"[DEBUG] Step {i+1}/{len(obs)} - State: {state}")
        action, _ = agent.predict(state, deterministic=True)
        print(f"[DEBUG] Agent predicted action: {action}")
        next_state, reward, done, _ = env.step(action)
        reward = rewards[i]
        print(f"[DEBUG] Reward used from data: {reward}")
        if done or (i == len(obs)-1):
            print(f"[INFO] Trajectory finished at step {i+1} (done={done})")
            state = env.reset()
        else:
            state = next_state

    print("[INFO] Learning with PPO ...")
    agent.learn(total_timesteps=100)
    agent.save(PPO_MODEL_PATH)
    print("[INFO] PPO model saved to", PPO_MODEL_PATH)
    last_state = obs[-1]
    action, _ = agent.predict(last_state, deterministic=True)
    write_action(action)
    print(f"[INFO] Action saved for next step: {action[0]:.3f}, {action[1]:.3f}")

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
                df = pd.read_csv(state_file)
                if len(df) < 1:
                    print("[WARN] No data in state.txt")
                    last_mtime = current_mtime
                    time.sleep(check_interval)
                    continue
                main_trajectory(df)
            except Exception as e:
                print(f"[ERROR] Failed to parse trajectory or run model: {e}")
            last_mtime = current_mtime

        time.sleep(check_interval)

if __name__ == "__main__":
    watch_file_loop()
