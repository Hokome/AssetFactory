using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory.Networking
{
	public class NetworkManager : MonoBehaviour
	{
		public static NetworkManager Inst { get; private set; }
		[SerializeField] NetworkSettings settings;
		public static List<Action> CallbackBuffer { get => Inst.callbackBuffer; }

		void Awake()
		{
			if (Inst != null)
			{
				Destroy(gameObject);
				return;
			}
			Inst = this;
			NetworkSettings.Settings = settings;

			DontDestroyOnLoad(gameObject);
#if UNITY_SERVER
			Server.CreateServer();
#else
			LocalClient.CreateClient();
#endif
		}

		float lastCheck;
		void FixedUpdate()
		{
			ExecuteCallbacks();
			if (Time.unscaledTime - lastCheck > NetworkSettings.Settings.checkFrequency)
			{
#if UNITY_SERVER
				Server.Inst.CheckClients();
#else
				if (LocalClient.Inst.IsConnected)
					LocalClient.Inst.Check();
#endif
				lastCheck = Time.unscaledTime;
			}
		}

		/// <summary>
		/// List of actions to do on the main Unity thread to avoid conflict.
		/// </summary>
		public List<Action> callbackBuffer = new List<Action>();
		List<Action> callbackBufferCopy = new List<Action>();
		void ExecuteCallbacks()
		{
			if (callbackBuffer.Count <= 0)
				return;
			callbackBufferCopy.Clear();
			lock (callbackBuffer)
			{
				callbackBufferCopy.AddRange(callbackBuffer);
				callbackBuffer.Clear();
			}

			callbackBufferCopy.ForEach((c) => c());
		}
		public void AddCallbacks(List<IReceiveCallback> handlers, Packet packet)
		{
			callbackBuffer.AddRange(handlers.ConvertAll<Action>((h) => () => h.OnNetworkMessage(packet)));
		}

		void OnApplicationQuit()
		{
#if UNITY_SERVER
			Server.Inst.CloseServer();
#else
			if (LocalClient.Inst != null)
				LocalClient.Inst.Disconnect(DisconnectReason.ApplicationClosed);
#endif
		}
	}
}