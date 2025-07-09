import tensorflow as tf

print("TensorFlow 개체 목록: ", tf.__version__)
gpus = tf.config.list_physical_devices('GPU')
if gpus:
    print("감지된 GPU: ", gpus)
else:
    print("GPU를 찾을 수 없습니다.")
