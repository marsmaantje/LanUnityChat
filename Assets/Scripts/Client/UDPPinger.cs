using server;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;
using TMPro;

public class UDPPinger : MonoBehaviour
{
    [SerializeField]
    int UDPPort = 9876;

    [SerializeField]
    TMP_InputField sendString;

    UdpClient _udpClient = new UdpClient();


    // Start is called before the first frame update
    void Start()
    {
        _udpClient.EnableBroadcast= true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Send()
    {
        var data = Encoding.UTF8.GetBytes(sendString.text);
        _udpClient.Send(data, data.Length, "255.255.255.255", UDPPort);
    }
}
