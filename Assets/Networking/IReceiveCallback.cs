using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory.Networking
{
	public interface IReceiveCallback
	{
		void OnNetworkMessage(Packet packet);
	}
}