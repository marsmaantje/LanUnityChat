using server;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;

public class UDPPinger : MonoBehaviour
{
    [SerializeField]
    int UDPPort = 9876;

    UdpClient _udpClient = new UdpClient();


    // Start is called before the first frame update
    void Start()
    {
        _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, UDPPort));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Send()
    {
        var data = Encoding.UTF8.GetBytes("Test123");
        _udpClient.Send(data, data.Length, "255.255.255.255", UDPPort);
    }
}
