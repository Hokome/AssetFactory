using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
	public enum RoomUpdate
	{
		Status,
		Kick,
		Closing
	}

	public class Room
	{
		public static Room Current { get; set; }
		public static Dictionary<string, Room> Rooms { get => Server.Inst.Rooms; }
		public static List<Room> RoomList { get => Rooms.Values.ToList(); }
		
		public string ID { get; protected set; }
		public Dictionary<int, Player> Players { get; protected set; }
		public List<Player> PlayerList { get => Players.Values.ToList(); }
		public RoomManager manager;

		public class Player
		{
			public int Number { get; }
			public string Nickname { get => Client.Nickname; }
			public Client Client { get; }
			public bool IsHost { get => Number == 1; }
			public bool IsLocal { get => Client.IsLocal; }
			public bool ready = false;

			public Player(int number, Client client, Room room)
			{
				Number = number;
				Client = client;
				client.player = this;
				client.room = room;
			}
			public override string ToString() => $"Player {Number}";
		}
		private Room(string id, int maxPlayers)
		{
			ID = id;
			Players = new Dictionary<int, Player>(maxPlayers);
		}

		public static RoomOperationResult TryCreateRoom(ServerClient client)
		{
			try
			{
				if (client.InRoom)
				{
					return RoomOperationResult.AlreadyInRoom;
				}
				else if (Rooms.Values.Count >= NetworkSettings.Settings.maxRooms)
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

					Room room = new Room(id, NetworkSettings.Settings.maxPlayers);
					Rooms.Add(room.ID, room);

					Console.WriteLine($"Created {room} for {client}");

					Packet.Writer writer = new Packet.Writer(Packet.ServerCodes.ROOM_OPERATION_RESULT);
					writer.Write((byte)RoomOperationResult.CreationSucces);

					Player p = new Player(1, client, room);
					room.Players.Add(1, p);

					room.Pack(writer);
					client.Send(writer.Finish());

					return RoomOperationResult.CreationSucces;
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				return RoomOperationResult.Exception;
			}
		}
		public static RoomOperationResult TryJoinRoom(ServerClient client, string roomId)
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
				else if (Rooms[roomId].Players.Count >= NetworkSettings.Settings.maxPlayers)
				{
					return RoomOperationResult.RoomIsFull;
				}
				else
				{
					Room room = Rooms[roomId];
					Player p;
					for (int i = 1; i <= NetworkSettings.Settings.maxPlayers; i++)
					{
						if (room.Players.ContainsKey(i))
							continue;
						p = new Player(i, client, room);
						room.Players.Add(i, p);
						goto Found;
					}
					return RoomOperationResult.RoomIsFull;
				Found:
					Packet.Writer writer = new Packet.Writer(Packet.ServerCodes.ROOM_OPERATION_RESULT);
					writer.Write((byte)RoomOperationResult.JoinSuccess);
					room.Pack(writer);
					client.Send(writer.Finish());

					writer = new Packet.Writer(Packet.ServerCodes.PLAYER_JOINED);
					PackPlayer(p, writer);
					room.SendToAllPlayers(writer.Finish(), p.Number);

					return RoomOperationResult.JoinSuccess;
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				return RoomOperationResult.Exception;
			}
		}
		public void AddPlayer(Packet.Reader reader)
		{
			Player p = UnpackPlayer(reader);
			Players.Add(p.Number, p);
		}
		public void RemovePlayerLocal(Player p)
		{
			Players.Remove(p.Number);
		}
		public void RemovePlayer(ServerClient client)
		{
			if (Players.ContainsKey(client.player.Number))
			{
				if (client != Players[client.player.Number].Client)
					goto Error;
				client.room = null;
				Console.WriteLine($"Removing {client.player} from {this}");
				Players.Remove(client.player.Number);
				if (client.player.IsHost)
					Close();
				else
				{
					Console.WriteLine("left");
					Packet.Writer writer = new Packet.Writer(Packet.ServerCodes.PLAYER_LEFT);
					PackPlayer(client.player, writer);
					SendToAllPlayers(writer.Finish());
				}
				client.player = null;
				return;
			}
		Error:
			Debug.LogError($"{client} is not in {this}");
		}
		public void Close()
		{
			Console.WriteLine($"Closing {this}");
			if (Players.Count > 0)
			{
				Packet.Writer writer = new Packet.Writer(Packet.ServerCodes.ROOM_UPDATE);
				writer.Write((byte)RoomUpdate.Closing);
				SendToAllPlayers(writer.Finish());
				foreach (Player p in Players.Values)
				{
					p.Client.room = null;
					p.Client.player = null;
				}
			}

			Rooms.Remove(ID);
			
			if (manager != null)
				NetworkManager.CallbackBuffer.Add(() => UnityEngine.Object.Destroy(manager.gameObject));
		}
		public void Start()
		{
			if (Players.Count < NetworkSettings.Settings.minPlayers)
				return;
			if (!Players.Values.All(p => p.ready))
				return;
			Packet.Writer wr = new Packet.Writer(Packet.ServerCodes.START_GAME);
			SendToAllPlayers(wr.Finish());
			SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive).completed += delegate
			{
				Scene sc = SceneManager.GetSceneAt(1);
				sc.GetRootGameObjects()[0].GetComponent<RoomManager>().Init(this);
				SceneManager.MergeScenes(sc, SceneManager.GetSceneAt(0));
			};
		}

#region Packing
		public void Pack(Packet.Writer writer)
		{
			writer.Write(ID);
			writer.Write(Players.Count);
			foreach (Player p in Players.Values)
			{
				PackPlayer(p, writer);
			}
		}
		public static void PackPlayer(Player p, Packet.Writer writer)
		{
			writer.Write(p.Client.ID);
			writer.Write(p.Number);
			writer.Write(p.Nickname);
		}
		public static Room Unpack(Packet.Reader reader)
		{
			Room room = new Room(reader.ReadString(), NetworkSettings.Settings.maxPlayers);
			int count = reader.ReadInt();
			for (int i = 0; i < count; i++)
			{
				int id = reader.ReadInt();
				Player p;
				if (id == LocalClient.Inst.ID)
				{
					p = new Player(reader.ReadInt(), LocalClient.Inst, room);
					reader.ReadString();
				}
				else
				{
					Client c = new Client(id);
					p = new Player(reader.ReadInt(), c, room);
					c.room = room;
					c.Nickname = reader.ReadString();
				}
				room.Players.Add(p.Number, p);
			}
			return room;
		}
		public Player UnpackPlayer(Packet.Reader reader)
		{
			int id = reader.ReadInt();
			int num = reader.ReadInt();
			string name = reader.ReadString();
			if (Players.ContainsKey(num))
				return Players[num];
			else
			{
				Client c = new Client(id);
				Player p = new Player(num, c, this);
				c.Nickname = name;
				return p;
			}
		}
#endregion

		public void SendToAllPlayers(Packet packet)
		{
			foreach (Player pl in Players.Values)
			{
				(pl.Client as NetworkClient).Send(packet);
			}
		}
		public void SendToAllPlayers(Packet packet, int except)
		{
			foreach (Player pl in Players.Values)
			{
				if (except == pl.Number)
					continue;
				(pl.Client as NetworkClient).Send(packet);
			}
		}

		static System.Random random = new System.Random();
		public const int CODE_LENGTH = 6;
		static string GenerateCode()
		{
			const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
			return new string(Enumerable.Repeat(chars, CODE_LENGTH)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}
		public static string RoomOperationMessage(RoomOperationResult result)
		{
			return result switch
			{
				RoomOperationResult.AlreadyInRoom => "Client is already in room",
				RoomOperationResult.RoomDoesNotExist => "The specified room does not exist",
				RoomOperationResult.RoomIsFull => "The specified room is full",
				RoomOperationResult.ServerRoomSlotsFull => "The server room limit is reached",
				RoomOperationResult.Exception => "An error occured",
				RoomOperationResult.CreationSucces => "Succesfully created room",
				RoomOperationResult.JoinSuccess => "Succesfully joined room",
				_ => null
			};
		}
		public override string ToString() => $"room '{ID}'";
	}
}