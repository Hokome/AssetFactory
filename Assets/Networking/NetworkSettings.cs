using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory
{
	[CreateAssetMenu(menuName = "Network Settings", fileName = "Network_Settings", order = 1000)]
	public class NetworkSettings : ScriptableObject
	{
		static NetworkSettings currentSettings;
		public static NetworkSettings Settings
		{
			get => currentSettings;
			set
			{
				if (currentSettings != null)
				{
					Debug.LogError("Network settings have already been set");
				}
				else
				{
					currentSettings = value;
				}
			}
		}

		[InspectorName("Server IP")]
		public string serverIP = "127.0.0.1";
		public short port = 25;
		[Header("Room settings")]
		public int maxServerConnections = 20;
		public int minPlayers = 2;
		public int maxPlayers = 4;
		public int maxRooms = 5;
		[Header("Connection settings")]
		[Tooltip("Time out when connection is established")]
		public float connectionTimeOut = 20f;
		[Tooltip("Time out when connecting")]
		public float loginTimeOut = 10f;
		public float checkFrequency = 1f;
		public int dataBufferSize = 1024;

		public string Version { get => Application.version; }
	}
}