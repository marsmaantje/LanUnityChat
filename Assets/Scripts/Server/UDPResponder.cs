using server;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class UDPResponder : MonoBehaviour
{
    [SerializeField]
    TCPGameServer _server;
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
        if(_udpClient.Available > 0)
        {
            var fromEndpoint = new IPEndPoint(0, 0);
            var recieveBuffer = _udpClient.Receive(ref fromEndpoint);
            Debug.Log((recieveBuffer));
        }
    }
}
