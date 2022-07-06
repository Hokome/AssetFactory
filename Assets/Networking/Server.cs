using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace AssetFactory.Networking
{
	public class Server
	{
		public static Server Inst { get; private set; }
		public static Server CreateServer()
		{
			if (Inst == null)
				Inst = new Server();
			return Inst;
		}

		TcpListener TCP { get; }
		
		public Dictionary<int, ServerClient> Clients { get; }
		public Dictionary<string, Room> Rooms { get; }

		private Server()
		{
#if !UNITY_SERVER
			Debug.LogWarning("Attempt to create a server when the application is a client");
#endif
			TCP = TcpListener.Create(NetworkSettings.Settings.port);
			TCP.Start();

			Console.WriteLine($"Server version {NetworkSettings.Settings.Version} is running on port {NetworkSettings.Settings.port}");

			Clients = new Dictionary<int, ServerClient>();
			Rooms = new Dictionary<string, Room>();

			TCP.BeginAcceptTcpClient(ConnectCallback, null);
		}

		List<Client> clientsCopy = new List<Client>();
		public void CheckClients()
		{
			float t = Time.unscaledTime;
			clientsCopy.Clear();
			lock (Clients.Values)
			{
				clientsCopy.AddRange(Clients.Values);
			}
			foreach (ServerClient c in clientsCopy)
			{
				if (t - c.lastMessage > NetworkSettings.Settings.connectionTimeOut)
				{
					c.Disconnect(DisconnectReason.TimeOut);
				}
			}
		}
		void ConnectCallback(IAsyncResult result)
		{
			TcpClient tcp = TCP.EndAcceptTcpClient(result);

			Console.WriteLine($"Incoming connection from '{tcp.Client.RemoteEndPoint}'");

			if (Clients.Count >= NetworkSettings.Settings.maxServerConnections)
			{
				Console.WriteLine($"Server is full, disconnecting client");
				tcp.Close();
				return;
			}
			int id = GenerateClientID();

			ServerClient client = new ServerClient(id, tcp);
			Clients.Add(id, client);
			TCP.BeginAcceptTcpClient(ConnectCallback, null);
		}

		public void TryCreateRoom(Packet packet)
		{
			ServerClient c = Clients[packet.sender];

			RoomOperationResult result = Room.TryCreateRoom(c);

			if (result != RoomOperationResult.CreationSucces)
			{
				Packet.Writer writer = new Packet.Writer(Packet.ServerCodes.ROOM_OPERATION_RESULT);
				writer.Write((byte)result);
				c.Send(writer.Finish());
			}
		}
		public void TryJoinRoom(Packet packet)
		{
			ServerClient c = Clients[packet.sender];
			Packet.Reader reader = new Packet.Reader(packet);

			RoomOperationResult result = Room.TryJoinRoom(c, reader.ReadString());

			if (result != RoomOperationResult.JoinSuccess)
			{
				Packet.Writer writer = new Packet.Writer(Packet.ServerCodes.ROOM_OPERATION_RESULT);
				writer.Write((byte)result);
				c.Send(writer.Finish());
			}
		}

		const int MAX_CLIENT_ID = 999;
		System.Random random = new System.Random();
		int GenerateClientID()
		{
			int i = random.Next(0, MAX_CLIENT_ID);
			while (Clients.ContainsKey(i))
			{
				if (i >= MAX_CLIENT_ID)
					i = 0;
				else
					i++;
			}
			return i;
		}
		string GenerateRoomID()
		{
			return "000000";
		}
		public void CloseServer()
		{
			foreach (ServerClient sc in Clients.Values)
			{
				try
				{
					sc.Disconnect(DisconnectReason.ServerClosed);
				}
				catch { }
			}
		}
	}
}