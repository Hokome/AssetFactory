using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory.Networking
{
	public class MonoBehaviourNetwork : MonoBehaviour, IPacketHandler
	{
		public virtual void OnNetworkMessage(Packet data) { }

		void OnEnable()
		{
			Client.Inst.MonoCallbacks.Add(this);
			Client.Inst.Callbacks.Add(this);
		}
		void OnDisable()
		{
			Client.Inst.MonoCallbacks.Remove(this);
			Client.Inst.Callbacks.Remove(this);
		}

		public virtual void OnConnected() { }
		public virtual void OnConnectionFailed() { }
		public virtual void OnRoomCreated() { }
		public virtual void OnRoomJoined() { }
		public virtual void OnPlayerJoined(Room.Player player) { }
		public virtual void OnPlayerLeft(Room.Player player) { }

		public virtual void OnRoomOperationFailed(RoomOperationResult result) { }
		public virtual void OnDisconnected() { }
	}
}