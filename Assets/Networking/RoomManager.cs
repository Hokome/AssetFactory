using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetFactory.Networking
{
	public class RoomManager : MonoBehaviour
	{
		[HideInInspector]
		public Room room;

		void Awake()
		{
#if !UNITY_SERVER
			Init(Room.Current);
#endif
		}

		public void Init(Room room)
		{
			this.room = room;
			room.manager = this;
		}
	}
}