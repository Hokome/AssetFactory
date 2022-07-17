using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory.UI
{
	public class YesNoPrompt : Prompt, ISingleton<YesNoPrompt>
	{
		public static YesNoPrompt Inst
		{
			get => ISingleton<YesNoPrompt>.Inst;
			set => ISingleton<YesNoPrompt>.Inst = value;
		}
		public static bool Exists => ISingleton<YesNoPrompt>.Exists;
		protected virtual void Awake()
		{
			Inst = this;
			Hide();
		}

		private Action<bool> callback;

		public static void Display(string message, Action<bool> callback)
		{
			Inst.Show();
			Inst.text.text = message;
			Inst.callback = callback;
		}
		public void InvokeCallback(bool value)
		{
			callback?.Invoke(value);
			Hide();
		}
	}
}
