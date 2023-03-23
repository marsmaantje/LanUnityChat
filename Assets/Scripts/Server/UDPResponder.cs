using server;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

/// <summary>
/// A class used by the SERVER to respond to UDP pings, it returns ACK and then the server IP and port the CLIENT can connect to via TCP
/// </summary>
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
        //Bind ourselves to the port so we can listen to broadcast pings
        _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, UDPPort));
    }

    // Update is called once per frame
    void Update()
    {
        //if we received a ping, process this
        if(_udpClient.Available > 0)
        {
            var fromEndpoint = new IPEndPoint(0, 0);
            var recieveBuffer = _udpClient.Receive(ref fromEndpoint);
            var data = Encoding.UTF8.GetString(recieveBuffer);
            Debug.Log("Recieved: " + data);

            //send back our server IP and Port
            var response = "ACK:" + _server.GetServerAddress() + ":" + _server.GetServerPort();
            var responseBuffer = Encoding.UTF8.GetBytes(response);
            _udpClient.Send(responseBuffer, responseBuffer.Length, fromEndpoint);
        }
    }
}
