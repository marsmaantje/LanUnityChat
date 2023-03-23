using System;
using System.Net.Sockets;
using System.Net;
using shared;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace server {

	/**
	 * Basic TCPGameServer that runs our game.
	 * 
	 * Server is made up out of different rooms that can hold different members.
	 * Each member is identified by a TcpMessageChannel, which can also be used for communication.
	 * In this setup each client is only member of ONE room, but you could change that of course.
	 * 
	 * Each room is responsible for cleaning up faulty clients (since it might involve gameplay, status changes etc).
	 * 
	 * As you can see this setup is limited/lacking:
	 * - only 1 game can be played at a time
	 */
	class TCPGameServer : MonoBehaviour
	{
		[SerializeField] private int serverPort = 55555;	//the port we listen on

		//we have 3 different rooms at the moment (aka simple but limited)
		TcpListener listener;


        private readonly LoginRoom _loginRoom;	//this is the room every new user joins
		private readonly LobbyRoom _lobbyRoom;	//this is the room a user moves to after a successful 'login'
		private readonly List<GameRoom> _gameRooms;		//this is the room a user moves to when a game is succesfully started
		
		//stores additional info for a player
		private readonly Dictionary<TcpMessageChannel, PlayerInfo> _playerInfo = new Dictionary<TcpMessageChannel, PlayerInfo>();

        private void Start()
        {
            Log.LogInfo("Starting server on port " + serverPort, this, ConsoleColor.Gray);

            //start listening for incoming connections (with max 50 in the queue)
            //we allow for a lot of incoming connections, so we can handle them
            //and tell them whether we will accept them or not instead of bluntly declining them
            listener = new TcpListener(IPAddress.Any, serverPort);
            listener.Start(50);
        }

        private TCPGameServer()
		{
			//we have only one instance of each room, this is especially limiting for the game room (since this means you can only have one game at a time).
			_loginRoom = new LoginRoom(this);
			_lobbyRoom = new LobbyRoom(this);
			_gameRooms = new List<GameRoom>();
		}

        private void Update()
        {
            //check for new members	
            if (listener.Pending())
            {
                //get the waiting client
                Log.LogInfo("Accepting new client...", this, ConsoleColor.White);
                TcpClient client = listener.AcceptTcpClient();
                //and wrap the client in an easier to use communication channel
                TcpMessageChannel channel = new TcpMessageChannel(client);
                //and add it to the login room for further 'processing'
                _loginRoom.AddMember(channel);
            }

            //now update every single room
            _loginRoom.Update();
            _lobbyRoom.Update();
            foreach (GameRoom gameRoom in _gameRooms)
            {
                gameRoom.Update();
            }
        }
		
		//provide access to the different rooms on the server 
		public LoginRoom GetLoginRoom() { return _loginRoom; }
		public LobbyRoom GetLobbyRoom() { return _lobbyRoom; }
		public GameRoom GetGameRoom()
		{
			GameRoom newRoom = new GameRoom(this);
            _gameRooms.Add(newRoom);
            return newRoom;
		}

		/**
		 * Returns a handle to the player info for the given client 
		 * (will create new player info if there was no info for the given client yet)
		 */
		public PlayerInfo GetPlayerInfo (TcpMessageChannel pClient)
		{
			if (!_playerInfo.ContainsKey(pClient))
			{
				_playerInfo[pClient] = new PlayerInfo();
			}

			return _playerInfo[pClient];
		}

		/**
		 * Returns a list of all players that match the predicate, e.g. to get a list of 
		 * all players named bob, you would do:
		 *	GetPlayerInfo((playerInfo) => playerInfo.name == "bob");
		 */
		public List<PlayerInfo> GetPlayerInfo(Predicate<PlayerInfo> pPredicate)
		{
			return _playerInfo.Values.ToList<PlayerInfo>().FindAll(pPredicate);
		}

		/**
		 * Should be called by a room when a member is closed and removed.
		 */
		public void RemovePlayerInfo (TcpMessageChannel pClient)
		{
			_playerInfo.Remove(pClient);
		}

		/// <summary>
		/// Method to get the IP address of the server
		/// </summary>
		/// <returns>The IP of the server</returns>
		public string GetServerAddress()
		{
			//get the local IP address
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
			{
                if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

		/// <summary>
		/// Method to get the port the server is running on
		/// </summary>
		/// <returns>The port of the server</returns>
		public int GetServerPort()
		{
			return serverPort;
		}

	}

}


