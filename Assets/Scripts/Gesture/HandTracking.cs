using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace landmarktest
{
    public class HandTracking : MonoBehaviour
    {
        public UDPReceive udpReceive;
        public List<GameObject> handPoints;

        [SerializeField]
        private float thumbModelLength = 0.03f;
        private float scale;
        private DepthCalibrator depthCalibrator = new DepthCalibrator(-0.0719f, 0.439f);
        private TransformLink[] transformLinkers;
        public string LinkType = "None";

        int flagR = 0;
        int flagL = 0;
        float Rx0 = 0;
        float Ry0 = 0;
        float Rz0 = 0;
        float RzD = 0;
        float Lx0 = 0;
        float Ly0 = 0;
        float Lz0 = 0;
        float LzD = 0;

        void Awake()
        {
            transformLinkers = this.transform.GetComponentsInChildren<TransformLink>();
        }

        void Update()
        {
            string data = udpReceive.data;

            if (string.IsNullOrEmpty(data) || data.Length < 2)
            {
                // 数据为空或过短，跳过处理
                return;
            }

            // 安全移除首尾字符
            data = data.Remove(0, 1);
            data = data.Remove(data.Length - 1, 1);

            string data1 = data;

            // 调试打印接收到的数据
            print(data);

            int indexRight = data.LastIndexOf("Right");
            int indexLeft = data.LastIndexOf("Left");

            string[] pointsLeft = null;
            string[] pointsRight = null;

            try
            {
                if (indexLeft != -1 && indexRight != -1)
                {
                    // 处理含有 Left 和 Right 的字符串
                    int removeEndPos = Mathf.Max(indexRight - 3, 0);
                    if (removeEndPos <= data.Length)
                        data = data.Remove(removeEndPos);
                    else
                        data = "";

                    int startPosLeft = indexLeft + 6;
                    if (startPosLeft <= data.Length)
                        data = data.Remove(0, startPosLeft);
                    else
                        data = "";

                    pointsLeft = string.IsNullOrEmpty(data) ? null : data.Split(',');

                    int startPosRight = data1.LastIndexOf("Right") + 7;
                    if (startPosRight <= data1.Length)
                        data1 = data1.Remove(0, startPosRight);
                    else
                        data1 = "";

                    pointsRight = string.IsNullOrEmpty(data1) ? null : data1.Split(',');
                }
                else if (indexLeft != -1 && indexRight == -1)
                {
                    int startPosLeft = indexLeft + 6;
                    if (startPosLeft <= data.Length)
                        data = data.Remove(0, startPosLeft);
                    else
                        data = "";

                    pointsLeft = string.IsNullOrEmpty(data) ? null : data.Split(',');
                    print("OnlyL" + data);
                }
                else if (indexLeft == -1 && indexRight != -1)
                {
                    int startPosRight = indexRight + 7;
                    if (startPosRight <= data1.Length)
                        data1 = data1.Remove(0, startPosRight);
                    else
                        data1 = "";

                    pointsRight = string.IsNullOrEmpty(data1) ? null : data1.Split(',');
                    print("OnlyR" + data1);
                }
                else
                {
                    pointsLeft = null;
                    pointsRight = null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"字符串解析异常: {e.Message}");
                return;
            }

            // 更新左手关键点位置
            if (LinkType == "Left" && pointsLeft != null && pointsLeft.Length >= handPoints.Count * 4)
            {
                for (int i = 1; i < handPoints.Count; i++)
                {
                    try
                    {
                        float x = float.Parse(pointsLeft[i * 4]) - float.Parse(pointsLeft[0]);
                        float y = float.Parse(pointsLeft[i * 4 + 1]) - float.Parse(pointsLeft[1]);
                        float z = float.Parse(pointsLeft[i * 4 + 2]) - float.Parse(pointsLeft[2]);

                        LzD = float.Parse(pointsLeft[i * 4 + 3]);

                        if (x == 0 && y == 0 && z == 0)
                            return;

                        handPoints[i].transform.localPosition = new Vector3(x, y, z);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"左手关键点解析错误，索引{i}: {e.Message}");
                    }
                }
            }

            // 更新右手关键点位置
            if (LinkType == "Right" && pointsRight != null && pointsRight.Length >= handPoints.Count * 4)
            {
                for (int i = 1; i < handPoints.Count; i++)
                {
                    try
                    {
                        float x = float.Parse(pointsRight[i * 4]) - float.Parse(pointsRight[0]);
                        float y = float.Parse(pointsRight[i * 4 + 1]) - float.Parse(pointsRight[1]);
                        float z = float.Parse(pointsRight[i * 4 + 2]) - float.Parse(pointsRight[2]);

                        RzD = float.Parse(pointsRight[i * 4 + 3]);

                        if (x == 0 && y == 0 && z == 0)
                            return;

                        handPoints[i].transform.localPosition = new Vector3(x, y, z);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"右手关键点解析错误，索引{i}: {e.Message}");
                    }
                }
            }

            // 根据右手数据更新模型位置和缩放
            if (LinkType == "Right" && pointsRight != null && pointsRight.Length >= 7)
            {
                float RHx = 0.557769716f;
                float RHy = 0.728625596f;
                float RHz = 0.126790136f;

                float depth = depthCalibrator.GetDepthFromThumbLength(scale);

                if (flagR == 0)
                {
                    Rx0 = float.Parse(pointsRight[0]);
                    Ry0 = float.Parse(pointsRight[1]);
                    Rz0 = RzD;
                    flagR = 1;
                }

                this.transform.localPosition = new Vector3((float.Parse(pointsRight[1]) - Ry0) / 1000 + RHx,
                                                           (-float.Parse(pointsRight[0]) + Rx0) / 1000 + RHy,
                                                           RHz + (RzD - Rz0) / 200);

                var pointA = new Vector3(float.Parse(pointsRight[0]), float.Parse(pointsRight[1]), float.Parse(pointsRight[2]));
                var pointB = new Vector3(float.Parse(pointsRight[4]), float.Parse(pointsRight[5]), float.Parse(pointsRight[6]));
                float thumbDetectedLength = Vector3.Distance(pointA, pointB);
                if (thumbDetectedLength == 0)
                    return;

                scale = thumbModelLength / thumbDetectedLength;
                this.transform.localScale = new Vector3(scale, scale, scale);
            }

            // 根据左手数据更新模型位置和缩放
            if (LinkType == "Left" && pointsLeft != null && pointsLeft.Length >= 7)
            {
                float LHx = 0.460089773f;
                float LHy = 0.420398116f;
                float LHz = 0.129199326f;

                float depth = depthCalibrator.GetDepthFromThumbLength(scale);

                if (flagL == 0)
                {
                    Lx0 = float.Parse(pointsLeft[0]);
                    Ly0 = float.Parse(pointsLeft[1]);
                    Lz0 = LzD;
                    flagL = 1;
                }

                this.transform.localPosition = new Vector3((float.Parse(pointsLeft[1]) - Ly0) / 1000 + LHx,
                                                           (-float.Parse(pointsLeft[0]) + Lx0) / 1000 + LHy,
                                                           LHz + (LzD - Lz0) / 200);

                var pointA = new Vector3(float.Parse(pointsLeft[0]), float.Parse(pointsLeft[1]), float.Parse(pointsLeft[2]));
                var pointB = new Vector3(float.Parse(pointsLeft[4]), float.Parse(pointsLeft[5]), float.Parse(pointsLeft[6]));
                float thumbDetectedLength = Vector3.Distance(pointA, pointB);
                if (thumbDetectedLength == 0)
                    return;

                scale = thumbModelLength / thumbDetectedLength;
                this.transform.localScale = new Vector3(scale, scale, scale);
            }

            // 更新手腕旋转
            updateWristRotation();

            // 更新子TransformLink组件
            foreach (var linker in transformLinkers)
            {
                linker.UpdateTransform();
            }
        }

        private void updateWristRotation()
        {
            if (handPoints == null || handPoints.Count < 10)
                return;

            var wristTransform = handPoints[0].transform;
            var indexFinger = handPoints[5].transform.position;
            var middleFinger = handPoints[9].transform.position;

            var vectorToMiddle = middleFinger - wristTransform.position;
            var vectorToIndex = indexFinger - wristTransform.position;

            Vector3.OrthoNormalize(ref vectorToMiddle, ref vectorToIndex);

            Vector3 normalVector = Vector3.Cross(vectorToIndex, vectorToMiddle);

            wristTransform.rotation = Quaternion.LookRotation(normalVector, vectorToIndex);
        }
    }
}