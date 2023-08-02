using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory.UI
{
    /// <summary>
    /// Component to handle displaying menus
    /// </summary>
    public class MenuManager : MonoSingleton<MenuManager>
    {
        /// <summary>
        /// The currently open menu
        /// </summary>
        public Menu CurrentMenu { get; private set; }
        /// <summary>
        /// The transition in progress. Null if there is currently no transition.
        /// </summary>
        public MenuTransition CurrentTransition { get; private set; }

        private Stack<Menu> _menuStack = new Stack<Menu>();


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

        }
    }
}
