using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace AssetFactory.UI.InputRebinding
{
	public class InputRebindUI : MonoBehaviour
	{
		public RebindButton[] buttons;
		[SerializeField] private TextMeshProUGUI actionName;

		[HideInInspector] public InputRebindMenu menu;
		[HideInInspector] public int index;

		private RebindableInput input;
		public RebindableInput Input
		{
			get => input;
			set
			{
				input = value;
				Refresh();
			}
		}

		public void Refresh()
		{
			actionName.text = Input.Action.name;
			for (int i = 0; i < buttons.Length; i++)
				buttons[i].Initialize(this, i);
		}
		public void StartRebinding(int index) => menu.StartRebinding(this, index);
	}
}
