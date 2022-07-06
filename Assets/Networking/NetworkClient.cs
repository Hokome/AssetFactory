using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AssetFactory.Networking
{
	public enum DisconnectReason
	{
		Unknown,
		Error,
		ServerClosed,
		ApplicationClosed,
		TimeOut,
		ClientCommand,
		RoomClosed,
		Kicked
	}

	public abstract class NetworkClient : Client, IReceiveCallback
	{
		public TcpClient TCP { get; protected set; }
		protected NetworkStream stream;
		protected byte[] receiveBuffer;

		public List<IReceiveCallback> Callbacks { get; protected set; }

		public event Action<DisconnectReason> OnDisconnected = delegate { };

		protected void InitializeConnection()
		{
			receiveBuffer = new byte[NetworkSettings.Settings.dataBufferSize];
			stream = TCP.GetStream();

			stream.BeginRead(receiveBuffer, 0, NetworkSettings.Settings.dataBufferSize, ReceiveCallback, null);
		}

		void ReceiveCallback(IAsyncResult result)
		{
			try
			{
				if (!ConnectionCheck())
					return;

				int length = stream.EndRead(result);

				if (length < 1)
				{
					Disconnect();
					return;
				}

				byte[] data = new byte[length];
				Array.Copy(receiveBuffer, data, length);
				Packet packet = new Packet(data);
#if UNITY_SERVER
				packet.sender = ID;
#endif
				OnNetworkMessage(packet);
				NetworkManager.Inst.AddCallbacks(Callbacks, packet);

				if (stream == null) return;
				stream.BeginRead(receiveBuffer, 0, NetworkSettings.Settings.dataBufferSize, ReceiveCallback, null);
			}
			catch (ObjectDisposedException)
			{
				LogDisposed();
			}
			catch (Exception e)
			{
				Disconnect(DisconnectReason.Error);
				Debug.LogError($"{e}");
			}
		}

		/// <summary>
		/// Sends packet to peer asynchronously.
		/// </summary>
		public async void Send(Packet packet)
		{
			try
			{
				if (!ConnectionCheck())
					return;

				await stream.WriteAsync(packet.Data, 0, packet.Length);
			}
			catch (ObjectDisposedException)
			{
				LogDisposed();
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}
		/// <summary>
		/// Sends packet to peer synchronously. If possible use <see cref="Send(Packet)"/> instead.
		/// </summary>
		public void SendImmediate(Packet packet)
		{
			try
			{
				if (!ConnectionCheck())
					return;

				stream.Write(packet.Data, 0, packet.Length);
			}
			catch (ObjectDisposedException)
			{
				LogDisposed();
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}
		void LogDisposed()
		{
#if UNITY_SERVER
			Console.WriteLine("Accessing closed connection!");
#else
			Debug.LogWarning("Accessing closed connection!");
#endif
		}
		public virtual void Disconnect(DisconnectReason reason = DisconnectReason.Unknown)
		{
			if (TCP == null)
				return;

			NetworkManager.CallbackBuffer.Add(() => OnDisconnected(reason));
			TCP.Close();
			TCP = null;
			stream = null;
			receiveBuffer = null;
		}
		protected void SendDisconnect(DisconnectReason reason)
		{
			Packet.Writer writer = new Packet.Writer(Packet.ClientCodes.DISCONNECT);
			writer.Write((byte)reason);

			SendImmediate(writer.Finish());
		}
		protected bool ConnectionCheck() => TCP != null && stream != null;
		public virtual void OnNetworkMessage(Packet packet) { }
		protected static string DisconnectMessage(DisconnectReason reason)
		{
			return reason switch
			{
				DisconnectReason.Unknown => "Unknown",
				DisconnectReason.Error => "A networking error occured",
				DisconnectReason.ApplicationClosed => "Application has closed",
				DisconnectReason.ServerClosed => "The server has stopped",
				DisconnectReason.TimeOut => "Timed out",
				DisconnectReason.ClientCommand => "Client command",
				DisconnectReason.RoomClosed => "The room was closed",
				DisconnectReason.Kicked => "Kicked from the room",
				_ => reason.ToString()
			};
		}
	}
}