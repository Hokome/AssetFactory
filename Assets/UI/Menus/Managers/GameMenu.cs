using AssetFactory.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory
{
    public class GameMenu : MenuManager
    {
		public static GameMenu Inst { get; private set; }

		//Can be useful if the game has pop up menus only.

		//public override void Back()
		//{
		//	if (!backEnabled) return;
		//	CurrentMenu.Display(false);
		//	if (navigationStack.Count > 0)
		//	{
		//		CurrentMenu = navigationStack.Pop();
		//		CallOnBack();
		//	}
		//}

		private void Awake()
		{
			if (Inst == null)
			{
				Inst = this;
				DontDestroyOnLoad(gameObject);
				currentMenu = main;
			}
			else
			{
				Debug.LogWarning($"Singleton for {typeof(PauseMenu)} already exists.");
				Destroy(gameObject);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
		protected override void OnDisable()
		{
			base.OnDisable();
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
}
