using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetFactory.UI;

//Orignially from AssetFactory
namespace AssetFactory.UI
{
	public class ErrorPrompt : Prompt, ISingleton<ErrorPrompt>
	{
		public static bool IsShowing { get; private set; }
		public static ErrorPrompt Inst => ISingleton<ErrorPrompt>.Inst;
		public static bool Exists => ISingleton<ErrorPrompt>.Exists;
		private void Awake() => ISingleton<ErrorPrompt>.Inst = this;

		public static void DisplayMessage(string message)
		{
			if (IsShowing)
				return;
			IsShowing = true;
		}
		public static void HidePrompt() => Inst.Hide();
	}
}