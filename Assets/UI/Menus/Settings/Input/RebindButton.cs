using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AssetFactory.UI.InputRebinding
{
	public class RebindButton : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI inputText;
		[SerializeField] private Image inputIcon;
		[SerializeField] private GameObject button;
		[SerializeField] private GameObject waitingText;

		public int Index { get; private set; }
		private InputRebindUI master;
		
		public void Initialize(InputRebindUI master, int index)
		{
			this.Index = index;
			this.master = master;
			SetWaiting(false);
		}
		public void SetWaiting(bool value)
		{
			waitingText.SetActive(value);
			button.SetActive(!value);
			if (!value)
				SetInputDisplay();
		}
		private void SetDisplayMode(bool iconMode)
		{
			inputText.gameObject.SetActive(!iconMode);
			inputIcon.gameObject.SetActive(iconMode);
		}
		private void SetInputDisplay()
		{
			string path = Action.bindings[BindingIndex].effectivePath;
			if (InputIconDictionary.Inst.TryGetValue(path, out Sprite s))
			{
				SetDisplayMode(true);
				inputIcon.sprite = s;
			}
			else
			{
				SetDisplayMode(false);
				var options = InputControlPath.HumanReadableStringOptions.OmitDevice;
				inputText.text = InputControlPath.ToHumanReadableString(path, options);
			}
		}
		public void StartRebind() => master.StartRebinding(Index);
		private int BindingIndex => master.Input.Bindings[Index];
		private InputAction Action => master.Input.Action;
		private string GetInputName()
			=> InputControlPath.ToHumanReadableString(Action.bindings[Index].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
	}
}
