import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import MinMaxScaler
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense

# 컬럼명 정의
columns = [
    'userx', 'usery',
    'monster1x', 'monster1y',
    'monster2x', 'monster2y',
    'monster3x', 'monster3y',
    'monster4x', 'monster4y',
    'monster5x', 'monster5y'
]

# 학습 데이터 불러오기
train_path = 'C:/Users/ice31/AppData/LocalLow/DefaultCompany/One_Of_Neglected/PlayerPositionLog.txt'  # 학습용 경로
df_train = pd.read_csv(train_path, header=0, names=columns)

# 테스트 데이터 불러오기
test_path = 'C:/Users/ice31/AppData/LocalLow/DefaultCompany/One_Of_Neglected/test.txt'  # 테스트용 경로
df_test = pd.read_csv(test_path, header=0, names=columns)

# NULL값 처리
df_train = df_train.dropna()
df_test = df_test.dropna()

# 정규화 (MinMaxScaler)
scaler = MinMaxScaler()
scaled_train = scaler.fit_transform(df_train)
scaled_test = scaler.transform(df_test)

# 데이터셋 생성 함수
def prepare_dataset(data, input_len=20, pred_offset=60):
    X, y = [], []
    for i in range(len(data) - input_len - pred_offset):
        X.append(data[i:i + input_len])  # (input_len, 12)
        y.append(data[i + input_len + pred_offset][:2])  # user x,y 좌표 (2,)
    return np.array(X), np.array(y)

# 학습용 데이터셋 생성
X_train, y_train = prepare_dataset(scaled_train, input_len=20, pred_offset=60)

# 테스트용 데이터셋 생성
X_test, y_test = prepare_dataset(scaled_test, input_len=20, pred_offset=60)

# 모델 구성
model = Sequential([
    LSTM(64, input_shape=(X_train.shape[1], X_train.shape[2])),
    Dense(32, activation='relu'),
    Dense(2)  # user 위치 (x,y)
])

model.compile(optimizer='adam', loss='mse', metrics=['mae'])
model.summary()

# 모델 학습
model.fit(X_train, y_train, epochs=100, batch_size=8, validation_split=0.2, verbose=1)

# 테스트 데이터로 예측 및 평가
y_pred_scaled = model.predict(X_test)

# 예측과 실제값을 원래 스케일로 복원하는 함수
def inverse_transform_positions(scaled_positions, scaler):
    dummy = np.zeros((scaled_positions.shape[0], 12))
    dummy[:, :2] = scaled_positions
    inv = scaler.inverse_transform(dummy)
    return inv[:, :2]

y_pred = inverse_transform_positions(y_pred_scaled, scaler)
y_true = inverse_transform_positions(y_test, scaler)

# 오차 계산 (유클리드 거리)
errors = np.linalg.norm(y_pred - y_true, axis=1)
print(f"테스트 세트 평균 위치 오차 (거리): {np.mean(errors):.4f}")
print(f"테스트 세트 MSE: {np.mean((y_pred - y_true)**2):.4f}")
print(f"테스트 세트 MAE: {np.mean(np.abs(y_pred - y_true)):.4f}")

# 10. 마지막 20개 프레임(1초)으로 3초 뒤 위치 예측 (테스트 데이터 기준)
last_seq_test = scaled_test[-20:].reshape(1, 20, 12)
scaled_pred_test = model.predict(last_seq_test)[0]

dummy = np.zeros((1, 12))
dummy[0, :2] = scaled_pred_test
inv_pred_test = scaler.inverse_transform(dummy)[0][:2]

print(f"테스트 데이터 기준 예측된 3초 뒤 유저 위치: {inv_pred_test}")

# 11. 시각화 (테스트 데이터 기준)
def plot_prediction(df, pred_point):
    plt.figure(figsize=(8, 6))
    plt.plot(df['userx'], df['usery'], label='User Path', color='blue')
    plt.scatter(df['userx'].iloc[0], df['usery'].iloc[0], color='green', label='Start Point', s=100)  # 시작점 초록색 점
    plt.scatter(pred_point[0], pred_point[1], color='red', label='Predicted Position (3s later)', s=100)
    plt.xlabel('X')
    plt.ylabel('Y')
    plt.title('User Movement and 3s Prediction')
    plt.legend()
    plt.grid(True)
    plt.axis('equal')
    plt.show()

plot_prediction(df_test, inv_pred_test)
