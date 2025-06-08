using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
//将角色各骨骼封装成一个树状结构节点，每个节点保存了Transform组件、对应索引、初始旋转和指向父/子节点的引用
public class Body : MonoBehaviour
{
    public class AvatarTree
    {
        public Transform transf;//对应虚拟角色的骨骼位置
        public AvatarTree child;//子关节（后续骨骼）
        public AvatarTree parent;//父关节（上一层骨骼）
        public int idx;  // 关节点数据序号
        public Quaternion quaternion;//骨骼初始旋转（姿态）


        public AvatarTree(Transform tf, int idx, Quaternion quaternion, AvatarTree parent = null)
        {
            this.transf = tf;
            this.parent = parent;
            this.idx = idx;
            this.quaternion = quaternion;
        }
        /// <summary>
        /// 获取当前骨骼相对父骨骼的方向向量
        /// </summary>
        /// <returns></returns>
        public Vector3 GetDir()
        {
            if (parent != null)
            {
                return transf.position - parent.transf.position; //子骨骼位置减去父骨骼位置，得到骨骼方向向量
            }
            return Vector3.up; //根节点默认向上
        }
    }
    //这些 Transform 对象是Unity虚拟模型中对应的骨骼部位，通过Animator获取
    public Animator anim;//角色动画控制器
    public Transform hip;//臀部
    public Transform spine;//脊椎
    public Transform thorax;//胸部
    public Transform neck;//颈部
    public Transform head;//头部
    public Transform nose;//鼻子
    public Transform lHip;//左臀
    public Transform lKnee;//左膝
    public Transform lFoot;//左脚
    public Transform rHip;//右臀
    public Transform rKnee;//右膝
    public Transform rFoot;//右脚
    public Transform lSld;//左肩
    public Transform lArm;//左膀
    public Transform lEblow;//左肘
    public Transform lWrist;//左手腕
    public Transform rSld;//右肩
    public Transform rArm;//右膀
    public Transform rEblow;//右肘
    public Transform rWrist;//右手腕
    //Transform对应的骨骼树节点，用 AvatarTree 封装成父子结构，方便递归遍历
    public AvatarTree Hip;
    public AvatarTree LHip;
    private AvatarTree LKnee;
    private AvatarTree LFoot;
    public AvatarTree RHip;
    private AvatarTree RKnee;
    private AvatarTree RFoot;
    private AvatarTree Spine;
    private AvatarTree Thorax;
    private AvatarTree Neck;
    private AvatarTree Head;
    private AvatarTree Nose;
    public AvatarTree LSld;
    private AvatarTree LEblow;
    private AvatarTree LWrist;
    public AvatarTree RSld;
    private AvatarTree REblow;
    private AvatarTree RWrist;
    private AvatarTree LArm;
    private AvatarTree RArm;
    //Thread receiveThread;
    //UdpClient client;
    //public int port = 5056;
    public string data;
    public float[] datas;
    public float lerp;
    private void Start()
    {
        InitAvatar();
        BulidTree();
    }
    private void InitAvatar()//把Unity自带的标准骨骼绑定到变量，便于对骨骼进行控制
    {
        if (anim != null)
        {
            lHip = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            lKnee = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            lFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
            rHip = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            rKnee = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            rFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
            spine = anim.GetBoneTransform(HumanBodyBones.Spine);
            thorax = anim.GetBoneTransform(HumanBodyBones.Chest);
            neck = anim.GetBoneTransform(HumanBodyBones.Neck);
            head = anim.GetBoneTransform(HumanBodyBones.Head);
            lSld = anim.GetBoneTransform(HumanBodyBones.LeftShoulder);
            lArm = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            lEblow = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            lWrist = anim.GetBoneTransform(HumanBodyBones.LeftHand);
            rSld = anim.GetBoneTransform(HumanBodyBones.RightShoulder);
            rArm = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
            rEblow = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
            rWrist = anim.GetBoneTransform(HumanBodyBones.RightHand);
        }
    }//匹配关节点
    private void BulidTree()//建立虚拟人物骨骼的父子层级关系树
    {
        Hip = new AvatarTree(hip, -1, hip.rotation);// -1
        Spine = Hip.child = new AvatarTree(spine, -2, spine.rotation, Hip);// -2
        Thorax = Spine.child = new AvatarTree(thorax, -3, thorax.rotation, Spine);// -3
        Neck = Thorax.child = new AvatarTree(neck, -4, neck.rotation, Thorax);// -4
        Head = Neck.child = new AvatarTree(head, -5, head.rotation, Neck);// -5
        Nose = Head.child = new AvatarTree(nose, 0, nose.rotation, Head);
        LHip = new AvatarTree(lHip, 23, lHip.rotation);
        LKnee = LHip.child = new AvatarTree(lKnee, 25, lKnee.rotation, LHip);
        LFoot = LKnee.child = new AvatarTree(lFoot, 29, lFoot.rotation, LKnee);
        RHip = new AvatarTree(rHip, 24, rHip.rotation);
        RKnee = RHip.child = new AvatarTree(rKnee, 26, rHip.rotation, RHip);
        RFoot = RKnee.child = new AvatarTree(rFoot, 30, rFoot.rotation, RKnee);
        LSld = new AvatarTree(lSld, -6, lSld.rotation);// -6
        LArm = LSld.child = new AvatarTree(lArm, 11, lArm.rotation, LSld);
        LEblow = LArm.child = new AvatarTree(lEblow, 13, lEblow.rotation, LArm);
        LWrist = LEblow.child = new AvatarTree(lWrist, 15, lWrist.rotation, LEblow);
        RSld = new AvatarTree(rSld, -7, rSld.rotation); // -7
        RArm = RSld.child = new AvatarTree(rArm, 12, rArm.rotation, RSld);
        REblow = RArm.child = new AvatarTree(rEblow, 14, rEblow.rotation, RArm);
        RWrist = REblow.child = new AvatarTree(rWrist, 16, rWrist.rotation, REblow);
        // 利用负数索引（如 - 1，-2）代表自定义或计算的中间骨骼节点
    }//匹配关节父子节点
    void Update()
    {
        lerp += Time.deltaTime; //增加插值参数，控制平滑过渡
        if (lerp >= 1.0f)
        {
            lerp = 0;
        }
        if (data != "")
        {
            datas = Array.ConvertAll(data.Remove(data.Length - 1).Split(','), float.Parse);//传入字符串数据按逗号拆分，并转换成float数组
            //递归更新骨骼树，更新臀部、右臂、左臂相关骨骼旋转
            UpdateTree(Hip, lerp);
            UpdateTree(RArm, lerp);
            UpdateTree(LArm, lerp);
            UpdateTree(LHip, lerp);
            UpdateTree(RHip, lerp);
        }
    }
    private Vector3 GetData(int idx)
    {
        float x, y, z;
        //因为Mediapipe给的标准关键点跟人物的关节点不同（比如上体Mediapipe给的是左肩右肩左腰右腰的点，但实际上需要的是脊椎的关节点）
        if (idx == -1)// 臀部：左臀与右臀中点
        {
            x = (datas[23 * 3] + datas[24 * 3]) / 2;
            y = (datas[23 * 3 + 1] + datas[24 * 3 + 1]) / 2;
            z = (datas[23 * 3 + 2] + datas[24 * 3 + 2]) / 2;
        }
        else if (idx == -2)// 脊椎，左肩和左臀中点加权平均
        {
            x = ((datas[11 * 3] + datas[12 * 3]) / 2 + (datas[23 * 3] + datas[24 * 3]) / 2) / 2;
            y = ((datas[11 * 3 + 1] + datas[12 * 3 + 1]) / 2 + (datas[23 * 3 + 1] + datas[24 * 3 + 1]) / 2) / 2;
            z = ((datas[11 * 3 + 2] + datas[12 * 3 + 2]) / 2 + (datas[23 * 3 + 2] + datas[24 * 3 + 2]) / 2) / 2;
        }
        else if (idx == -3)// 胸部：左肩和右肩中点
        {
            x = (datas[11 * 3] + datas[12 * 3]) / 2;
            y = (datas[11 * 3 + 1] + datas[12 * 3 + 1]) / 2;
            z = (datas[11 * 3 + 2] + datas[12 * 3 + 2]) / 2;
        }
        else if (idx == -4)//颈部：头部左右方向坐标加权平均
        {
            x = ((datas[8 * 3] + datas[7 * 3]) / 2 + (datas[11 * 3] + datas[12 * 3]) / 2) / 2;
            y = ((datas[8 * 3 + 1] + datas[7 * 3 + 1]) / 2 + (datas[11 * 3 + 1] + datas[12 * 3 + 1]) / 2) / 2;
            z = ((datas[8 * 3 + 2] + datas[7 * 3 + 2]) / 2 + (datas[11 * 3 + 2] + datas[12 * 3 + 2]) / 2) / 2;
        }
        else if (idx == -5)//头部
        {
            x = (datas[8 * 3] + datas[7 * 3]) / 2;
            y = (datas[8 * 3 + 1] + datas[7 * 3 + 1]) / 2;
            z = (datas[8 * 3 + 2] + datas[7 * 3 + 2]) / 2;
        }
        else if (idx == -6)//左肩：左肩加权平均
        {
            x = (datas[11 * 3] * 3 + datas[12 * 3]) / 4;
            y = (datas[11 * 3 + 1] * 3 + datas[12 * 3 + 1]) / 4;
            z = (datas[11 * 3 + 2] * 3 + datas[12 * 3 + 2]) / 4;
        }
        else if (idx == -7)//右肩：右肩加权平均
        {
            x = (datas[11 * 3] + datas[12 * 3] * 3) / 4;
            y = (datas[11 * 3 + 1] + datas[12 * 3 + 1] * 3) / 4;
            z = (datas[11 * 3 + 2] + datas[12 * 3 + 2] * 3) / 4;
        }
        else//其他节点直接取对应索引的坐标
        {
            x = datas[idx * 3];
            y = datas[idx * 3 + 1];
            z = datas[idx * 3 + 2];
        }//返回转换后的Vector3，坐标做了取反
        return new Vector3(-x, y, -z);
    }
    private void UpdateTree(AvatarTree tree, float lerp)//从根节点开始，在父子关系中递归更新骨骼旋转
    {
        if (tree.parent != null)
        {
            UpdateBone(tree, lerp);
        }
        if (tree.child != null)
        {
            UpdateTree(tree.child, lerp);
        }
    }//遍历关节
    private void UpdateBone(AvatarTree tree, float lerp)
    {
        var dir1 = tree.GetDir();//当前骨骼相对于父骨骼的方向向量（当前虚拟姿态）
        var dir2 = GetData(tree.parent.idx) - GetData(tree.idx);//目标姿态（Mediapipe采集的坐标）中父子节点向量
        Quaternion rot = Quaternion.FromToRotation(dir1, dir2);//计算从dir1旋转到dir2所需的旋转四元数
        Quaternion rot1 = tree.parent.transf.rotation;//父骨骼当前旋转
        tree.parent.transf.rotation = Quaternion.Lerp(rot1, rot * rot1, lerp);//采用线性插值平滑旋转，应用旋转增量到父骨骼
    }//计算骨骼旋转
}
