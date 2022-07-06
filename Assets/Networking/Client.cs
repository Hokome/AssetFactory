using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory.Networking
{
	public class Client
	{
		public int ID { get; protected set; }
		public string Nickname { get; set; }
		public virtual bool IsLocal { get => false; }
		public bool InRoom { get => room != null; }

		public Room room;
		public Room.Player player;

		protected Client() { }
		public Client(int id)
		{
			ID = id;
		}
		public override string ToString()
		{
			return $"Client [{ID}]";
		}
	}
}