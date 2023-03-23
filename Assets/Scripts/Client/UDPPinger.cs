using server;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;
using TMPro;
using System;

/// <summary>
/// A class used by the CLIENT to broadcast a server request and wait for answers
/// </summary>
public class UDPPinger
{
    readonly int UDPPort;

    readonly string pingMessage = "ACK";

    readonly UdpClient _udpClient = new UdpClient();

    public System.Action<string> OnMessageReceived;

    public UDPPinger(int port)
    {
        UDPPort = port;
        _udpClient.EnableBroadcast = true;
    }

    public void Update()
    {
        if (_udpClient.Available > 0)
        {
            var remoteEP = new IPEndPoint(IPAddress.Any, 0);
            var data = _udpClient.Receive(ref remoteEP);
            var message = Encoding.UTF8.GetString(data);
            OnMessageReceived?.Invoke(message);
        }
    }

    public void Ping()
    {
        var data = Encoding.UTF8.GetBytes(pingMessage);
        _udpClient.Send(data, data.Length, "255.255.255.255", UDPPort);
    }
}
