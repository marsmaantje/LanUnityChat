using shared;
using System.Collections.Generic;
using UnityEngine;

/**
 * Starting state where you can connect to the server.
 */
public class ServerPickState : ApplicationStateWithView<ServerPickView>
{
    [SerializeField]    private int _broadcastPort = 0;

    private UDPPinger _udpPinger;


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
        _udpPinger.Ping();
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

    private void OnUDPMessageReceived(string message)
    {
        if (message.StartsWith("ACK"))
        {
            string[] parts = message.Split(':');
            if (parts.Length == 3)
            {
                string address = parts[1];
                int port = int.Parse(parts[2]);
                view.AddServer(address, port);
            }
        }
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