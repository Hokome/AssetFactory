using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using RebindOperation = UnityEngine.InputSystem.InputActionRebindingExtensions.RebindingOperation;

namespace AssetFactory.UI.InputRebinding
{
	public class InputRebindMenu : SubMenu, ISaveable
	{
		private const string SAVE_PATH = "input.dat";

		[SerializeField] private InputActionAsset asset;
		[SerializeField] private RebindableInput[] rebindables;
		[SerializeField] private InputRebindUI prefab;
		[SerializeField] private InputIconDictionary iconDictionary;

		public bool IsRebinding => currentUI != null;

		private InputRebindUI currentUI;
		private RebindButton currentButton;
		private RebindOperation operation;

		private List<RebindInfo> overrides;

		private void Start()
		{
			InputIconDictionary.Inst = iconDictionary;
			ISaveable menu = this;
			if (!SaveManager.TryLoadFile(SAVE_PATH, ref menu))
				overrides = new();
			foreach (RebindInfo info in overrides)
			{
				asset.FindAction(info.Guid).ApplyBindingOverride(info.Index, info.Path);
			}
			for (int i = 0; i < rebindables.Length; i++)
			{
				var ui = Instantiate(prefab, transform);
				ui.Input = rebindables[i];
				ui.menu = this;
			}
		}
		private void Refresh()
		{
			foreach (var ui in GetComponentsInChildren<InputRebindUI>())
			{
				ui.Refresh();
			}
		}
		public void StartRebinding(InputRebindUI ui, int index)
		{
			if (IsRebinding) return;
			currentUI = ui;
			currentButton = ui.buttons[index];

			currentButton.SetWaiting(true);
			operation = ui.Input.Action.PerformInteractiveRebinding(ui.Input.Bindings[index])
				.WithTimeout(5f)
				.OnMatchWaitForAnother(0.1f)
				.OnComplete(_ => EndRebind(true))
				.OnCancel(_ => EndRebind(false))
				.Start();
		}
		public void EndRebind(bool result)
		{
			operation.Dispose();
			if (result)
			{
				RebindInfo info = currentUI.Input.GetRebindInfo(currentButton.Index);
				AddOverride(info);
			}

			currentButton.SetWaiting(false);
			currentUI = null;
			currentButton = null;
		}
		public override void Back()
		{
			if (IsRebinding)
				CancelRebind();
			base.Back();
		}
		private void CancelRebind() => operation.Cancel();
		private void AddOverride(RebindInfo info)
		{
			for (int i = 0; i < overrides.Count; i++)
			{
				RebindInfo current = overrides[i];
				if (current.Guid == info.Guid && current.Index == info.Index)
				{
					if (current.Path == info.Path)
						return;
					overrides[i] = info;
					goto Save;
				}
			}
			overrides.Add(info);
		Save:
			SaveManager.SaveFile(SAVE_PATH, this);
		}
		public void ResetToDefaultsCheck()
		{
			YesNoPrompt.Display("Do you want to reset input to defaults?", v =>
			{
				if (v)
					ResetToDefaults();
			});
		}
		public void ResetToDefaults()
		{
			overrides.Clear();
			SaveManager.SaveFile(SAVE_PATH, this);
			Refresh();
		}
		public void SaveFile(BinaryWriter writer) => writer.WriteCollection(overrides);
		public void LoadFile(BinaryReader reader) => overrides = new(reader.ReadCollection<RebindInfo>());
	}
	[System.Serializable]
	public class RebindableInput
	{
		[SerializeField] private InputActionReference action;
		[SerializeField] private int[] bindings;
		public InputAction Action => action.action;
		public int[] Bindings => bindings;
		public RebindInfo GetRebindInfo(int index)
		{
			int bindingIndex = bindings[index];
			string path = Action.bindings[bindingIndex].effectivePath;
			return new RebindInfo(Action.id.ToString(), bindingIndex, path);
		}
	}
	public struct RebindInfo : ISaveable
	{
		public string Guid { get; private set; }
		public int Index { get; private set; }
		public string Path { get; private set; }

		public RebindInfo(string guid, int index, string path)
		{
			Guid = guid;
			Index = index;
			Path = path;
		}

		public void LoadFile(BinaryReader reader)
		{
			Guid = reader.ReadString();
			Index = reader.ReadInt32();
			Path = reader.ReadString();
		}
		public void SaveFile(BinaryWriter writer)
		{
			writer.Write(Guid);
			writer.Write(Index);
			writer.Write(Path);
		}
	}
}
