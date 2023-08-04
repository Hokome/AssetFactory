using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory.UI
{
    /// <summary>
    /// Handles transition animation between menus
    /// </summary>
    public class MenuTransition
    {
        public static readonly MenuTransition defaultTransition = new MenuTransition();

        /// <summary>
        /// Sets the menus of the transition
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="next"></param>
        public void SetMenus(Menu previous, Menu next)
        {
            Previous = previous;
            Next = next;
        }

        protected Menu Previous { get; private set; }
        protected Menu Next { get; private set; }

        private Action<Menu> callback;

        public void StartTransition(Action<Menu> callback)
        {
            if (Next == null) throw new NullReferenceException("The destination menu of the transition is null");
            this.callback = callback;
            StartTransition();
        }
        protected virtual void StartTransition()
        {
            if (Previous != null)
                Previous.Display(false);
            EndTransition();
        }
        public virtual void CancelTransition()
        {
            EndTransition();
        }
        protected void EndTransition()
        {
            callback(Next);
            SetMenus(null, null);
        }
    }
}
