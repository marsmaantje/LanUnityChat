using shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Starting state where you can connect to the server.
 */
public class ServerPickState : ApplicationStateWithView<ServerPickView>
{
    [SerializeField]    private int _broadcastPort = 0;
    [SerializeField] private float _broadcastInterval = 0.5f;
    [SerializeField] private float _broadcastTime = 5.0f;
    private float _broadcastStartTime;
    public bool IsSearching => Time.time > _broadcastStartTime + _broadcastTime;

    private UDPPinger _udpPinger;

    private Dictionary<string, ServerAddress> _servers = new Dictionary<string, ServerAddress>();

    public override void EnterState()
    {
        base.EnterState();

        //listen to a connect click from our view
        view.OnServerConnectRequest += TryConnect;

    }

    public override void ExitState ()
    {
        base.ExitState();

        //stop listening to connect requests
        view.OnServerConnectRequest -= TryConnect;
    }

    /**
     * Connect to the server (with some client side validation)
     */
    private void TryConnect(string address, int port)
    {
        fsm.channel.Connect(address, port);
    }

    private void Start()
    {
        _udpPinger = new UDPPinger(_broadcastPort);
        _udpPinger.OnMessageReceived += OnUDPMessageReceived;
        StartCoroutine(PingCoroutine());
    }

    /// <summary>
    /// Start a search for potential servers in our area
    /// </summary>
    public void Search()
    {
        StartCoroutine(PingCoroutine());
    }

    /// <summary>
    /// Do a search for potential servers in our area, it will ping at an interval for a predetermined amount of seconds
    /// </summary>
    private IEnumerator PingCoroutine()
    {
        _broadcastStartTime = Time.time;
        while (Time.time - _broadcastStartTime < _broadcastTime)
        {
            _udpPinger.Ping();
            yield return new WaitForSeconds(_broadcastInterval);
        }
    }

    /// //////////////////////////////////////////////////////////////////
    ///                     NETWORK MESSAGE PROCESSING
    /// //////////////////////////////////////////////////////////////////

    private void Update()
    {
        //if we are connected, start processing messages
        if (fsm.channel.Connected) receiveAndProcessNetworkMessages();

        _udpPinger.Update();
    }

    /// <summary>
    /// If we received a response from a server, check if the response is valid and not already in the list, if so, add it to the list of potential servers.
    /// </summary>
    /// <param name="message"></param>
    private void OnUDPMessageReceived(string message)
    {
        if (message.StartsWith("ACK"))
        {
            string[] parts = message.Split(':');
            if (parts.Length == 3)
            {
                string address = parts[1];
                int port = int.Parse(parts[2]);

                if (!_servers.ContainsKey(address))
                    _servers.Add(address, view.AddServer(address, port));
            }
        }
        Debug.Log("wow, we received something back: " + message);
    }

    protected override void handleNetworkMessage(ASerializable pMessage)
    {
        Debug.Log("how did we get here?");
        switch(pMessage)
        {
            case RoomJoinedEvent roomJoinedEvent:
                handleServerConnectEvent(roomJoinedEvent);
                break;
        }
    }

    private void handleServerConnectEvent (RoomJoinedEvent pMessage)
    {
        if (pMessage.room == RoomJoinedEvent.Room.LOGIN_ROOM)
        {
            fsm.ChangeState<LoginState>();
        } 
    }

}