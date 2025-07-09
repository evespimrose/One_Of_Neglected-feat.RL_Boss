import numpy as np
import matplotlib.pyplot as plt
import tensorflow as tf
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense
from sklearn.model_selection import train_test_split

# 데이터 불러오기
def load_movement_data(filepath):
    data = []
    with open(filepath, 'r') as f:
        for line in f:
            x, y = map(float, line.strip().split(','))
            data.append((x, y))
    return np.array(data)

# 데이터셋 구성
def prepare_dataset(data, input_len=10, pred_offset=30):
    X, y = [], []
    for i in range(len(data) - input_len - pred_offset):
        input_seq = data[i:i+input_len]  # (input_len, 2)
        target_pos = data[i + input_len + pred_offset]  # (2,)
        X.append(input_seq)
        y.append(target_pos)
    return np.array(X), np.array(y)

# 시각화
def plot_with_prediction(data, pred_point, input_seq=None):
    plt.figure(figsize=(8, 6))
    plt.plot(data[:, 0], data[:, 1], label='User Path', color='blue', linewidth=1)
    plt.scatter(pred_point[0], pred_point[1], color='red', label='Predicted Position (3s later)', s=100)
    if input_seq is not None:
        plt.plot(input_seq[:, 0], input_seq[:, 1], color='green', label='Input Sequence', linewidth=2)
    plt.xlabel('X')
    plt.ylabel('Y')
    plt.title('Player Movement & 3s Prediction (LSTM)')
    plt.legend()
    plt.grid(True)
    plt.axis('equal')
    plt.show()

# 경로
filepath = '/Users/ice31/Desktop/user_movement_log.txt'
data = load_movement_data(filepath)

# 입력 시퀀스 10개, 예측 30스텝 뒤 (3초)
X, y = prepare_dataset(data, input_len=10, pred_offset=30)

# 데이터 나누기
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# 모델 정의
model = Sequential([
    LSTM(64, input_shape=(X.shape[1], X.shape[2])),
    Dense(32, activation='relu'),
    Dense(2)  # X, Y 출력
])

model.compile(optimizer='adam', loss='mse', metrics=['mae'])
model.summary()

# 모델 학습
model.fit(X_train, y_train, epochs=100, batch_size=8, validation_split=0.2, verbose=1)

# 예측
input_seq = data[-(10 + 30):-30]  # 마지막 input 10개
input_seq = input_seq.reshape(1, 10, 2)
predicted_pos = model.predict(input_seq)[0]

print(f"예측 위치 (3초 뒤): {predicted_pos}")

# 시각화
plot_with_prediction(data, pred_point=predicted_pos, input_seq=input_seq[0])
