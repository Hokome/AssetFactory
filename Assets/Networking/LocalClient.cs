using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace AssetFactory.Networking
{
	public class LocalClient : NetworkClient
	{
		public static LocalClient Inst { get; private set; }

		public override bool IsLocal => true;
		public bool IsConnected { get; private set; }

		public static LocalClient CreateClient()
		{
			if (Inst == null)
				Inst = new LocalClient();
			return Inst;
		}
		private LocalClient()
		{
#if UNITY_SERVER
			Debug.LogWarning("Attempt to create a local client when the application is a server");
#endif
			Callbacks = new List<IReceiveCallback>();
		}
		public void ConnectToServer()
		{
			TCP = new TcpClient()
			{
				ReceiveBufferSize = NetworkSettings.Settings.dataBufferSize,
				SendBufferSize = NetworkSettings.Settings.dataBufferSize
			};

			TCP.BeginConnect(NetworkSettings.Settings.serverIP, NetworkSettings.Settings.port, ConnectCallback, null);
			connectionTimeOut = NetworkManager.Inst.StartCoroutine(ConnectionTimeOut());
		}
		void ConnectCallback(IAsyncResult result)
		{
			TCP.EndConnect(result);

			if (!TCP.Connected)
			{
				IsConnected = false;
				return;
			}
			IsConnected = true;

			InitializeConnection();
			NetworkManager.CallbackBuffer.Add(() => NetworkManager.Inst.StopCoroutine(connectionTimeOut));
		}
		Coroutine connectionTimeOut;
		IEnumerator ConnectionTimeOut()
		{
			yield return new WaitForSecondsRealtime(NetworkSettings.Settings.loginTimeOut);
			ErrorPrompt.Inst.DisplayMessage("Connection timed out");
			Disconnect();
		}
		public override void Disconnect(DisconnectReason reason = DisconnectReason.Unknown)
		{
			if (TCP == null) return;

			Debug.Log($"{this} disconnected: {DisconnectMessage(reason)}");
			switch (reason)
			{
				case DisconnectReason.ApplicationClosed:
					SendDisconnect(reason);
					break;
			}

			base.Disconnect(reason);
		}

		public override void OnNetworkMessage(Packet packet)
		{
			switch (packet.Code)
			{
				case Packet.ServerCodes.WELCOME:
					ServerWelcome(packet);
					break;
				case Packet.ServerCodes.DISCONNECT:
					Packet.Reader reader = new Packet.Reader(packet);
					Disconnect((DisconnectReason)reader.ReadByte());
					break;
				case Packet.ServerCodes.ROOM_OPERATION_RESULT:
					RoomResult(packet);
					break;
				case Packet.ServerCodes.PLAYER_JOINED:
					room.AddPlayer(new Packet.Reader(packet));
					break;
				case Packet.ServerCodes.PLAYER_LEFT:
					Room.Player p = room.UnpackPlayer(new Packet.Reader(packet));
					room.RemovePlayerLocal(p);
					break;
				case Packet.ServerCodes.ROOM_UPDATE:
					UpdateRoom(packet);
					break;
			}
		}


		void UpdateRoom(Packet packet)
		{
			Packet.Reader reader = new Packet.Reader(packet);
			RoomUpdate update = (RoomUpdate)reader.ReadByte();
			switch (update)
			{
				case RoomUpdate.Status:
					Room.Player p = room.UnpackPlayer(reader);
					p.ready = reader.ReadBool();
					break;
				case RoomUpdate.Kick:
					p = room.UnpackPlayer(reader);
					room.RemovePlayerLocal(p);
					break;
				case RoomUpdate.Closing:
					Room.Current = null;
					room = null;
					player = null;
					Disconnect(DisconnectReason.RoomClosed);
					break;
				default:
					break;
			}
		}

		void ServerWelcome(Packet packet)
		{
			Packet.Reader reader = new Packet.Reader(packet);

			ID = reader.ReadInt();
			Debug.Log($"Server Message: {reader.ReadString()}");

			Packet.Writer writer = new Packet.Writer(Packet.ClientCodes.WELCOME_CONFIRM);
			writer.Write(ID);
			writer.Write(Nickname);

			Send(writer.Finish());
		}
		void RoomResult(Packet packet)
		{
			Packet.Reader reader = new Packet.Reader(packet);
			RoomOperationResult result = (RoomOperationResult)reader.ReadByte();

			if (result == RoomOperationResult.CreationSucces || result == RoomOperationResult.JoinSuccess)
			{
				Room.Current = Room.Unpack(reader);
			}
		}
		
		public void Check()
		{
			Send(new Packet.Writer(Packet.ClientCodes.CHECK).Finish());
		}
	}
}