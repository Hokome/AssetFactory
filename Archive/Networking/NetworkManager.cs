using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory.Networking
{
	public class NetworkManager : MonoBehaviour
	{
		public static NetworkManager Inst { get; private set; }
		[SerializeField] NetworkSettings settings;

		void Awake()
		{
			if (Inst == null)
				Inst = this;
			else
			{
				Destroy(gameObject);
				return;
			}

			DontDestroyOnLoad(gameObject);
#if UNITY_SERVER
			Server.CreateServer(settings);
#else
			Client.CreateClient(settings);
#endif
		}

		void OnApplicationQuit()
		{
			Client.Inst.Disconnect();
		}

		void FixedUpdate()
		{
#if UNITY_SERVER
			ExecuteCallbacks(Server.Inst.callbackBuffer);
#else
			ExecuteCallbacks(Client.Inst.callbackBuffer);
#endif
		}
		List<Action> callbacksCopy = new List<Action>();
		void ExecuteCallbacks(List<Action> callbacks)
		{
			if (callbacks.Count < 0)
				return;
			callbacksCopy.Clear();
			lock (callbacks)
			{
				callbacksCopy.AddRange(callbacks);
				callbacks.Clear();
			}

			callbacksCopy.ForEach((c) => c());
		}
		public void AddCallbacks(List<Action<Packet>> actions, Packet packet)
		{
			Action[] a = new Action[actions.Count];
			for (int i = 0; i < a.Length; i++)
			{
				Action<Packet> ap = actions[i];
				a[i] = () => ap(packet);
			}
#if UNITY_SERVER
			Server.Inst.callbackBuffer.AddRange(a);
#else
			Client.Inst.callbackBuffer.AddRange(a);
#endif
		}
		public void AddCallbacks(List<IPacketHandler> handlers, Packet packet)
		{
			Action[] a = new Action[handlers.Count];
			for (int i = 0; i < a.Length; i++)
			{
				IPacketHandler h = handlers[i];
				a[i] = () => h.OnNetworkMessage(packet);
			}
#if UNITY_SERVER
			Server.Inst.callbackBuffer.AddRange(a);
#else
			Client.Inst.callbackBuffer.AddRange(a);
#endif
		}
	}
}
