using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory.Networking
{
	public interface IPacketHandler
	{
		void OnNetworkMessage(Packet packet);
	}
}