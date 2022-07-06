using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace AssetFactory.UI
{
	public class MenuManager : MonoBehaviour
	{
		[Header("Menus")]
		public List<Menu> list;
		public Menu ActiveMenu { get; private set; }
		Stack<Menu> stack;

		public bool backEnabled;
		public event Action<Menu> OnBack;

		public Action<InputAction.CallbackContext> OnBackInput = delegate { };

		EventSystem events;
		InputSystemUIInputModule input;

		protected void Init()
		{
			DontDestroyOnLoad(gameObject);
			stack = new Stack<Menu>();
			OnBack = delegate { };

			events = GetComponentInChildren<EventSystem>();
			input = GetComponentInChildren<InputSystemUIInputModule>();

			input.cancel.action.performed += Back;
			ToMain();

			events.SetSelectedGameObject(ActiveMenu.firstSelection);
		}

		public void SoleSelect(string name) => SoleSelect(GetMenu(name));
		public void SoleSelect(Menu menu)
		{
			if (!isActiveAndEnabled) return;

			foreach (Menu m in list)
				Display(m, false);

			Select(menu);
			menu.onLoad.Invoke();

			stack.Clear();
		}

		public void StackSelect(string name) => StackSelect(GetMenu(name), false);
		public void StackSelect(string name, bool showParent) => StackSelect(GetMenu(name), showParent);
		public void StackSelect(Menu menu, bool showParent = false)
		{
			if (!isActiveAndEnabled) return;

			stack.Push(ActiveMenu);

			Display(showParent);

			Select(menu);

			menu.onLoad.Invoke();
		}

		void Select(Menu menu)
		{
			if (!isActiveAndEnabled) return;

			ActiveMenu = menu;
			Display(true);

			if (menu.firstSelection != null)
				events.SetSelectedGameObject(menu.firstSelection);
		}

		protected void Back(InputAction.CallbackContext ctx)
		{
			if (!isActiveAndEnabled) return;
			OnBackInput.Invoke(ctx);
			Back();
		}
		public void Back()
		{
			if (!backEnabled) return;
			if (stack.Count <= 0) return;

			Display(false);

			Menu m = stack.Pop();
			OnBack.Invoke(m);
			Select(m);
		}

		public void ToMain()
		{
			SoleSelect(list[0]);
		}

		void OnDisable()
		{
			Display(false);
		}

		void OnEnable()
		{
			Display(true);
		}

		void Display(Menu m, bool b)
		{
			m.holder.alpha = b ? 1f : 0f;
			m.holder.interactable = b;
			m.holder.blocksRaycasts = b;
		}

		public void Display(bool b)
		{
			if (ActiveMenu != null)
				Display(ActiveMenu, b);
		}

		public Menu GetMenu(string name)
		{
			foreach (Menu m in list)
				if (name == m.name)
					return m;

			Debug.LogError($"Menu with name '{name}' was not found");
			return null;
		}

		public void SelectObject(GameObject g)
		{
			if (!g.activeSelf)
				return;

			events.SetSelectedGameObject(g);
		}

		public void OnDestroy()
		{
			input.cancel.action.performed -= OnBackInput;
		}

		[Serializable]
		public class Menu
		{
			public string name;
			public CanvasGroup holder;
			public GameObject firstSelection;

			public UnityEvent onLoad;
		}
	}
}