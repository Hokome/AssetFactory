using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

//Originally from AssetFactory
namespace AssetFactory.Legacy.UI
{
    public abstract class MenuManager : MonoBehaviour
    {
        [SerializeField] protected Menu main;
        [SerializeField] protected bool lockCursorIfDisabled;

        protected Menu currentMenu;
        protected Stack<Menu> navigationStack = new Stack<Menu>();
        protected bool backEnabled;

        private InputSystemUIInputModule inputModule;

        public Menu CurrentMenu
        {
            get => currentMenu;
            set
            {
                value.Display(true);
                currentMenu = value;
            }
        }
        public bool BackEnabled
        {
            get => backEnabled;
            set
            {
                backEnabled = value;
                if (value)
                {
                    InputModule.cancel.action.performed += _ => Back();
                }
                else
                {
                    InputModule.cancel.action.performed -= _ => Back();
                }
            }
        }
        public InputSystemUIInputModule InputModule
        {
            get
            {
                if (inputModule == null)
                    inputModule = GetComponent<InputSystemUIInputModule>();
                return inputModule;
            }
        }

        public event Action OnBack = delegate { };
        protected void CallOnBack() => OnBack();

        /// <summary>
        /// Selects and displays a menu. To allow backtracking, use <see cref="StackSelect(Menu)"/> instead.
        /// Clears the navigation stack.
        /// </summary>
        /// <param name="menu"></param>
        public virtual void SoleSelect(Menu menu)
        {
            navigationStack.Clear();

            if (!isActiveAndEnabled) return;
            if (CurrentMenu != null) CurrentMenu.Display(false);
            CurrentMenu = menu;
        }
        /// <summary>
        /// Selects and displays a menu and adds it to the navigation stack for backtracking.
        /// </summary>
        /// <param name="menu"></param>
        public virtual void StackSelect(Menu menu)
        {
            if (!isActiveAndEnabled) return;

            navigationStack.Push(currentMenu);

            if (currentMenu != null)
                currentMenu.Display(false);
            CurrentMenu = menu;
        }
        public void ToMain()
        {
            SoleSelect(main);
        }

        public virtual void Back()
        {
            if (!backEnabled || navigationStack.Count <= 0)
                return;
            currentMenu.Display(false);
            CurrentMenu = navigationStack.Pop();
            OnBack();
        }
        public virtual void SilentBack()
        {
            if (!backEnabled || navigationStack.Count <= 0)
                return;
            currentMenu.Display(false);
            CurrentMenu = navigationStack.Pop();
        }
        protected void LockCursor(bool value)
        {
            Cursor.visible = !value;
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        }

        protected virtual void Start()
        {
            navigationStack = new Stack<Menu>();
        }
        protected virtual void OnEnable()
        {
            BackEnabled = true;
        }
        protected virtual void OnDisable()
        {
            BackEnabled = false;
        }
    }
}
