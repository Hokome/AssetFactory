using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

[RequireComponent(typeof(CanvasGroup))]
public class RoomPlayerStatusUI : MonoBehaviour
{
	public bool isLocal;

	public Toggle ready;
	public TMP_Text playerName;
	public TMP_Text playerId;

	CanvasGroup cg;

	[HideInInspector] public int id;

	private void Awake()
	{
		cg = GetComponent<CanvasGroup>();
	}
	public void Init()
	{
		SetActive(false);
	}

	public void SetActive(bool value)
	{
		if (!value)
			ready.isOn = false;
		else
			ready.interactable = isLocal;

		cg.alpha = value ? 1f : 0f;
		cg.interactable = value;
	}

	public void SetReady(bool value)
	{
		if (!PhotonNetwork.IsConnected || id + 1 != PhotonNetwork.LocalPlayer.ActorNumber)
			return;

		object[] data = new object[2] { value, PhotonNetwork.LocalPlayer.ActorNumber };
		RaiseEventOptions options = new RaiseEventOptions() { Receivers = ReceiverGroup.All };
		PhotonNetwork.RaiseEvent(RoomUI.SET_READY, data, options, SendOptions.SendReliable);
	}
}
