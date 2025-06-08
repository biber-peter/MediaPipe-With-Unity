using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
//����ɫ��������װ��һ����״�ṹ�ڵ㣬ÿ���ڵ㱣����Transform�������Ӧ��������ʼ��ת��ָ��/�ӽڵ������
public class Body : MonoBehaviour
{
    public class AvatarTree
    {
        public Transform transf;//��Ӧ�����ɫ�Ĺ���λ��
        public AvatarTree child;//�ӹؽڣ�����������
        public AvatarTree parent;//���ؽڣ���һ�������
        public int idx;  // �ؽڵ��������
        public Quaternion quaternion;//������ʼ��ת����̬��


        public AvatarTree(Transform tf, int idx, Quaternion quaternion, AvatarTree parent = null)
        {
            this.transf = tf;
            this.parent = parent;
            this.idx = idx;
            this.quaternion = quaternion;
        }
        /// <summary>
        /// ��ȡ��ǰ������Ը������ķ�������
        /// </summary>
        /// <returns></returns>
        public Vector3 GetDir()
        {
            if (parent != null)
            {
                return transf.position - parent.transf.position; //�ӹ���λ�ü�ȥ������λ�ã��õ�������������
            }
            return Vector3.up; //���ڵ�Ĭ������
        }
    }
    //��Щ Transform ������Unity����ģ���ж�Ӧ�Ĺ�����λ��ͨ��Animator��ȡ
    public Animator anim;//��ɫ����������
    public Transform hip;//�β�
    public Transform spine;//��׵
    public Transform thorax;//�ز�
    public Transform neck;//����
    public Transform head;//ͷ��
    public Transform nose;//����
    public Transform lHip;//����
    public Transform lKnee;//��ϥ
    public Transform lFoot;//���
    public Transform rHip;//����
    public Transform rKnee;//��ϥ
    public Transform rFoot;//�ҽ�
    public Transform lSld;//���
    public Transform lArm;//���
    public Transform lEblow;//����
    public Transform lWrist;//������
    public Transform rSld;//�Ҽ�
    public Transform rArm;//�Ұ�
    public Transform rEblow;//����
    public Transform rWrist;//������
    //Transform��Ӧ�Ĺ������ڵ㣬�� AvatarTree ��װ�ɸ��ӽṹ������ݹ����
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
    private void InitAvatar()//��Unity�Դ��ı�׼�����󶨵����������ڶԹ������п���
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
    }//ƥ��ؽڵ�
    private void BulidTree()//����������������ĸ��Ӳ㼶��ϵ��
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
        // ���ø����������� - 1��-2�������Զ���������м�����ڵ�
    }//ƥ��ؽڸ��ӽڵ�
    void Update()
    {
        lerp += Time.deltaTime; //���Ӳ�ֵ����������ƽ������
        if (lerp >= 1.0f)
        {
            lerp = 0;
        }
        if (data != "")
        {
            datas = Array.ConvertAll(data.Remove(data.Length - 1).Split(','), float.Parse);//�����ַ������ݰ����Ų�֣���ת����float����
            //�ݹ���¹������������β����ұۡ������ع�����ת
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
        //��ΪMediapipe���ı�׼�ؼ��������Ĺؽڵ㲻ͬ����������Mediapipe����������Ҽ����������ĵ㣬��ʵ������Ҫ���Ǽ�׵�Ĺؽڵ㣩
        if (idx == -1)// �β��������������е�
        {
            x = (datas[23 * 3] + datas[24 * 3]) / 2;
            y = (datas[23 * 3 + 1] + datas[24 * 3 + 1]) / 2;
            z = (datas[23 * 3 + 2] + datas[24 * 3 + 2]) / 2;
        }
        else if (idx == -2)// ��׵�����������е��Ȩƽ��
        {
            x = ((datas[11 * 3] + datas[12 * 3]) / 2 + (datas[23 * 3] + datas[24 * 3]) / 2) / 2;
            y = ((datas[11 * 3 + 1] + datas[12 * 3 + 1]) / 2 + (datas[23 * 3 + 1] + datas[24 * 3 + 1]) / 2) / 2;
            z = ((datas[11 * 3 + 2] + datas[12 * 3 + 2]) / 2 + (datas[23 * 3 + 2] + datas[24 * 3 + 2]) / 2) / 2;
        }
        else if (idx == -3)// �ز��������Ҽ��е�
        {
            x = (datas[11 * 3] + datas[12 * 3]) / 2;
            y = (datas[11 * 3 + 1] + datas[12 * 3 + 1]) / 2;
            z = (datas[11 * 3 + 2] + datas[12 * 3 + 2]) / 2;
        }
        else if (idx == -4)//������ͷ�����ҷ��������Ȩƽ��
        {
            x = ((datas[8 * 3] + datas[7 * 3]) / 2 + (datas[11 * 3] + datas[12 * 3]) / 2) / 2;
            y = ((datas[8 * 3 + 1] + datas[7 * 3 + 1]) / 2 + (datas[11 * 3 + 1] + datas[12 * 3 + 1]) / 2) / 2;
            z = ((datas[8 * 3 + 2] + datas[7 * 3 + 2]) / 2 + (datas[11 * 3 + 2] + datas[12 * 3 + 2]) / 2) / 2;
        }
        else if (idx == -5)//ͷ��
        {
            x = (datas[8 * 3] + datas[7 * 3]) / 2;
            y = (datas[8 * 3 + 1] + datas[7 * 3 + 1]) / 2;
            z = (datas[8 * 3 + 2] + datas[7 * 3 + 2]) / 2;
        }
        else if (idx == -6)//��磺����Ȩƽ��
        {
            x = (datas[11 * 3] * 3 + datas[12 * 3]) / 4;
            y = (datas[11 * 3 + 1] * 3 + datas[12 * 3 + 1]) / 4;
            z = (datas[11 * 3 + 2] * 3 + datas[12 * 3 + 2]) / 4;
        }
        else if (idx == -7)//�Ҽ磺�Ҽ��Ȩƽ��
        {
            x = (datas[11 * 3] + datas[12 * 3] * 3) / 4;
            y = (datas[11 * 3 + 1] + datas[12 * 3 + 1] * 3) / 4;
            z = (datas[11 * 3 + 2] + datas[12 * 3 + 2] * 3) / 4;
        }
        else//�����ڵ�ֱ��ȡ��Ӧ����������
        {
            x = datas[idx * 3];
            y = datas[idx * 3 + 1];
            z = datas[idx * 3 + 2];
        }//����ת�����Vector3����������ȡ��
        return new Vector3(-x, y, -z);
    }
    private void UpdateTree(AvatarTree tree, float lerp)//�Ӹ��ڵ㿪ʼ���ڸ��ӹ�ϵ�еݹ���¹�����ת
    {
        if (tree.parent != null)
        {
            UpdateBone(tree, lerp);
        }
        if (tree.child != null)
        {
            UpdateTree(tree.child, lerp);
        }
    }//�����ؽ�
    private void UpdateBone(AvatarTree tree, float lerp)
    {
        var dir1 = tree.GetDir();//��ǰ��������ڸ������ķ�����������ǰ������̬��
        var dir2 = GetData(tree.parent.idx) - GetData(tree.idx);//Ŀ����̬��Mediapipe�ɼ������꣩�и��ӽڵ�����
        Quaternion rot = Quaternion.FromToRotation(dir1, dir2);//�����dir1��ת��dir2�������ת��Ԫ��
        Quaternion rot1 = tree.parent.transf.rotation;//��������ǰ��ת
        tree.parent.transf.rotation = Quaternion.Lerp(rot1, rot * rot1, lerp);//�������Բ�ֵƽ����ת��Ӧ����ת������������
    }//���������ת
}
