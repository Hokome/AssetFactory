using AssetFactory.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AssetFactory
{
	public class Scoreboard : MonoSingleton<Scoreboard>
	{
		[SerializeField] private ScoreDisplay prefab;
		[SerializeField] private RectTransform parent;
		[SerializeField] private TMP_Text titleText;

		private Menu menu;
		private Menu Menu
		{
			get
			{
				if (menu == null)
					menu = GetComponent<Menu>();
				return menu;
			}
		}
		public void ShowScoreboard<T>(IEnumerable<T> scores) => ShowScoreboard(scores, t => t.ToString());
		public void ShowScoreboard<T>(IEnumerable<T> scores, Converter<T, string> converter)
		{
			MainMenu.Inst.StackSelect(Menu);
			parent.DestroyChildren();
			foreach (T score in scores)
			{
				Instantiate(prefab, parent).Text = converter(score);
			}
		}
	}
}
