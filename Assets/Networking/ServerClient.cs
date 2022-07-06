using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace AssetFactory.Networking
{
	public class ServerClient : NetworkClient
	{
		public float lastMessage;
		public ServerClient(int id, TcpClient tcp)
		{
#if !UNITY_SERVER
			Debug.LogWarning("Attempt to create a server client when the application is a client");
#endif
			ID = id;
			TCP = tcp;

			Callbacks = new List<IReceiveCallback>();
			InitializeConnection();
			NetworkManager.CallbackBuffer.Add(() => SendWelcome());
			NetworkManager.CallbackBuffer.Add(() => lastMessage = Time.unscaledTime);
		}

		public override void Disconnect(DisconnectReason reason)
		{
			if (InRoom)
			{
				room.RemovePlayer(this);
			}
			Console.WriteLine($"{this} disconnected: {DisconnectMessage(reason)}"); 

			switch (reason)
			{
				case DisconnectReason.ServerClosed:
					SendDisconnect(reason);
					break;
			}

			base.Disconnect(reason);
			if (reason != DisconnectReason.ServerClosed)
				Server.Inst.Clients.Remove(ID);
		}

		void SendWelcome()
		{
			Packet.Writer writer = new Packet.Writer(Packet.ServerCodes.WELCOME);

			writer.Write(ID);
			writer.Write("Welcome to the server!");

			Send(writer.Finish());
		}

		public override void OnNetworkMessage(Packet packet)
		{
			NetworkManager.CallbackBuffer.Add(() => lastMessage = Time.unscaledTime);
			switch (packet.Code)
			{
				case Packet.ClientCodes.WELCOME_CONFIRM:
					WelcomeConfirm(packet);
					break;
				case Packet.ClientCodes.DISCONNECT:
					Packet.Reader reader = new Packet.Reader(packet);
					Disconnect((DisconnectReason)reader.ReadByte());
					break;
				case Packet.ClientCodes.REQUEST_CREATE_ROOM:
					Server.Inst.TryCreateRoom(packet);
					break;
				case Packet.ClientCodes.REQUEST_JOIN_ROOM:
					Server.Inst.TryJoinRoom(packet);
					break;
				case Packet.ClientCodes.REQUEST_START_GAME:
					if (player.IsHost)
						NetworkManager.CallbackBuffer.Add(room.Start);
					break;
				case Packet.ClientCodes.UPDATE_STATUS:
					UpdateStatus(packet);
					break;
				case Packet.ClientCodes.REQUEST_KICK:
					TryKick(packet);
					break;
			}
		}

		private void TryKick(Packet packet)
		{
			Packet.Reader reader = new Packet.Reader(packet);
			Room.Player p = room.UnpackPlayer(reader);
			if (!player.IsHost)
				return;
			(p.Client as ServerClient).Disconnect(DisconnectReason.Kicked);
		}

		void UpdateStatus(Packet packet)
		{
			Packet.Reader reader = new Packet.Reader(packet);
			switch ((RoomUpdate)reader.ReadByte())
			{
				case RoomUpdate.Status:
					Room.Player p = room.UnpackPlayer(reader);
					if (p.Client.ID == ID)
					{
						player.ready = reader.ReadBool();
						packet[0] = Packet.ServerCodes.ROOM_UPDATE;
						room.SendToAllPlayers(packet, p.Number);
					}
					break;
			}
		}

		void WelcomeConfirm(Packet packet)
		{
			Packet.Reader reader = new Packet.Reader(packet);

			int id = reader.ReadInt();
			if (id != ID)
			{
				Debug.LogError($"{this} assumed the wrong id");
				Disconnect(DisconnectReason.Error);
				return;
			}

			Nickname = reader.ReadString();
			Console.WriteLine($"{this} connected with nickname: {Nickname}");
		}
	}
}