using shared;
using UnityEngine;

/**
 * 'Chat' state while you are waiting to start a game where you can signal that you are ready or not.
 */
public class LobbyState : ApplicationStateWithView<LobbyView>
{
    [Tooltip("Should we enter the lobby in a ready state or not?")]
    [SerializeField] private bool autoQueueForGame = false;
    [SerializeField] private Texture2D testImage = null; //for testing purposes, you can set this in the inspector

    public override void EnterState()
    {
        base.EnterState();

        view.SetLobbyHeading("Welcome to the Lobby...");
        view.ClearOutput();
        view.AddOutput($"Server settings:"+fsm.channel.GetRemoteEndPoint());
        view.SetReadyToggle(autoQueueForGame);

        view.OnChatTextEntered += onTextEntered;
        view.OnReadyToggleClicked += onReadyToggleClicked;
        view.OnTestImageButtonClicked += OnSendImageButtonClicked;

        if (autoQueueForGame)
        {
            onReadyToggleClicked(true);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        
        view.OnChatTextEntered -= onTextEntered;
        view.OnReadyToggleClicked -= onReadyToggleClicked;
        view.OnTestImageButtonClicked -= OnSendImageButtonClicked;
    }

    /**
     * Called when you enter text and press enter.
     */
    private void onTextEntered(string pText)
    {
        view.ClearInput();

        ChatMessage msg = new ChatMessage();
        msg.message = pText;
        fsm.channel.SendMessage(msg);

        //addOutput("(noone else will see this because I broke the chat on purpose):"+pText);        
    }

    /**
     * Called when you click the send image button
     */
    public void OnSendImageButtonClicked()
    {
        //for testing purposes, we will send a test image
        if (testImage != null)
        {
            SendImage(testImage);
        }
        else
        {
            addOutput("No test image found in Resources/test_image.png");
        }
    }

    public void SendImage(Texture2D image)
    {
        if (image != null)
        {
            byte[] bytes = image.EncodeToPNG();
            ImageMessage message = new ImageMessage();
            message.data = bytes;
            fsm.channel.SendMessage(message);
        }
    }

    /**
     * Called when you click on the ready checkbox
     */
    private void onReadyToggleClicked(bool pNewValue)
    {
        ChangeReadyStatusRequest msg = new ChangeReadyStatusRequest();
        msg.ready = pNewValue;
        fsm.channel.SendMessage(msg);
    }

    private void addOutput(string pInfo)
    {
        view.AddOutput(pInfo);
    }

    /// //////////////////////////////////////////////////////////////////
    ///                     NETWORK MESSAGE PROCESSING
    /// //////////////////////////////////////////////////////////////////

    private void Update()
    {
        receiveAndProcessNetworkMessages();
    }
    
    protected override void handleNetworkMessage(ASerializable pMessage)
    {
        Debug.Log("LobbyState received message: " + pMessage.GetType().Name);
        switch (pMessage)
        {
            case ChatMessage msg: handleChatMessage(msg); break;
            case ImageMessage msg: handleImageMessage(msg); break;
            case RoomJoinedEvent msg: handleRoomJoinedEvent(msg); break;
            case LobbyInfoUpdate msg: handleLobbyInfoUpdate(msg); break;
        }
    }

    private void handleChatMessage(ChatMessage pMessage)
    {
        //just show the message
        addOutput(pMessage.message);
    }

    private void handleImageMessage(ImageMessage pMessage)
    {
        //decode the image again
        Texture2D decodedTexture = new Texture2D(2, 2);
        decodedTexture.LoadImage(pMessage.data);
        view.SetImage(decodedTexture);
    }

    private void handleRoomJoinedEvent(RoomJoinedEvent pMessage)
    {
        //did we move to the game room?
        if (pMessage.room == RoomJoinedEvent.Room.GAME_ROOM)
        {
            fsm.ChangeState<GameState>();
        }
    }

    private void handleLobbyInfoUpdate(LobbyInfoUpdate pMessage)
    {
        //update the lobby heading
        view.SetLobbyHeading($"Welcome to the Lobby ({pMessage.memberCount} people, {pMessage.readyCount} ready)");
    }

}
