using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace AssetFactory.Networking
{
	public class Client : IPacketHandler
	{
		public static Client Inst
		{
#if UNITY_SERVER
			get
			{
				Debug.LogError("Attempt to acces client instance when the application is a server");
				return null;
			}
			set
			{
				Debug.LogError("Attempt to set client instance when the application is a server");
			}
#else
			get; set;
#endif
		}
		public static NetworkSettings Settings
		{
#if UNITY_SERVER
			get
			{
				Debug.LogWarning("Attempt to access client network settings when application is a server");
				return Server.Settings;
			}
			set
			{
				Debug.LogWarning("Attempt to access client network settings when application is a server");
				Server.Settings = value;
			}
#else
			get; set;
#endif
		}

		public int id;
		public string nickname;

		public List<IPacketHandler> Callbacks { get; private set; }
		public List<Action> callbackBuffer;
		Dictionary<byte, List<Action<Packet>>> packetHandles;
		public List<MonoBehaviourNetwork> MonoCallbacks { get; private set; }

		bool isConnected = false;

		TcpClient tcp;
		NetworkStream stream;
		byte[] receiveBuffer;

#region Client TCP
		public static Client CreateClient(NetworkSettings settings)
		{
			if (Inst == null)
				Inst = new Client(settings);
			return Inst;
		}
		private Client(NetworkSettings settings)
		{
			Settings = settings;

			Callbacks = new List<IPacketHandler>
			{
				this
			};

			callbackBuffer = new List<Action>();
			packetHandles = new Dictionary<byte, List<Action<Packet>>>(255);
			MonoCallbacks = new List<MonoBehaviourNetwork>();
		}
		public void ConnectToServer()
		{
			tcp = new TcpClient()
			{
				ReceiveBufferSize = Settings.dataBufferSize,
				SendBufferSize = Settings.dataBufferSize
			};

			receiveBuffer = new byte[Settings.dataBufferSize];

			tcp.BeginConnect(Settings.serverIP, Settings.port, ConnectCallback, null);
		}
		void ConnectCallback(IAsyncResult result)
		{
			tcp.EndConnect(result);

			if (!tcp.Connected)
			{
				foreach (MonoBehaviourNetwork mbn in MonoCallbacks)
				{
					mbn.OnConnectionFailed();
				}
				isConnected = false;
				return;
			}
			isConnected = true;

			stream = tcp.GetStream();

			stream.BeginRead(receiveBuffer, 0, Settings.dataBufferSize, ReceiveCallback, null);
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
				Packet packet = new Packet(data);

				NetworkManager.Inst.AddCallbacks(Callbacks, packet);
				if (packetHandles.ContainsKey(packet.Code))
				{
					List<Action<Packet>> validHandles = new List<Action<Packet>>(packetHandles[packet.Code]);
					NetworkManager.Inst.AddCallbacks(validHandles, packet);
				}

				stream.BeginRead(receiveBuffer, 0, Settings.dataBufferSize, ReceiveCallback, null);
			}
			catch (Exception e)
			{
				Disconnect();
				Debug.LogError($"{e}");
			}
		}
		public void Send(Packet packet)
		{
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
		}
		public void Disconnect()
		{
			if (isConnected)
			{
				isConnected = false;
				tcp.Close();
				receiveBuffer = null;
				stream = null;
				tcp = null;


				Debug.Log("Disconnected from server");
				foreach (MonoBehaviourNetwork mbn in MonoCallbacks)
				{
					mbn.OnDisconnected();
				}
			}
			else if (tcp != null)
			{
				tcp.Close();
				receiveBuffer = null;
				tcp = null;
			}
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

		public void OnNetworkMessage(Packet packet)
		{
			switch(packet.Code)
			{
				case Packet.ServerCodes.WELCOME:
					ServerWelcome(packet);
					break;
				case Packet.ServerCodes.ROOM_OPERATION_RESULT:
					RoomResult(packet);
					break;
			}
		}

		void ServerWelcome(Packet packet)
		{
			Packet.Reader reader = new Packet.Reader(packet);

			Inst.id = reader.ReadInt();
			Debug.Log($"Server Message: {reader.ReadString()}");

			Packet.Writer writer = new Packet.Writer(Packet.ClientCodes.WELCOME_CONFIRM);
			writer.Write(Inst.id);
			writer.Write(nickname);

			Inst.Send(writer.EndWrite());

			foreach (MonoBehaviourNetwork mbn in MonoCallbacks)
			{
				mbn.OnConnected();
			}
		}
		void RoomResult(Packet packet)
		{
			Packet.Reader reader = new Packet.Reader(packet);
			RoomOperationResult result = (RoomOperationResult)reader.ReadByte();

			if (result == RoomOperationResult.CreationSucces)
			{
				Room.Inst = Room.UnpackRoom(reader);
				foreach (MonoBehaviourNetwork mbn in MonoCallbacks)
				{
					mbn.OnRoomCreated();
					mbn.OnRoomJoined();
				}
			}
			else if (result == RoomOperationResult.JoinSuccess)
			{
				Room.Inst = Room.UnpackRoom(reader);
				foreach (MonoBehaviourNetwork mbn in MonoCallbacks)
				{
					mbn.OnRoomJoined();
				}
			}
			else
			{
				foreach (MonoBehaviourNetwork mbn in MonoCallbacks)
				{
					mbn.OnRoomOperationFailed(result);
				}
			}
		}
		public void PlayerJoined(Packet packet)
		{
			
		}
	}
}