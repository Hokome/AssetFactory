using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;

namespace AssetFactory.Networking
{
	public class Server : IPacketHandler
	{
		public static Server Inst
		{
#if !UNITY_SERVER
			get
			{
				Debug.LogError("Attempt to acces server instance when the application is a client");
				return null;
			}
			set
			{
				Debug.LogError("Attempt to set server instance when the application is a client");
			}
#else
			get; set;
#endif
		}
		public static NetworkSettings Settings
		{
#if !UNITY_SERVER
			get
			{
				Debug.LogWarning("Attempt to access server network settings when application is a client");
				return Networking.Client.Settings;
			}
			set
			{
				Debug.LogWarning("Attempt to access server network settings when application is a client");
				Networking.Client.Settings = value;
			}
#else
			get; set;
#endif
		}

		public List<IPacketHandler> Callbacks { get; private set; }
		Dictionary<byte, List<Action<Packet>>> packetHandles;

		public List<Action> callbackBuffer;


		TcpListener tcp;
		public Client[] clients;

#region Server TCP
		public static Server CreateServer(NetworkSettings settings)
		{
			if (Inst == null)
				Inst = new Server(settings);
			return Inst;
		}
		private Server(NetworkSettings settings)
		{
			Server.Settings = settings;

			tcp = TcpListener.Create(settings.port);
			tcp.Start();

			Room.Rooms = new Dictionary<string, Room>(settings.maxPlayersPerRoom);
			clients = new Client[settings.maxPlayerConnections];

			Console.WriteLine($"Server version {settings.Version} is running on port {settings.port}");
			tcp.BeginAcceptTcpClient(ConnectCallback, null);

			Callbacks = new List<IPacketHandler>
			{
				this
			};
			callbackBuffer = new List<Action>();
			packetHandles = new Dictionary<byte, List<Action<Packet>>>(255);
		}

		void ConnectCallback(IAsyncResult result)
		{
			TcpClient tcpClient = tcp.EndAcceptTcpClient(result);
			Client client;

			tcp.BeginAcceptTcpClient(ConnectCallback, null);
			Console.WriteLine($"Incoming connection from '{tcpClient.Client.RemoteEndPoint}'");

			for (int i = 0; i < clients.Length; i++)
			{
				if (clients[i] == null)
				{
					clients[i] = client = new Client(i, tcpClient);
					goto Found;
				}
			}

			Console.WriteLine($"Server is full, disconnecting client");
			tcpClient.Close();
			return;

			Found:
			callbackBuffer.Add(() => SendWelcome(client));
		}
#endregion

		public void AddHandle(byte code, Action<Packet> handle)
		{
			if (!packetHandles.ContainsKey(code))
				packetHandles.Add(code, new List<Action<Packet>>());
			packetHandles[code].Add(handle);
		}
		public void RemoveHandle(byte code, Action<Packet> handle)
		{
			packetHandles[code].Remove(handle);
		}
		public class Client
		{
			public int id;
			public string nickname;
			public TcpClient tcp;

			public bool InRoom { get => room != null; }
			public Room room;

			NetworkStream stream;
			byte[] receiveBuffer;

			#region Client TCP
			public Client(int id, TcpClient tcp)
			{
#if !UNITY_SERVER
				Debug.LogError("Attempt to send packet to client when application is a client");
#else
				this.id = id;
				this.tcp = tcp;

				tcp.SendBufferSize = Settings.dataBufferSize;
				tcp.ReceiveBufferSize = Settings.dataBufferSize;

				stream = tcp.GetStream();

				receiveBuffer = new byte[Settings.dataBufferSize];

				stream.BeginRead(receiveBuffer, 0, Settings.dataBufferSize, ReceiveCallback, null);
#endif
			}
			void ReceiveCallback(IAsyncResult result)
			{
				try
				{
					if (tcp == null)
						return;

					int length = stream.EndRead(result);
					if (length <= 0)
					{
						Disconnect();
						return;
					}

					byte[] data = new byte[length];
					Array.Copy(receiveBuffer, data, length);
					Packet packet = new Packet(data)
					{
						sender = id
					};

					NetworkManager.Inst.AddCallbacks(Inst.Callbacks, packet);
					if (Inst.packetHandles.ContainsKey(packet.Code))
					{
						List<Action<Packet>> validHandles = new List<Action<Packet>>(Inst.packetHandles[packet.Code]);
						NetworkManager.Inst.AddCallbacks(validHandles, packet);
					}

					stream.BeginRead(receiveBuffer, 0, Settings.dataBufferSize, ReceiveCallback, null);
				}
				catch (Exception e)
				{
					Debug.LogError($"{e}");
					Disconnect();
				}
			}
			public void Send(Packet packet)
			{
#if !UNITY_SERVER
				Debug.LogError("Attempt to send packet to client when application is a client");
#else
				try
				{
					if (tcp == null)
						return;

					stream.BeginWrite(packet.Data, 0, packet.TrueLength, null, null);
				}
				catch (Exception e)
				{
					Debug.LogError(e);
				}
#endif
			}
			public void Disconnect()
			{
				Console.WriteLine($"Disconnecting client {tcp.Client.RemoteEndPoint}");

				tcp.Close();
				stream = null;
				receiveBuffer = null;
				tcp = null;
				Inst.clients[id] = null;
				if (InRoom)
					room.RemovePlayer(this);
			}
#endregion
		}

#region Packets
		public void OnNetworkMessage(Packet packet)
		{
			switch (packet.Code)
			{
				case Packet.ClientCodes.WELCOME_CONFIRM:
					ClientWelcomeConfirm(packet);
					break;
				case Packet.ClientCodes.REQUEST_CREATE_ROOM:
					TryCreateRoom(packet);
					break;
				case Packet.ClientCodes.REQUEST_JOIN_ROOM:
					TryJoinRoom(packet);
					break;
			}
		}
		void SendWelcome(Client client)
		{
			Packet.Writer writer = new Packet.Writer(Packet.ServerCodes.WELCOME);

			writer.Write(client.id);
			writer.Write("Welcome to the server!");

			client.Send(writer.EndWrite());
		}
		void ClientWelcomeConfirm(Packet packet)
		{
			Packet.Reader reader = new Packet.Reader(packet);

			int id = reader.ReadInt();
			if (id != packet.sender)
			{
				Debug.LogError($"Client [{packet.sender}] assumed the wrong id");
				clients[packet.sender].Disconnect();
				return;
			}
			string nickname = reader.ReadString();
			clients[id].nickname = nickname;
			Console.WriteLine($"Client [{id}] connected with nickname: {nickname}");
		}
		void TryCreateRoom(Packet packet)
		{
			Client c = clients[packet.sender];

			RoomOperationResult result = Room.TryCreateRoom(c);

			if (result != RoomOperationResult.CreationSucces)
			{
				Packet.Writer writer = new Packet.Writer(Packet.ServerCodes.ROOM_OPERATION_RESULT);
				writer.Write((byte)result);
				c.Send(writer.EndWrite());
			}
		}
		void TryJoinRoom(Packet packet)
		{
			Client c = clients[packet.sender];
			Packet.Reader reader = new Packet.Reader(packet);

			RoomOperationResult result = Room.TryJoinRoom(c, reader.ReadString());

			if (result != RoomOperationResult.JoinSuccess)
			{
				Packet.Writer writer = new Packet.Writer(Packet.ServerCodes.ROOM_OPERATION_RESULT);
				writer.Write((byte)result);
				c.Send(writer.EndWrite());
			}
		}
#endregion

	}
}