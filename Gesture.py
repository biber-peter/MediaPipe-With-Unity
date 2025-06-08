import math
import os

import cv2
import numpy as np
from cvzone.HandTrackingModule import HandDetector #导入手部检测模块 使用HandDetector类处理视频帧
import socket
import mediapipe as mp

import tkinter as tk


switch = False
port = 5056

# Parameters
width, height = 1280, 720

# Webcam
cap = cv2.VideoCapture(0)
cap.set(3, width)
cap.set(4, height)

# Hand Detector
detector = HandDetector(maxHands=2, detectionCon=0.8)

# Communication
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

#通过多项式拟合计算距离与厘米的转换系数
x = [300, 245, 200, 170, 145, 130, 112, 103, 93, 87, 80, 75, 70, 67, 62, 59, 57]
y = [20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100]
coff = np.polyfit(x, y, 2) #np.polyfit 是 NumPy 库中的函数，用于对数据进行多项式拟合
d1=0

#创建GUI窗口，用于设置端口号
window = tk.Tk()
window.title('tracking hand')
window.geometry('400x200')
labelP = tk.Label(window, text='port: ').place(x=150, y=0)
var = tk.IntVar()
var.set(port)
label = tk.Label(window, textvariable=var)
label.pack()
entery = tk.Entry(window, show=None)
entery.pack()
#从输入框获取端口号并更新全局变量 port
def insertPort():
    global port
    port = entery.get()
    port = int(port)
    var.set(port)
#主循环函数，负责摄像头捕获、手部检测和数据发送
def changeSwitch():
    serverAddressPort = ("127.0.0.1", port)
    switch = True
    while switch:
        success, img = cap.read()
#检测手部关键点并初始化数据存储变量
        # Hands
        hands, img = detector.findHands(img)
        data = []
        lmList = []
        lmList1 = []
        HandLeft = []
        HandRight = []
        # Landmark values
        #处理检测到的手部关键点
        if hands:
            hand = hands[0]  # get first hand detected
            handType = hand['type']  #获取手部类型（左/右手）
            lmList.append(handType)
            lmList.append(hand['lmList'])  #获取手部关键点列表
            # 处理检测到一只手时，正确区分左右手
            if lmList[0] == "Left" and len(hands) == 1: #lmList[0] 是表示该手的类型字符串（"Left" 或 "Right"）
                HandLeft = hand['lmList'] #如果检测到一只手是左手，则赋值给Handleft
            else:
                HandRight = hand['lmList']
            #处理检测到两只手时，正确区分左右手
            if len(hands) == 2:
                hand1 = hands[1]
                hand1Type = hand1['type']
                lmList1.append(hand1Type)
                lmList1.append(hand1['lmList'])

                if lmList1[0] == "Left":
                    HandLeft = hand1['lmList']
                    HandRight = hand['lmList']
                else:
                    HandLeft = hand['lmList']
                    HandRight = hand1['lmList']
            #计算手部关键点之间的距离并转换为厘米
            if len(HandLeft) != 0 and len(HandRight) != 0:
                Lx1, Ly1 = HandLeft[5][:2]
                Lx2, Ly2 = HandLeft[17][:2] #计算左手关键点第5个和第17个点的像素距离
                Ldistance = int(math.sqrt((Ly2 - Ly1) ** 2 + (Lx2 - Lx1) ** 2))
                A, B, C = coff
                LdistanceCM = A * Ldistance ** 2 + B * Ldistance + C

                Rx1, Ry1 = HandRight[5][:2]
                Rx2, Ry2 = HandRight[17][:2]
                Rdistance = int(math.sqrt((Ry2 - Ry1) ** 2 + (Rx2 - Rx1) ** 2))
                A, B, C = coff
                RdistanceCM = A * Rdistance ** 2 + B * Rdistance + C
            #分别应对只检测到左手或右手的情况
            elif len(HandLeft) != 0 and len(HandRight) == 0:
                Lx1, Ly1 = HandLeft[5][:2]
                Lx2, Ly2 = HandLeft[17][:2]
                Ldistance = int(math.sqrt((Ly2 - Ly1) ** 2 + (Lx2 - Lx1) ** 2)) #用勾股定理计算两点间距离
                A, B, C = coff #coff 是之前用 np.polyfit 获得的二次多项式系数 用于将像素距离转换为厘米的估算
                LdistanceCM = A * Ldistance ** 2 + B * Ldistance + C #使用二次多项式计算实际距离
            elif len(HandLeft) == 0 and len(HandRight) != 0:
                Rx1, Ry1 = HandRight[5][:2]
                Rx2, Ry2 = HandRight[17][:2]
                Rdistance = int(math.sqrt((Ry2 - Ry1) ** 2 + (Rx2 - Rx1) ** 2))
                A, B, C = coff
                RdistanceCM = A * Rdistance ** 2 + B * Rdistance + C
            # print(LdistanceCM, RdistanceCM)

            if len(HandLeft) != 0:
                data.extend(["Left"]) #如果检测到左手（HandLeft非空），在列表 data 中添加字符串 "Left"，作为左手数据的标识
            for lm in HandLeft:
                data.extend([lm[0], height - lm[1], lm[2], LdistanceCM]) #y坐标做了翻转处理（height减去原y），
                # 因为图像的坐标原点通常在左上角 LdistanceCM：左手特定两个关键点计算出来的估计实际距离（单位厘米），作为附加信息
            if len(HandRight) != 0:
                data.extend(["Right"])
            for lm1 in HandRight:
                data.extend([lm1[0], height - lm1[1], lm1[2], RdistanceCM])
            print(data)
            sock.sendto(str.encode(str(data)), serverAddressPort)
            #过UDP协议，将 data 列表（转换成字符串编码）发送到指定的本机或远程地址 serverAddressPort，
            # 供其他程序（如Unity）接收并进一步处理

        img = cv2.resize(img, (0, 0), None, 0.5, 0.5)
        cv2.imshow("Image", img)
        cv2.waitKey(1)
        if cv2.getWindowProperty('Image', cv2.WND_PROP_VISIBLE) < 1:
            switch = False

#创建GUI按钮并启动主循环
button = tk.Button(window,text='Enter', width=15, height=2, command=insertPort)
button.pack()
button = tk.Button(window,text='Start', width=15, height=2, command=changeSwitch)
button.pack()
window.mainloop()