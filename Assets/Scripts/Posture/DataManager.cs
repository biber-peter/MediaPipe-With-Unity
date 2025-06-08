using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System;

/// <summary>
/// ���ݹ��������ջ�Python�˵�����
/// </summary>
public class DataManager : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public Body body;
    public Hand hand;
    Thread receiveThread;
    UdpClient client;
    public int port = 5054;
    public string[] data;

    void Start()
    {
        //����Socket�ջ�����
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                //�����������ݸ�������ֲ�
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataByte = client.Receive(ref anyIP);
                data = Encoding.UTF8.GetString(dataByte).Split(';');
                hand.data = data[0];
                body.data = data[1];
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }//��������

    void Update()
    {

    }
}
