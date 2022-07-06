using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AssetFactory.Networking;

namespace AssetFactory.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class RoomPlayerStatusUI : MonoBehaviour
	{
		public bool IsLocal { get => player != null && player.IsLocal; }

		public Toggle ready;
		[SerializeField] TMP_Text playerName;
		[SerializeField] TMP_Text playerId;
		[SerializeField] Button kickButton;

		Room.Player player;

		CanvasGroup cg;

		[HideInInspector] public int id;
		public void Init()
		{
			cg = GetComponent<CanvasGroup>();
			SetActive(false);
		}

		public void SetActive(bool value)
		{
			if (!value)
				ready.SetIsOnWithoutNotify(false);
			else
				ready.interactable = IsLocal;

			cg.alpha = value ? 1f : 0f;
			cg.interactable = value;
		}
		public void SetPlayer(Room.Player p)
		{
			player = p;
			playerName.text = p.Nickname;
			playerId.text = p.ToString();
			kickButton.gameObject.SetActive(LocalClient.Inst.player.IsHost);
			kickButton.interactable = !p.IsLocal;
		}

		public void SetReady(bool value)
		{
			player.ready = value;
			Packet.Writer writer = new Packet.Writer(Packet.ClientCodes.UPDATE_STATUS);
			writer.Write((byte)RoomUpdate.Status);
			Room.PackPlayer(player, writer);
			writer.Write(value);
			LocalClient.Inst.Send(writer.Finish());
		}
		public void Kick()
		{
			Packet.Writer writer = new Packet.Writer(Packet.ClientCodes.REQUEST_KICK);
			Room.PackPlayer(player, writer);
			LocalClient.Inst.Send(writer.Finish());
		}
	}

}