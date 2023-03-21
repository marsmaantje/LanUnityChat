using shared;
using System;

/**
 * This is where we 'play' a game.
 */
public class GameState : ApplicationStateWithView<GameView>
{
    //just for fun we keep track of how many times a player clicked the board
    //note that in the current application you have no idea whether you are player 1 or 2
    //normally it would be better to maintain this sort of info on the server if it is actually important information
    private string player1Name = "player1";
    private int player1MoveCount = 0;
    private string player2Name = "player2";
    private int player2MoveCount = 0;

    public override void EnterState()
    {
        base.EnterState();
        
        view.gameBoard.OnCellClicked += _onCellClicked;
    }

    private void _onCellClicked(int pCellIndex)
    {
        MakeMoveRequest makeMoveRequest = new MakeMoveRequest();
        makeMoveRequest.move = pCellIndex;

        fsm.channel.SendMessage(makeMoveRequest);
    }

    public override void ExitState()
    {
        base.ExitState();
        view.gameBoard.OnCellClicked -= _onCellClicked;
    }

    private void Update()
    {
        receiveAndProcessNetworkMessages();
    }

    protected override void handleNetworkMessage(ASerializable pMessage)
    {
        switch(pMessage)
        {
            case MakeMoveResult moveResult:
                handleMakeMoveResult(moveResult);
                break;

            case StartGame startGame:
                handleStartGame(startGame);
                break;

            case GameWin gameWin:
                handleGameWin(gameWin);
                break;

            case RoomJoinedEvent joinEvent:
                handleRoomJoinedEvent(joinEvent);
                break;

            default:
                break;
        }
    }


    private void handleMakeMoveResult(MakeMoveResult pMakeMoveResult)
    {
        view.gameBoard.SetBoardData(pMakeMoveResult.boardData);

        //some label display
        if (pMakeMoveResult.whoMadeTheMove == 1)
        {
            player1MoveCount++;
            view.playerLabel1.text = $"{player1Name} (Movecount: {player1MoveCount})";
        }
        if (pMakeMoveResult.whoMadeTheMove == 2)
        {
            player2MoveCount++;
            view.playerLabel2.text = $"{player2Name} (Movecount: {player2MoveCount})";
        }

    }

    private void handleStartGame(StartGame startGame)
    {
        view.playerLabel1.SetText(startGame.player1Name);
        player1Name = startGame.player1Name;
        view.playerLabel2.SetText(startGame.player2Name);
        player2Name = startGame.player2Name;
    }
    private void handleGameWin(GameWin gameWin)
    {

    }

    private void handleRoomJoinedEvent(RoomJoinedEvent pMessage)
    {
        //did we move to the game room?
        if (pMessage.room == RoomJoinedEvent.Room.LOBBY_ROOM)
        {
            fsm.ChangeState<LobbyState>();
        }
    }
}
