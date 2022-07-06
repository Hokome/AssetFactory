using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviourPunCallbacks, IOnEventCallback
{
	public const byte SET_READY = 0;
	public const byte START_GAME = 1;

	bool isHosting = false;

	[Header("Groups")]
	[SerializeField] CanvasGroup mainGroup;
	[SerializeField] CanvasGroup readyingGroup;
	[SerializeField] CanvasGroup joinGroup;

	[Header("UI")]
	[SerializeField] Button backButton;
	[SerializeField] TMP_InputField idField;
	[SerializeField] TMP_InputField nicknameField;
	[SerializeField] TMP_Text hostStatus;
	[SerializeField] Button startButton;
	[SerializeField] GameObject statusesTransform;

	RoomPlayerStatusUI[] statuses;

	string nickname;

	#region Initialization
	void Awake()
	{
		PhotonNetwork.AddCallbackTarget(this);
		statuses = statusesTransform.GetComponentsInChildren<RoomPlayerStatusUI>();
		for (int i = 0; i < statuses.Length; i++)
		{
			statuses[i].id = i;
		}
	}

	public void Init()
	{
		MainMenu.Inst.OnBackInput += (ctx) => Back();
		MainMenu.Inst.OnBack += (m) => Disable();
		ResetUI();
	}
	public void Disable()
	{
		MainMenu.Inst.OnBack -= (m) => Disable();
		MainMenu.Inst.OnBackInput -= (ctx) => Back();
	}
	#endregion

	#region UI Commands
	public void Host()
	{
		nickname = nicknameField.text;
		if (string.IsNullOrWhiteSpace(nickname))
			return;

		isHosting = true;
		nickname = nicknameField.text;
		mainGroup.interactable = false;
		PhotonNetwork.ConnectUsingSettings();
	}
	public void Join()
	{
		nickname = nicknameField.text;
		if (string.IsNullOrWhiteSpace(nickname))
			return;

		mainGroup.interactable = false;
		MainMenu.Inst.backEnabled = false;
		SetGroup(joinGroup, true);
	}
	public void Connect()
	{
		isHosting = false;
		PhotonNetwork.ConnectUsingSettings();
	}
	public void Back()
	{
		if (MainMenu.Inst.backEnabled)
			MainMenu.Inst.Back();
		else
		{
			PhotonNetwork.Disconnect();
			ResetUI();
		}
	}
	public void StartGame()
	{
		if (!PhotonNetwork.IsMasterClient || !AllPlayersReady())
			return;

		PhotonNetwork.LoadLevel(1);
		RaiseEventOptions op = new RaiseEventOptions() { Receivers = ReceiverGroup.All };
		PhotonNetwork.RaiseEvent(START_GAME, null, op, SendOptions.SendReliable);
	}
	#endregion

	#region Networking
	public override void OnConnectedToMaster()
	{
		PhotonNetwork.LocalPlayer.NickName = nickname;
		if (isHosting)
		{
			PhotonNetwork.CreateRoom(GenerateCode(), new RoomOptions() { MaxPlayers = 4 });
		}
		else
		{
			if (!PhotonNetwork.JoinRoom(idField.text))
			{
				ResetUI();
				return;
			}
		}
	}
	public override void OnJoinedRoom()
	{
		if (isHosting)
			hostStatus.text = $"Hosting Room ID: {PhotonNetwork.CurrentRoom.Name}";
		else
			hostStatus.text = $"Room ID: {PhotonNetwork.CurrentRoom.Name}";

		SetGroup(joinGroup, false);
		SetGroup(readyingGroup, true);
		MainMenu.Inst.backEnabled = false;

		foreach (Player pl in NetworkManager.nPlayers)
		{
			SetStatus(pl);
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		SetStatus(newPlayer);
		if (PhotonNetwork.IsMasterClient)
		{
			RaiseEventOptions op = new RaiseEventOptions() { Receivers = ReceiverGroup.Others };
			foreach (Player p in NetworkManager.nPlayers)
			{
				int id = p.ActorNumber;
				object[] data = new object[2] { statuses[id - 1].ready.isOn, id };
				PhotonNetwork.RaiseEvent(SET_READY, data, op, SendOptions.SendReliable);
			}
		}
	}
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		RemoveStatus(otherPlayer);
	}

	#region Errors
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		ResetUI();
		PhotonNetwork.Disconnect();
		Debug.LogError(message);
		base.OnCreateRoomFailed(returnCode, message);
	}
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		PhotonNetwork.Disconnect();
		Debug.LogError(message);
		base.OnJoinRoomFailed(returnCode, message);
	}
	#endregion

	#endregion

	#region Statuses
	public void SetStatus(Player player)
	{
		RoomPlayerStatusUI s = statuses[player.ActorNumber - 1];

		s.SetActive(true);
		s.playerName.text = player.NickName;
		s.ready.interactable = player.IsLocal;
		s.playerId.text = $"Player {player.ActorNumber}";
	}

	public void RemoveStatus(Player id)
	{
		RoomPlayerStatusUI s = statuses[id.ActorNumber - 1];
		s.playerName.text = "Name";
		s.SetActive(false);
	}
	#endregion

	#region UI Utility
	void SetGroup(CanvasGroup g, bool active)
	{
		g.interactable = active;
		g.alpha = active ? 1f : 0f;
	}
	void ResetUI()
	{
		isHosting = false;

		SetGroup(mainGroup, true);
		SetGroup(readyingGroup, false);
		SetGroup(joinGroup, false);

		MainMenu.Inst.backEnabled = true;
		startButton.interactable = false;

		foreach (RoomPlayerStatusUI rps in statuses)
		{
			rps.SetActive(false);
		}
	}
	public void CheckInputField()
	{
		idField.text = idField.text.ToLower();
	}
	public void CopyRoomID()
	{
		if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
			return;
		Utility.CopyToClipboard(PhotonNetwork.CurrentRoom.Name);
	}
	#endregion

	private static System.Random random = new System.Random();
	private const int CODE_LENGTH = 6;
	public static string GenerateCode()
	{
		const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
		return new string(Enumerable.Repeat(chars, CODE_LENGTH)
		  .Select(s => s[random.Next(s.Length)]).ToArray());
	}

	bool AllPlayersReady()
	{
		for (int i = 0; i < NetworkManager.nPlayers.Length; i++)
		{
			if (!statuses[i].ready.isOn)
				return false;
		}
		return true;
	}
	public void OnEvent(EventData photonEvent)
	{
		if (photonEvent.Code == SET_READY)
		{
			object[] ob = (object[])photonEvent.CustomData;
			bool b = (bool)ob[0];
			int id = (int)ob[1];
			if (id != PhotonNetwork.LocalPlayer.ActorNumber)
				statuses[id - 1].ready.SetIsOnWithoutNotify(b);

			startButton.interactable = isHosting && NetworkManager.nPlayers.Length > 1 && AllPlayersReady();
		}
		else if (photonEvent.Code == START_GAME)
		{
			NetworkManager.Init();
		}
	}
}
