import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from sklearn.preprocessing import MinMaxScaler
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense
from tensorflow.keras.callbacks import EarlyStopping
from matplotlib.patches import Circle
import os
import sys
import tensorflow as tf
import time

def main():
    # ─────────────────────────────────────
    # GPU 설정 및 사용 여부 확인
    gpus = tf.config.list_physical_devices('GPU')
    gpu_used = False
    if gpus:
        try:
            # 첫 번째 GPU만 사용
            tf.config.set_visible_devices(gpus[0], 'GPU')
            tf.config.experimental.set_memory_growth(gpus[0], True)
            gpu_used = True
            print(f"[INFO] GPU 사용: {gpus[0].name}")
        except RuntimeError as e:
            print(f"[ERROR] GPU 설정 실패: {e}")
    else:
        print("[INFO] GPU를 찾을 수 없습니다. CPU로 실행됩니다.")

    # 추가로, GPU 장치 이름 출력
    physical_devices = tf.config.list_physical_devices('GPU')
    for device in physical_devices:
        print(f"사용 가능한 GPU: {device.name}")

    # ─────────────────────────────────────
    # 하이퍼파라미터 및 경로 처리
    #TRAIN_PATH  = r'C:/Users/ice31/AppData/LocalLow/DefaultCompany/One_Of_Neglected/PlayerPositionLog.txt'
    #TEST_PATH   = r'C:/Users/ice31/AppData/LocalLow/DefaultCompany/One_Of_Neglected/PlayerPositionLog.txt'
    #OUTPUT_TXT  = r'C:/Users/ice31/AppData/LocalLow/DefaultCompany/One_Of_Neglected/auto_trap_placement.txt'
    TRAIN_PATH  = r'C:/Users/Jang/AppData/LocalLow/DefaultCompany/One_Of_Neglected/PlayerPositionLog.txt'
    TEST_PATH   = r'C:/Users/Jang/AppData/LocalLow/DefaultCompany/One_Of_Neglected/PlayerPositionLog.txt'
    OUTPUT_TXT  = r'C:/Users/Jang/AppData/LocalLow/DefaultCompany/One_Of_Neglected/auto_trap_placement.txt'

    # if len(sys.argv) < 3:
    #     print("Usage: python DL2.py <input_file> <output_file>")
    #     sys.exit(1)
    #
    # TRAIN_PATH = sys.argv[1]
    # TEST_PATH = sys.argv[1]  # 같은 파일 사용
    # OUTPUT_TXT = sys.argv[2]

    FPS = 20  # 1초에 20프레임
    STILL_SEC = 5
    STILL_FRAMES = STILL_SEC * FPS
    STILL_TOL = 1e-4
    PRED_OFFSET = 100  # 4초
    cols = [
        'userx', 'usery',
        'monster1x', 'monster1y',
        'monster2x', 'monster2y',
        'monster3x', 'monster3y',
        'monster4x', 'monster4y',
        'monster5x', 'monster5y'
    ]

    # ─────────────────────────────────────
    # 유틸 함수
    def add_user_motion(df):
        df = df.copy()
        df['vx'] = df['userx'].diff().fillna(0)
        df['vy'] = df['usery'].diff().fillna(0)
        df['ax'] = df['vx'].diff().fillna(0)
        df['ay'] = df['vy'].diff().fillna(0)
        df['speed'] = np.sqrt(df['vx']**2 + df['vy']**2)
        df['acc'] = np.sqrt(df['ax']**2 + df['ay']**2)
        return df

    def user_stationary(df):
        if len(df) < STILL_FRAMES:
            return False
        tail = df.tail(STILL_FRAMES)
        return (tail['userx'].max() - tail['userx'].min() < STILL_TOL) and \
               (tail['usery'].max() - tail['usery'].min() < STILL_TOL)

    def save_trap_xy(xy, path=OUTPUT_TXT):
        with open(path, 'w') as fp:
            fp.write(f'{xy[0]:.2f},{xy[1]:.2f}')
        print(f'[INFO] trap 좌표 저장 → {path}')

    def prepare_dataset(data, input_len, pred_offset=PRED_OFFSET, fps=FPS):
        X, y = [], []
        weight_frames = 3 * fps  # 마지막 3초 (60프레임)
        for i in range(len(data) - input_len - pred_offset):
            seq = data[i:i+input_len].copy()
            # 마지막 3초 구간 강조 (1.8배)
            if weight_frames < input_len:
                seq[-weight_frames:] *= 1.8
            X.append(seq)
            y.append(data[i+input_len+pred_offset][:2])  # userx, usery
        return np.array(X, dtype=np.float32), np.array(y, dtype=np.float32)

    def inverse_transform_positions(scaled_pos, scaler, feat_dim):
        dummy = np.zeros((scaled_pos.shape[0], feat_dim))
        dummy[:, :2] = scaled_pos
        inv = scaler.inverse_transform(dummy)
        return inv[:, :2]

    def dynamic_input_len(df, recent_window=None, short_len=60, long_len=100, speed_threshold=0.015, movement_std_threshold=0.1):
        if recent_window is None:
            recent_window = min(len(df) // 3, 150)
        vx = df['userx'].diff().fillna(0)
        vy = df['usery'].diff().fillna(0)
        speed = np.sqrt(vx**2 + vy**2)
        directions = np.arctan2(vy, vx)

        recent_speed_mean = speed.tail(recent_window).mean()
        recent_direction_std = np.std(directions.tail(recent_window))

        print(f"[INFO] 최근 평균 속도: {recent_speed_mean:.6f}")
        print(f"[INFO] 최근 방향 변화량 std: {recent_direction_std:.6f}")

        if recent_speed_mean > speed_threshold and recent_direction_std > movement_std_threshold:
            print("[INFO] 빠르고 방향 변화 심함 → input_len 짧게 설정")
            return short_len
        else:
            print("[INFO] 느리고 일정한 패턴 → input_len 길게 설정")
            return long_len

    def plot_prediction(df, pred_point, max_distance=None, title='User Path & Prediction'):
        plt.figure(figsize=(8,6))
        plt.plot(df['userx'], df['usery'], 'b-', label='User Path')
        plt.scatter(df['userx'].iloc[0], df['usery'].iloc[0], c='green', s=90, label='Start')
        plt.scatter(pred_point[0], pred_point[1], c='red', s=110, label='Predicted')

        if max_distance is not None:
            last_xy = df[['userx', 'usery']].iloc[-1].values
            circle = Circle((last_xy[0], last_xy[1]), max_distance, color='orange', alpha=0.2, label='Max Prediction Range')
            plt.gca().add_patch(circle)

        plt.axis('equal'); plt.grid(); plt.xlabel('X'); plt.ylabel('Y')
        plt.title(title)
        plt.legend()
        plt.show()

    # ─────────────────────────────────────
    # 데이터 로드 및 전처리
    df_train = pd.read_csv(TRAIN_PATH, header=0, names=cols).dropna().reset_index(drop=True)
    df_test  = pd.read_csv(TEST_PATH , header=0, names=cols).dropna().reset_index(drop=True)

    if user_stationary(df_train):
        last_xy = (df_train['userx'].iloc[-1], df_train['usery'].iloc[-1])
        print('사용자 5초 이상 정지 상태 → 모델 학습·예측 건너뜀')
        save_trap_xy(last_xy)
        sys.exit()

    df_train = add_user_motion(df_train)
    df_test  = add_user_motion(df_test)

    INPUT_LEN = dynamic_input_len(df_train)
    print(f"[INFO] 결정된 input_len: {INPUT_LEN}")

    scaler = MinMaxScaler()
    scaled_train = scaler.fit_transform(df_train)
    scaled_test  = scaler.transform(df_test)

    X_train, y_train = prepare_dataset(scaled_train, input_len=INPUT_LEN)
    X_test , y_test  = prepare_dataset(scaled_test,  input_len=INPUT_LEN)

    # ─────────────────────────────────────
    # 모델 학습
    model = Sequential([
        LSTM(16, input_shape=(INPUT_LEN, X_train.shape[2]), dropout=0.2),
        Dense(16, activation='relu'),
        Dense(2)
    ])
    model.compile(optimizer='adam', loss='mse', metrics=['mae'])
    early = EarlyStopping(monitor='val_loss', patience=5, restore_best_weights=True)
    model.fit(X_train, y_train, epochs=1, batch_size=64, validation_split=0.2, callbacks=[early], verbose=1)

    # ─────────────────────────────────────
    # 실시간 예측 + 최대 이동 반경 제한 (평균 속도 기반)
    last_seq = scaled_test[-INPUT_LEN:].reshape(1, INPUT_LEN, -1)
    future_scaled = model.predict(last_seq)[0]
    future_xy = inverse_transform_positions(future_scaled[None, :], scaler, scaled_train.shape[1])[0]

    last_xy = df_test[['userx', 'usery']].iloc[-1].values

    # 평균 속도 기반 최대 이동 반경
    recent_speed = df_train['speed'].tail(STILL_FRAMES)
    avg_speed = recent_speed.mean()
    max_distance = avg_speed * PRED_OFFSET
    print(f"[INFO] 평균 속도 기반 최대 이동 거리: {max_distance:.4f}")

    displacement = future_xy - last_xy
    dist = np.linalg.norm(displacement)
    if dist > max_distance:
        displacement = displacement / dist * max_distance
        future_xy = last_xy + displacement
        print("[INFO] 예측 위치가 반경 초과 → 내부로 보정됨")

    save_trap_xy(future_xy)

    # ─────────────────────────────────────
    # GPU 사용 여부에 따른 타이틀에 상태 표시
    title_status = "[GPU_USE]" if gpu_used else "[CPU_USE]"
    # plot_prediction(df_test, future_xy, max_distance=max_distance, title=f'User Path & Prediction {title_status}')

#file_path = r'C:/Users/ice31/AppData/LocalLow/DefaultCompany/One_Of_Neglected/PlayerPositionLog.txt'  # 체크할 파일 경로
file_path = r'C:/Users/Jang/AppData/LocalLow/DefaultCompany/One_Of_Neglected/PlayerPositionLog.txt'  # 체크할 파일 경로

# 파일의 마지막 수정 시간 가져오기
last_modified_time = os.path.getmtime(file_path)

print("처음 수정 시간:", last_modified_time)

while True:
    time.sleep(1)  # 1초 대기
    current_modified_time = os.path.getmtime(file_path)
    if current_modified_time != last_modified_time:
        print("파일이 수정됨")
        last_modified_time = current_modified_time
        main()
    else:
        print("파일에 변화 없음")