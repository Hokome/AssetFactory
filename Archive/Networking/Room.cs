using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AssetFactory.Networking
{
	public enum RoomOperationResult
	{
		CreationSucces,
		JoinSuccess,
		ServerRoomSlotsFull,
		RoomIsFull,
		RoomDoesNotExist,
		AlreadyInRoom,
		Exception
	}

	public class Room
	{
		public static Room Inst
		{
#if UNITY_SERVER
			get
			{
				Debug.LogError("Attempt to acces room instance when the application is a server");
				return null;
			}
			set
			{
				Debug.LogError("Attempt to set room instance when the application is a server");
			}
#else
			get; set;
#endif
		}
		public static Dictionary<string, Room> Rooms
		{
#if UNITY_SERVER
			get; set;
#else
			get
			{
				Debug.LogError("Attempt to access room list when application is client");
				return null;
			}
			set
			{
				Debug.LogError("Attempt to set room list when application is client");
			}
#endif
		}

		public string ID { get; private set; }
		public Dictionary<int, Player> players;
		public Player[] PlayerList { get => players.Values.OrderBy((p) => p.Number).ToArray(); }
		public Player[] CachedList { get; private set; }

		public Player LocalPlayer
		{
#if !UNITY_SERVER
			get
			{
				Debug.LogError("Attempt to access local player when application is server");
				return null;
			}
			private set
			{
				Debug.LogError("Attempt to set local player when application is server");
			}
#else
			get; private set;
#endif
		}

		public bool inGame = false;

		public Room(string id, int maxPlayers)
		{
			ID = id;
			players = new Dictionary<int, Player>(maxPlayers);
		}
		public class Player
		{
			public Server.Client Connection
			{
#if UNITY_SERVER
				get; set;
#else
				get
				{
					Debug.LogError("Attempt to access player's server-side connection when the application is a client");
					return null;
				}
				set
				{
					Debug.LogError("Attempt to set player's server-side connection when the application is a client");
				}
#endif
			}
			public bool IsHost { get => Number == 1; }
			public bool IsLocal
			{
#if UNITY_SERVER
				get
				{
					Debug.LogError("Attempt to access local player when the application is a server");
					return false;
				}
				set
				{
					Debug.LogError("Attempt to set local player when the application is a server");
				}
#else
				get; set;
#endif
			}
			public int Number { get; set; }
			public string Nickname
			{
#if UNITY_SERVER
				get => Connection.nickname;
				set => Connection.nickname = value;
#else
				get; set;
#endif
			}

			public bool isReady = false;

 			public Player(Server.Client client, int number)
			{
				Connection = client;
				Number = number;
			}
			public Player(string nickname, int number)
			{
				Nickname = nickname;
				Number = number;
			}
		}

		public Player AddPlayer(Server.Client client)
		{
#if !UNITY_SERVER
			if (players.Count >= Server.Settings.maxPlayersPerRoom)
			{
				Debug.Log("Room is full");
				return null;
			}
			for (int i = 1; i <= Server.Settings.maxPlayersPerRoom; i++)
			{
				if (!players.ContainsKey(i))
				{
					Player p = new Player(client, i);
					players.Add(i, p);
					client.room = this;
					return p;
				}
			}
			Debug.Log("Room is full");
			return null;
#else
			Debug.LogError("Attempt to add player using server-side client when application is a client");
			return null;
#endif
		}
		public Player SetPlayer(string nickname, int id)
		{
#if !UNITY_SERVER
			if (players.ContainsKey(id))
			{
				if (players[id].Nickname == nickname)
				{
					return players[id];
				}
			}
#else
			Debug.LogError("Attempt to add player using server-side client when application is a client");
			return null;
#endif
		}
		public void RemovePlayer(int id)
		{
			if (!players.ContainsKey(id))
				return;

			Player p = players[id];
#if UNITY_SERVER
			p.Connection.room = null;
			players.Remove(id);
			if (players.Count <= 0)
			{
				//TODO: Close room
			}
			else
			{
				//TODO: Send event
			}
#else
			foreach (MonoBehaviourNetwork mbn in Client.Inst.MonoCallbacks)
			{
				mbn.OnPlayerLeft(p);
			}
			players.Remove(id);
#endif
		}

		public void Pack(Packet.Writer writer, Player destination)
		{
			writer.Write(destination.Number);
			writer.Write(ID);
			writer.Write(players.Count);
			foreach (Player p in players.Values)
			{
				writer.Write(p.Number);
				writer.Write(p.Nickname);
			}
		}
		public static Room UnpackRoom(Packet.Reader reader)
		{
			int thisId = reader.ReadInt();
			Room room = new Room(reader.ReadString(), Client.Settings.maxPlayersPerRoom);
			return room;
		}
		public void UpdateRoom(Packet.Reader reader)
		{
			int id = reader.ReadInt();
			reader.ReadString();
			int count = reader.ReadInt();
			reader.ReadString();
		}

		public static RoomOperationResult TryCreateRoom(Server.Client client)
		{
			try
			{
				if (client.InRoom)
				{
					return RoomOperationResult.AlreadyInRoom;
				}
				else if (Rooms.Values.Count >= Server.Settings.maxRooms)
				{
					return RoomOperationResult.ServerRoomSlotsFull;
				}
				else
				{
					string id;
					do
					{
						id = GenerateCode();
					} while (Rooms.ContainsKey(id));

					Room room = new Room(id, Server.Settings.maxPlayersPerRoom);
					Rooms.Add(room.ID, room);

					Console.WriteLine($"Created room with ID '{room.ID}' for client [{client.id}]");

					Packet.Writer writer = new Packet.Writer(Packet.ServerCodes.ROOM_OPERATION_RESULT);
					writer.Write((byte)RoomOperationResult.CreationSucces);
					room.Pack(writer, room.AddPlayer(client));
					client.Send(writer.EndWrite());

					return RoomOperationResult.CreationSucces;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed");
				Debug.LogError(e);
				return RoomOperationResult.Exception;
			}
		}
		public static RoomOperationResult TryJoinRoom(Server.Client client, string roomId)
		{
			try
			{
				if (client.InRoom)
				{
					return RoomOperationResult.AlreadyInRoom;
				}
				else if (!Rooms.ContainsKey(roomId))
				{
					return RoomOperationResult.RoomDoesNotExist;
				}
				else if (Rooms[roomId].players.Count >= Server.Settings.maxPlayersPerRoom)
				{
					return RoomOperationResult.RoomIsFull;
				}
				else
				{
					Room room = Rooms[roomId];
					Player player = room.AddPlayer(client);

					return RoomOperationResult.JoinSuccess;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed");
				Debug.LogError(e);
				return RoomOperationResult.Exception;
			}
		}

		public void SendToAllPlayers(Packet packet)
		{
#if !UNITY_SERVER
			Debug.LogError($"Cannot call '{nameof(SendToAllPlayers)}' when application is a client");
#else
			foreach (Player p in players)
			{
				p.Connection.Send(packet);
			}
#endif
		}
		public void SendToAllPlayersExcept(Packet packet, int except)
		{
#if !UNITY_SERVER
			Debug.LogError($"Cannot call '{nameof(SendToAllPlayersExcept)}' when application is a client");
#else
			foreach (Player p in players)
			{
				if (p.RoomID == except)
					continue;
				p.Connection.Send(packet);
			}
#endif
		}

#region Util
		private static System.Random random = new System.Random();
		public const int CODE_LENGTH = 6;
		public static string GenerateCode()
		{
			const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
			return new string(Enumerable.Repeat(chars, CODE_LENGTH)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}
#endregion
	}
}