using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Originally from AssetFactory
namespace AssetFactory
{
	public class LocalMultiplayerManager : MonoBehaviour
	{
		private PlayerInputManager manager;

		private void Start()
		{
			manager = GetComponent<PlayerInputManager>();
		}

		private void OnPlayerJoined(PlayerInput player)
		{
			Debug.Log($"{player.name} joined");
		}

		private void OnPlayerLeft(PlayerInput player)
		{
			Debug.Log($"{player.name} left");
		}
	}
}
