using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AssetFactory.UI
{
    /// <summary>
    /// Component to handle displaying menus
    /// </summary>
    public class MenuManager : MonoSingleton<MenuManager>
    {
        [SerializeField] private Menu _firstMenu;

        /// <summary>
        /// The currently open menu
        /// </summary>
        public Menu CurrentMenu { get; private set; }
        /// <summary>
        /// The transition in progress. Null if there is currently no transition.
        /// </summary>
        public MenuTransition CurrentTransition { get; private set; }

        private EventSystem _eventSystem;
        private Stack<Menu> _menuStack = new Stack<Menu>();

        private IEnumerator Start()
        {
            DontDestroyOnLoad(gameObject);
            _eventSystem = GetComponent<EventSystem>();
            if (_eventSystem == null)
            {
                Debug.LogError("Event system is not present on this object", this);
            }
            yield return null;
            if (_firstMenu != null)
                OpenMenuSingle(_firstMenu);
        }

        /// <summary>
        /// Opens a menu without tracking the previous ones. Also clears the current menu stack. Uses default transition.
        /// </summary>
        /// <param name="menu">The menu to open</param>
        public void OpenMenuSingle(Menu menu) => OpenMenuSingle(menu, MenuTransition.defaultTransition);
        /// <summary>
        /// Opens a menu without tracking the previous ones. Also clears the current menu stack.
        /// </summary>
        /// <param name="menu">The menu to open</param>
        /// <param name="transition">Transition animation used</param>
        public void OpenMenuSingle(Menu menu, MenuTransition transition)
        {
            if (CurrentTransition != null) ThrowAnimationInProgressException();
            _menuStack.Clear();

            OpenMenu(menu, transition);
        }
        /// <summary>
        /// Opens a menu and stacks the previous one for backtracking. Uses default transition.
        /// </summary>
        /// <param name="menu">The menu to open</param>
        public void OpenMenuTracked(Menu menu) => OpenMenuTracked(menu, MenuTransition.defaultTransition);
        /// <summary>
        /// Opens a menu and stacks the previous one for backtracking.
        /// </summary>
        /// <param name="menu">The menu to open</param>
        /// <param name="transition">Transition animation used</param>
        public void OpenMenuTracked(Menu menu, MenuTransition transition)
        {
            if (CurrentTransition != null) ThrowAnimationInProgressException();
            _menuStack.Push(CurrentMenu);

            OpenMenu(menu, transition);
        }

        private void OpenMenu(Menu menu, MenuTransition transition)
        {
            CurrentTransition = transition;

            transition.SetMenus(CurrentMenu, menu);
            transition.StartTransition(FinalizeTransition);
        }

        private void FinalizeTransition(Menu menu)
        {
            CurrentTransition = null;
            CurrentMenu = menu;
            GameObject selection = menu.Display(true);
            _eventSystem.SetSelectedGameObject(selection);
        }

        private void ThrowAnimationInProgressException()
        {
            throw new InvalidOperationException("Menu transition is already in progress.");
        }
    }
}
