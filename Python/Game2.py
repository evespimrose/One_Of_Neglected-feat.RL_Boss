import numpy as np
import matplotlib.pyplot as plt
from sklearn.ensemble import RandomForestRegressor
from sklearn.metrics import mean_squared_error

# 데이터 불러오기
def load_movement_data(filepath):
    data = []
    with open(filepath, 'r') as f:
        for line in f:
            x, y = map(float, line.strip().split(','))
            data.append((x, y))
    return np.array(data)

# 학습 데이터 생성
def prepare_dataset(data, input_len=10, pred_offset=30):
    X, y = [], []
    for i in range(len(data) - input_len - pred_offset):
        input_seq = data[i:i+input_len].flatten()
        target_pos = data[i + input_len + pred_offset]
        X.append(input_seq)
        y.append(list(target_pos))
    return np.array(X), np.array(y)

# 모델 학습
def train_model(X, y):
    model_x = RandomForestRegressor()
    model_y = RandomForestRegressor()
    model_x.fit(X, y[:, 0])
    model_y.fit(X, y[:, 1])
    return model_x, model_y

# 예측
def predict_next_position(model_x, model_y, input_seq):
    input_seq = input_seq.flatten().reshape(1, -1)
    pred_x = model_x.predict(input_seq)[0]
    pred_y = model_y.predict(input_seq)[0]
    return pred_x, pred_y

# 저장
def save_prediction(pred_x, pred_y, filename='predicted_position.txt'):
    with open(filename, 'w') as f:
        f.write(f"{pred_x:.4f},{pred_y:.4f}\n")

# 시각화
def plot_with_prediction(data, pred_point, input_seq=None):
    plt.figure(figsize=(8, 6))
    plt.plot(data[:, 0], data[:, 1], label='User Path', color='blue', linewidth=1)
    plt.scatter(pred_point[0], pred_point[1], color='red', label='Predicted Position (3s later)', s=100)
    if input_seq is not None:
        plt.plot(input_seq[:, 0], input_seq[:, 1], color='green', label='Input Sequence', linewidth=2)
    plt.xlabel('X')
    plt.ylabel('Y')
    plt.title('Player Movement & 3s Prediction')
    plt.legend()
    plt.grid(True)
    plt.axis('equal')
    plt.show()

filepath = '/Users/ice31/Desktop/user_movement_log.txt'
data = load_movement_data(filepath)

X, y = prepare_dataset(data)
print("y shape:", y.shape)
print("y example:", y[:5])

# train/test split (80% train, 20% test)
split_idx = int(len(X) * 0.8)
X_train, X_test = X[:split_idx], X[split_idx:]
y_train, y_test = y[:split_idx], y[split_idx:]

model_x, model_y = train_model(X_train, y_train)

# 테스트셋에 대해 예측
pred_x_test = model_x.predict(X_test)
pred_y_test = model_y.predict(X_test)

# MSE 계산
mse_x = mean_squared_error(y_test[:, 0], pred_x_test)
mse_y = mean_squared_error(y_test[:, 1], pred_y_test)
print(f"Test MSE for X: {mse_x:.4f}")
print(f"Test MSE for Y: {mse_y:.4f}")

# 마지막 입력 시퀀스로 예측
input_seq = data[-(10 + 30):-30]
pred_x, pred_y = predict_next_position(model_x, model_y, input_seq)
print(f"3초 뒤 예측 위치: x = {pred_x:.4f}, y = {pred_y:.4f}")
save_prediction(pred_x, pred_y)
plot_with_prediction(data, pred_point=(pred_x, pred_y), input_seq=input_seq)
