using AssetFactory.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AssetFactory.UI
{
	public class RoomUI : MonoBehaviour, IReceiveCallback
	{
		[Header("Groups")]
		[SerializeField] CanvasGroup connectGroup;
		[SerializeField] CanvasGroup hostGroup;
		[SerializeField] CanvasGroup joinGroup;
		[SerializeField] CanvasGroup roomGroup;

		[Header("UI")]
		[SerializeField] TMP_InputField idField;
		[SerializeField] TMP_InputField nicknameField;
		[SerializeField] TMP_Text hostStatus;
		[SerializeField] Button startButton;
		[SerializeField] GameObject statusesTransform;

		RoomPlayerStatusUI[] statuses;
		private enum UIState { Main, Host, Join, Room }
		UIState state = UIState.Main;

		bool isHosting;
		

		#region Initialization
		void Awake()
		{
			statuses = statusesTransform.GetComponentsInChildren<RoomPlayerStatusUI>();
			for (int i = 0; i < statuses.Length; i++)
			{
				statuses[i].id = i;
			}
		}
		public void OnEnable()
		{
			LocalClient.Inst.OnDisconnected += OnDisconnected;
			LocalClient.Inst.Callbacks.Add(this);
			foreach (RoomPlayerStatusUI rps in statuses)
			{
				rps.Init();
			}
			ResetUI();
		}
		public void OnDisable()
		{
			LocalClient.Inst.OnDisconnected -= OnDisconnected;
			LocalClient.Inst.Callbacks.Remove(this);
		}
		#endregion

		#region UI Commands
		public void Connect(bool asHost)
		{
			if (string.IsNullOrWhiteSpace(nicknameField.text))
				return;

			LocalClient.Inst.Nickname = nicknameField.text;
			connectGroup.interactable = false;

			isHosting = asHost;

			LocalClient.Inst.ConnectToServer();
		}
		public void CreateRoom()
		{
			Packet.Writer writer = new Packet.Writer(Packet.ClientCodes.REQUEST_CREATE_ROOM);
			LocalClient.Inst.Send(writer.Finish());
		}
		public void JoinRoom()
		{
			if (idField.text.Length < Room.CODE_LENGTH)
				return;

			Packet.Writer writer = new Packet.Writer(Packet.ClientCodes.REQUEST_JOIN_ROOM);
			writer.Write(idField.text.ToLower());

			LocalClient.Inst.Send(writer.Finish());
		}
		public void Back()
		{
			switch (state)
			{
				case UIState.Main:
					LocalClient.Inst.Disconnect(DisconnectReason.ClientCommand);
					break;
				case UIState.Host:
				case UIState.Join:
				case UIState.Room:
					LocalClient.Inst.Disconnect(DisconnectReason.ClientCommand);
					ResetUI();
					break;
			}
		}

		public void SetReady(bool value)
		{
			if (value)
				return;
		}
		public void StartGame()
		{
			if (!LocalClient.Inst.player.IsHost)
				return;
			Packet.Writer wr = new Packet.Writer(Packet.ClientCodes.REQUEST_START_GAME);
			LocalClient.Inst.Send(wr.Finish());
		}
		public void CopyRoomID()
		{
			if (!LocalClient.Inst.InRoom)
				return;
			Utility.CopyToClipboard(Room.Current.ID);
		}
		#endregion

		#region Networking
		public void OnNetworkMessage(Packet packet)
		{
			switch (packet.Code)
			{
				case Packet.ServerCodes.WELCOME:
					OnConnected();
					break;
				case Packet.ServerCodes.ROOM_OPERATION_RESULT:
					RoomOperationResult result = (RoomOperationResult)new Packet.Reader(packet).ReadByte();
					if (result == RoomOperationResult.CreationSucces)
						OnRoomCreated();
					else if (result == RoomOperationResult.JoinSuccess)
						OnRoomJoined();
					else
						OnRoomOperationFailed(result);
					break;
				case Packet.ServerCodes.PLAYER_JOINED:
				case Packet.ServerCodes.PLAYER_LEFT:
				case Packet.ServerCodes.ROOM_UPDATE:
					UpdateStatuses();
					break;
				case Packet.ServerCodes.START_GAME:
					break;
			}
		}
		void OnConnected()
		{
			if (isHosting)
			{
				SetGroup(hostGroup, true);
				state = UIState.Host;
			}
			else
			{
				SetGroup(joinGroup, true);
				state = UIState.Join;
			}
			SetGroup(connectGroup, false);
		}
		void OnRoomCreated()
		{
			SetGroup(roomGroup, true);
			SetGroup(hostGroup, false);
			hostStatus.text = $"Hosting : {Room.Current.ID}";
			startButton.interactable = true;
			UpdateStatuses();
		}
		void OnRoomJoined()
		{
			SetGroup(roomGroup, true);
			SetGroup(joinGroup, false);
			startButton.interactable = false;
			hostStatus.text = $"Attending : {Room.Current.ID}";
			UpdateStatuses();
		}

		#region Errors
		void OnConnectionFailed()
		{
			ErrorPrompt.Inst.DisplayMessage("Could not connect to server");
			ResetUI();
		}
		void OnRoomOperationFailed(RoomOperationResult result)
		{
			if (isHosting)
				ErrorPrompt.Inst.DisplayMessage($"Failed to create room: {Room.RoomOperationMessage(result)}");
			else
				ErrorPrompt.Inst.DisplayMessage($"Failed to join room: {Room.RoomOperationMessage(result)}");
			ResetUI();
		}
		void OnDisconnected(DisconnectReason reason)
		{
			switch (reason)
			{
				case DisconnectReason.RoomClosed:
					ErrorPrompt.Inst.DisplayMessage("The room has been closed.");
					break;
				case DisconnectReason.Kicked:
					ErrorPrompt.Inst.DisplayMessage("You were kicked from the room.");
					break;
			}
			ResetUI();
		}
		#endregion

		#endregion

		#region Statuses
		void UpdateStatuses()
		{
			int i = 0;
			List<Room.Player> players = Room.Current.PlayerList;
			while (i < players.Count)
			{
				RoomPlayerStatusUI psui = statuses[i];
				Room.Player p = players[i];
				psui.SetPlayer(p);
				psui.SetActive(true);
				psui.ready.SetIsOnWithoutNotify(p.ready);
				i++;
			}
			while (i < NetworkSettings.Settings.maxPlayers)
			{
				RoomPlayerStatusUI psui = statuses[i];
				psui.SetActive(false);
				i++;
			}
		}
		#endregion

		#region UI Utility
		void SetGroup(CanvasGroup g, bool active)
		{
			g.interactable = active;
			g.blocksRaycasts = active;
			g.alpha = active ? 1f : 0f;
		}
		void ResetUI()
		{
			state = UIState.Main;

			SetGroup(connectGroup, true);
			SetGroup(hostGroup, false);
			SetGroup(roomGroup, false);
			SetGroup(joinGroup, false);

			//startButton.interactable = false;
			startButton.interactable = true;

			foreach (RoomPlayerStatusUI rps in statuses)
			{
				rps.SetActive(false);
			}
		}
		public void CheckInputField()
		{
			idField.text = idField.text.ToLower();
		}
		#endregion
	}
}