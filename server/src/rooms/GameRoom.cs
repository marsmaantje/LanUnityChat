using shared;
using System;

namespace server
{
	/**
	 * This room runs a single Game (at a time). 
	 * 
	 * The 'Game' is very simple at the moment:
	 *	- all client moves are broadcasted to all clients
	 *	
	 * The game has no end yet (that is up to you), in other words:
	 * all players that are added to this room, stay in here indefinitely.
	 */
	class GameRoom : Room
	{
		public bool IsGameInPlay { get; private set; }

		//wraps the board to play on...
		private TicTacToeBoard _board = new TicTacToeBoard();
		TcpMessageChannel player1;
		TcpMessageChannel player2;

		public GameRoom(TCPGameServer pOwner) : base(pOwner)
		{
		}

		public void StartGame (TcpMessageChannel pPlayer1, TcpMessageChannel pPlayer2)
		{
			if (IsGameInPlay) throw new Exception("Programmer error duuuude.");

			IsGameInPlay = true;
			addMember(pPlayer1);
			addMember(pPlayer2);
			this.player1 = pPlayer1;
			this.player2 = pPlayer2;

			StartGame startGame = new StartGame();
			startGame.player1Name = _server.GetPlayerInfo(pPlayer1).Name;
			startGame.player2Name = _server.GetPlayerInfo(pPlayer2).Name;
			startGame.movingPlayer = 0;

			sendToAll(startGame);
		}

		protected override void addMember(TcpMessageChannel pMember)
		{
			base.addMember(pMember);

			//notify client he has joined a game room 
			RoomJoinedEvent roomJoinedEvent = new RoomJoinedEvent();
			roomJoinedEvent.room = RoomJoinedEvent.Room.GAME_ROOM;
			pMember.SendMessage(roomJoinedEvent);
		}

		public override void Update()
		{
			//demo of how we can tell people have left the game...
			int oldMemberCount = memberCount;
			base.Update();
			int newMemberCount = memberCount;

			if (oldMemberCount != newMemberCount)
			{
				Log.LogInfo("People left the game...", this);
			}
		}

		protected override void handleNetworkMessage(ASerializable pMessage, TcpMessageChannel pSender)
		{
			switch(pMessage)
			{
				case MakeMoveRequest moveRequest:
					handleMakeMoveRequest(moveRequest, pSender);
					break;
			}
		}

		private void handleMakeMoveRequest(MakeMoveRequest pMessage, TcpMessageChannel pSender)
		{
			//we have two players, so index of sender is 0 or 1, which means playerID becomes 1 or 2
			int playerID = indexOfMember(pSender) + 1;
			//make the requested move (0-8) on the board for the player
			_board.MakeMove(pMessage.move, playerID);

			//and send the result of the boardstate back to all clients
			MakeMoveResult makeMoveResult = new MakeMoveResult();
			makeMoveResult.whoMadeTheMove = playerID;
			makeMoveResult.boardData = _board.GetBoardData();
			sendToAll(makeMoveResult);

			//check whether one of the players won the game
			int winIndex = _board.GetBoardData().WhoHasWon();
			if(winIndex > 0)
			{
				GameEnd(winIndex - 1);
			}
		}

		private void GameEnd(int winIndex)
		{
			GameWin gameWin = new GameWin();
			gameWin.winningPlayerIndex = winIndex - 1;
			sendToAll(gameWin);

			string p1Name = _server.GetPlayerInfo(player1).Name;
			string p2Name = _server.GetPlayerInfo(player2).Name;


			safeForEach(_server.GetLobbyRoom().AddMember);
			safeForEach(removeMember);
			_server.GetLobbyRoom().broadcastServerMessage($"Game between {p1Name} and {p2Name} has ended.\n" +
                $"Player {(winIndex == 0 ? p1Name : p2Name)} has won.");
		}

	}
}
