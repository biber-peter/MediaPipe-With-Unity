import socket
import os
import mediapipe as mp
import cv2

#有时候反复启动端口没有释放，需要杀死数据端口进程
def kill_port(port):
    print("try to kill %s pid..." % port)
    find_port= 'netstat -aon | findstr %s' % port #查找指定端口的进程ID
    result = os.popen(find_port)
    text = result.read()
    pid= text[-5:-1]
    if pid == "":
        print("not found %s pid..." % port)
        return
    else:
        find_kill= 'taskkill -f -pid %s' % pid
        result = os.popen(find_kill)
        return result.read()

def camera():
    cap = cv2.VideoCapture(0)

    #这里用的是Mediapipe的Holistic模型
    mp_holistic = mp.solutions.holistic
    with mp_holistic.Holistic(min_detection_confidence=0.5, min_tracking_confidence=0.5, model_complexity=1,
                              smooth_landmarks=True) as holistic: #初始检测 跟踪置信度阈值 模型复杂度 平滑关键点坐标
        while True:
            ret, image = cap.read() #获取一帧图像
            image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB) #实现 BGR 与 RGB 两种常用颜色格式之间的转换
            results = holistic.process(image)
            image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
            pos1 = pos2 = poseString = ""
            if results.left_hand_landmarks:
                # 可以选择画出左手识别结果
                # mp_drawing.draw_landmarks(img, results.left_hand_landmarks, mp_holistic.HAND_CONNECTIONS)
                for lm in results.left_hand_landmarks.landmark:
                    pos1 += f'{lm.x},{lm.y},{lm.z},'
                pos1 += 'Left,' #如果检测到左手关键点，遍历所有关键点坐标(x,y,z)，以逗号分隔拼接成字符串。
            if results.right_hand_landmarks:
                # 可以选择画出右手识别结果
                # mp_drawing.draw_landmarks(img, results.right_hand_landmarks, mp_holistic.HAND_CONNECTIONS)
                for lm in results.right_hand_landmarks.landmark:
                    pos2 += f'{lm.x},{lm.y},{lm.z},'
                pos2 += 'Right,'
            if results.pose_landmarks:
                # 可以选择画出全身识别结果
                # mp_drawing.draw_landmarks(img, results.pose_landmarks, mp_holistic.POSE_CONNECTIONS)
                for lm in results.pose_landmarks.landmark:
                    poseString += f'{lm.x},{lm.y},{lm.z},' #遍历识别到的全身关键点坐标，拼接成字符串。

            # 手部数据
            date1 = pos1 + pos2 + ';' #组合左右手坐标字符串
            # 身体数据
            date2 = poseString #为身体关键点数据字符串
            # 使用UDP发送识别数据给Unity端
            sock.sendto(str.encode(date1 + date2), serverAddressPort)
            cv2.imshow('0', image)
            if cv2.waitKey(10) == 27:
                break


if __name__ == '__main__':
    kill_port(5054)
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)  # 向5054端口发送数据
    serverAddressPort = ('127.0.0.1', 5054)
    camera()