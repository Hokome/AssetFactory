using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Orignially from AssetFactory
namespace AssetFactory
{
	public class PlayerSlot : MonoBehaviour
	{
		public int id;

		[SerializeField] Toggle ready;
		[SerializeField] TMP_Text playerStatus;
		[SerializeField] TMP_Text playerId;

		const string WAIT = "Press any button...";
		const string PREP = "Preparing...";
		const string READY = "Ready!";

		private void Start()
		{
			playerId.text = $"Player {id}";
			SetActive(false);
		}

		public void SetActive(bool v)
		{
			ready.SetIsOnWithoutNotify(false);
			playerStatus.text = v ? PREP : WAIT;
			ready.interactable = v;
		}
		
		public void SetReady(bool v)
		{
			playerStatus.text = v ? READY : PREP;
		}
	}
}
