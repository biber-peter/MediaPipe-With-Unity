using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
public class Hand : MonoBehaviour
{
    public class AvatarTree
    {
        public Transform transf;
        public AvatarTree[] childs;
        public AvatarTree parent;
        public int idx;
        public Quaternion quaternion;//起始旋转
        public AvatarTree(Transform tf, int count, int idx, Quaternion quaternion, AvatarTree parent = null)
        {
            this.transf = tf;
            this.parent = parent;
            this.idx = idx;
            this.quaternion = quaternion;
            if (count > 0)
            {
                childs = new AvatarTree[count];
            }
        }
        public Vector3 GetDir()
        {
            if (parent != null)
            {
                return transf.position - parent.transf.position;
            }
            return Vector3.up;
        }
    }
    public Transform l_wrist;//左手腕
    public Transform l_thumb1;//左拇指
    public Transform l_thumb2;
    public Transform l_thumb3;
    public Transform l_thumb4;
    public Transform l_index1;//左食指
    public Transform l_index2;
    public Transform l_index3;
    public Transform l_index4;
    public Transform l_middle1;//左中指
    public Transform l_middle2;
    public Transform l_middle3;
    public Transform l_middle4;
    public Transform l_ring1;//左无名指
    public Transform l_ring2;
    public Transform l_ring3;
    public Transform l_ring4;
    public Transform l_pinky1;//左小拇指
    public Transform l_pinky2;
    public Transform l_pinky3;
    public Transform l_pinky4;
    public Transform r_wrist;//右手腕
    public Transform r_thumb1;//右拇指
    public Transform r_thumb2;
    public Transform r_thumb3;
    public Transform r_thumb4;
    public Transform r_index1;//右食指
    public Transform r_index2;
    public Transform r_index3;
    public Transform r_index4;
    public Transform r_middle1;//右中指
    public Transform r_middle2;
    public Transform r_middle3;
    public Transform r_middle4;
    public Transform r_ring1;//右无名指
    public Transform r_ring2;
    public Transform r_ring3;
    public Transform r_ring4;
    public Transform r_pinky1;//右小拇指
    public Transform r_pinky2;
    public Transform r_pinky3;
    public Transform r_pinky4;
    public AvatarTree L_Wrist;
    private AvatarTree L_Thumb1;
    private AvatarTree L_Thumb2;
    private AvatarTree L_Thumb3;
    private AvatarTree L_Thumb4;
    private AvatarTree L_Index1;
    private AvatarTree L_Index2;
    private AvatarTree L_Index3;
    private AvatarTree L_Index4;
    private AvatarTree L_Middle1;
    private AvatarTree L_Middle2;
    private AvatarTree L_Middle3;
    private AvatarTree L_Middle4;
    private AvatarTree L_Ring1;
    private AvatarTree L_Ring2;
    private AvatarTree L_Ring3;
    private AvatarTree L_Ring4;
    private AvatarTree L_Pinky1;
    private AvatarTree L_Pinky2;
    private AvatarTree L_Pinky3;
    private AvatarTree L_Pinky4;
    public AvatarTree R_Wrist;
    private AvatarTree R_Thumb1;
    private AvatarTree R_Thumb2;
    private AvatarTree R_Thumb3;
    private AvatarTree R_Thumb4;
    private AvatarTree R_Index1;
    private AvatarTree R_Index2;
    private AvatarTree R_Index3;
    private AvatarTree R_Index4;
    private AvatarTree R_Middle1;
    private AvatarTree R_Middle2;
    private AvatarTree R_Middle3;
    private AvatarTree R_Middle4;
    private AvatarTree R_Ring1;
    private AvatarTree R_Ring2;
    private AvatarTree R_Ring3;
    private AvatarTree R_Ring4;
    private AvatarTree R_Pinky1;
    private AvatarTree R_Pinky2;
    private AvatarTree R_Pinky3;
    private AvatarTree R_Pinky4;
    public string data;
    public float[] l_datas;
    public float[] r_datas;
    public float lerp;
    private void Start()
    {
        BulidTree();
    }
    private void BulidTree()
    {
        L_Wrist = new AvatarTree(l_wrist, 5, 0, l_wrist.rotation);//左手
        L_Thumb1 = L_Wrist.childs[0] = new AvatarTree(l_thumb1, 1, 1, l_thumb1.rotation, L_Wrist);
        L_Index1 = L_Wrist.childs[1] = new AvatarTree(l_index1, 1, 5, l_index1.rotation, L_Wrist);
        L_Middle1 = L_Wrist.childs[2] = new AvatarTree(l_middle1, 1, 9, l_middle1.rotation, L_Wrist);
        L_Ring1 = L_Wrist.childs[3] = new AvatarTree(l_ring1, 1, 13, l_ring1.rotation, L_Wrist);
        L_Pinky1 = L_Wrist.childs[4] = new AvatarTree(l_pinky1, 1, 17, l_pinky1.rotation, L_Wrist);
        L_Thumb2 = L_Thumb1.childs[0] = new AvatarTree(l_thumb2, 1, 2, l_thumb2.rotation, L_Thumb1);
        L_Thumb3 = L_Thumb2.childs[0] = new AvatarTree(l_thumb3, 1, 3, l_thumb3.rotation, L_Thumb2);
        L_Thumb4 = L_Thumb3.childs[0] = new AvatarTree(l_thumb4, 0, 4, l_thumb4.rotation, L_Thumb3);
        L_Index2 = L_Index1.childs[0] = new AvatarTree(l_index2, 1, 6, l_index2.rotation, L_Index1);
        L_Index3 = L_Index2.childs[0] = new AvatarTree(l_index3, 1, 7, l_index3.rotation, L_Index2);
        L_Index4 = L_Index3.childs[0] = new AvatarTree(l_index4, 0, 8, l_index4.rotation, L_Index3);
        L_Middle2 = L_Middle1.childs[0] = new AvatarTree(l_middle2, 1, 10, l_middle2.rotation, L_Middle1);
        L_Middle3 = L_Middle2.childs[0] = new AvatarTree(l_middle3, 1, 11, l_middle3.rotation, L_Middle2);
        L_Middle4 = L_Middle3.childs[0] = new AvatarTree(l_middle4, 0, 12, l_middle4.rotation, L_Middle3);
        L_Ring2 = L_Ring1.childs[0] = new AvatarTree(l_ring2, 1, 14, l_ring2.rotation, L_Ring1);
        L_Ring3 = L_Ring2.childs[0] = new AvatarTree(l_ring3, 1, 15, l_ring3.rotation, L_Ring2);
        L_Ring4 = L_Ring3.childs[0] = new AvatarTree(l_ring4, 0, 16, l_ring4.rotation, L_Ring3);
        L_Pinky2 = L_Pinky1.childs[0] = new AvatarTree(l_pinky2, 1, 18, l_pinky2.rotation, L_Pinky1);
        L_Pinky3 = L_Pinky2.childs[0] = new AvatarTree(l_pinky3, 1, 19, l_pinky3.rotation, L_Pinky2);
        L_Pinky4 = L_Pinky3.childs[0] = new AvatarTree(l_pinky4, 0, 20, l_pinky4.rotation, L_Pinky3);
        R_Wrist = new AvatarTree(r_wrist, 5, 0, r_wrist.rotation);//右手
        R_Thumb1 = R_Wrist.childs[0] = new AvatarTree(r_thumb1, 1, 1, r_thumb1.rotation, R_Wrist);
        R_Index1 = R_Wrist.childs[1] = new AvatarTree(r_index1, 1, 5, r_index1.rotation, R_Wrist);
        R_Middle1 = R_Wrist.childs[2] = new AvatarTree(r_middle1, 1, 9, r_middle1.rotation, R_Wrist);
        R_Ring1 = R_Wrist.childs[3] = new AvatarTree(r_ring1, 1, 13, r_ring1.rotation, R_Wrist);
        R_Pinky1 = R_Wrist.childs[4] = new AvatarTree(r_pinky1, 1, 17, r_pinky1.rotation, R_Wrist);
        R_Thumb2 = R_Thumb1.childs[0] = new AvatarTree(r_thumb2, 1, 2, r_thumb2.rotation, R_Thumb1);
        R_Thumb3 = R_Thumb2.childs[0] = new AvatarTree(r_thumb3, 1, 3, r_thumb3.rotation, R_Thumb2);
        R_Thumb4 = R_Thumb3.childs[0] = new AvatarTree(r_thumb4, 0, 4, r_thumb4.rotation, R_Thumb3);
        R_Index2 = R_Index1.childs[0] = new AvatarTree(r_index2, 1, 6, r_index2.rotation, R_Index1);
        R_Index3 = R_Index2.childs[0] = new AvatarTree(r_index3, 1, 7, r_index3.rotation, R_Index2);
        R_Index4 = R_Index3.childs[0] = new AvatarTree(r_index4, 0, 8, r_index4.rotation, R_Index3);
        R_Middle2 = R_Middle1.childs[0] = new AvatarTree(r_middle2, 1, 10, r_middle2.rotation, R_Middle1);
        R_Middle3 = R_Middle2.childs[0] = new AvatarTree(r_middle3, 1, 11, r_middle3.rotation, R_Middle2);
        R_Middle4 = R_Middle3.childs[0] = new AvatarTree(r_middle4, 0, 12, r_middle4.rotation, R_Middle3);
        R_Ring2 = R_Ring1.childs[0] = new AvatarTree(r_ring2, 1, 14, r_ring2.rotation, R_Ring1);
        R_Ring3 = R_Ring2.childs[0] = new AvatarTree(r_ring3, 1, 15, r_ring3.rotation, R_Ring2);
        R_Ring4 = R_Ring3.childs[0] = new AvatarTree(r_ring4, 0, 16, r_ring4.rotation, R_Ring3);
        R_Pinky2 = R_Pinky1.childs[0] = new AvatarTree(r_pinky2, 1, 18, r_pinky2.rotation, R_Pinky1);
        R_Pinky3 = R_Pinky2.childs[0] = new AvatarTree(r_pinky3, 1, 19, r_pinky3.rotation, R_Pinky2);
        R_Pinky4 = R_Pinky3.childs[0] = new AvatarTree(r_pinky4, 0, 20, r_pinky4.rotation, R_Pinky3);
    }
    void Update()
    {
        lerp += Time.deltaTime;
        if (lerp >= 1.0f)
        {
            lerp = 0;
        }
        if (data != "" && data != ":")
        {
            if (data.Contains("Left") && !data.Contains("Right"))
            {
                string l_data = data.Remove(data.Length - 6);
                l_datas = Array.ConvertAll(l_data.Split(','), float.Parse);
            }//只有左手数据
            else if (!data.Contains("Left") && data.Contains("Right"))
            {
                string r_data = data.Remove(data.Length - 7);
                r_datas = Array.ConvertAll(r_data.Split(','), float.Parse);
            }//只有右手数据
            else
            {
                string l_data = data.Substring(0, data.IndexOf("Left") - 1);
                string r_data = data.Substring(data.IndexOf("Left") + 5, data.IndexOf("Right") - data.IndexOf("Left") - 6);
                l_datas = Array.ConvertAll(l_data.Split(','), float.Parse);
                r_datas = Array.ConvertAll(r_data.Split(','), float.Parse);
            }//有双手数据
            if (data.Contains("Left"))
            {
                UpdateTree(L_Wrist, lerp, true);
            }//更新左手
            if (data.Contains("Right"))
            {
                UpdateTree(R_Wrist, lerp, false);
            }//更新右手
        }
    }
    private void UpdateTree(AvatarTree tree, float lerp, bool isLeft)
    {
        if (tree.parent != null)
        {
            UpdateBone(tree, lerp, isLeft);
        }
        if (tree.childs != null)
        {
            for (int i = 0; i < tree.childs.Length; i++)
            {
                UpdateTree(tree.childs[i], lerp, isLeft);
            }
        }
    }//遍历关节
    private void UpdateBone(AvatarTree tree, float lerp, bool isLeft)
    {
        Vector3 dir1 = tree.GetDir();
        Vector3 dir2;
        if (isLeft)
        {
            var child_dir = new Vector3(1 - l_datas[tree.idx * 3], l_datas[tree.idx * 3 + 1], l_datas[tree.idx * 3 + 2]);
            var parent_dir = new Vector3(1 - l_datas[tree.parent.idx * 3], l_datas[tree.parent.idx * 3 + 1], l_datas[tree.parent.idx * 3 + 2]);
            dir2 = parent_dir - child_dir;
        }
        else
        {
            var child_dir = new Vector3(1 - r_datas[tree.idx * 3], r_datas[tree.idx * 3 + 1], r_datas[tree.idx * 3 + 2]);
            var parent_dir = new Vector3(1 - r_datas[tree.parent.idx * 3], r_datas[tree.parent.idx * 3 + 1], r_datas[tree.parent.idx * 3 + 2]);
            dir2 = parent_dir - child_dir;
        }
        dir2.z = -dir2.z;
        Quaternion rot = Quaternion.FromToRotation(dir1, dir2);
        Quaternion rot1 = tree.parent.transf.rotation;
        tree.parent.transf.rotation = Quaternion.Lerp(rot1, rot * rot1, lerp);
    }//计算关节旋转
}