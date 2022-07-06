using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	public static NetworkManager inst;

	public static Player[] nPlayers;
	public static PlayerBehaviour[] players;
	public static bool inGame;

	void Awake()
	{
		if (inst != null && inst != this)
		{
			Destroy(gameObject);
			return;
		}
		inst = this;
		DontDestroyOnLoad(gameObject);
	}

	public static void Init() => inst.Inititialize();
	public void Inititialize()
	{
		MainMenu.Inst.enabled = false;
		players = new PlayerBehaviour[nPlayers.Length];
		PhotonNetwork.Instantiate("Player", Vector2.zero, Quaternion.identity);
	}
	public override void OnJoinedRoom()
	{
		UpdatePlayerList();
		base.OnJoinedRoom();
	}
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		UpdatePlayerList();
		base.OnPlayerEnteredRoom(newPlayer);
	}
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		UpdatePlayerList();
		base.OnPlayerLeftRoom(otherPlayer);
	}

	public override void OnLeftRoom()
	{
		nPlayers = null;
		base.OnLeftRoom();
	}
	public override void OnDisconnected(DisconnectCause cause)
	{
		nPlayers = null;
		base.OnDisconnected(cause);
	}

	void UpdatePlayerList() => nPlayers = PhotonNetwork.PlayerList;
}
