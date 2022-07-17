using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory.UI
{
	public class SubMenu : Menu
	{
		protected static MenuManager CurrentMenu => PauseMenu.IsPaused ? PauseMenu.Inst : MainMenu.Inst;
		public bool BackEnabled
		{
			set => CurrentMenu.BackEnabled = value;
		}

		public virtual void Back() => CurrentMenu.Back();

		public void StackSelect(Menu menu) => CurrentMenu.StackSelect(menu);
		public void SoleSelect(Menu menu) => CurrentMenu.SoleSelect(menu);
	}
}
