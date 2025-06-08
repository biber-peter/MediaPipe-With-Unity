using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System;

/// <summary>
/// 数据管理器，收获Python端的数据
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
        //启动Socket收获数据
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
                //处理两端数据给身体和手部
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
    }//接收数据

    void Update()
    {

    }
}
