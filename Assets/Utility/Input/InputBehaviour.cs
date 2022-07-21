using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;
using InputHandler = System.Collections.Generic.KeyValuePair<UnityEngine.InputSystem.InputAction, AssetFactory.InputCallback>;

namespace AssetFactory
{
	public delegate void InputCallback(InputContext ctx);
	[RequireComponent(typeof(PlayerInput))]
	public abstract class InputBehaviour : MonoBehaviour
	{
		private PlayerInput input;
		/// <summary>
		/// <see cref="PlayerInput"/> component attached to the object or provided by the child class. Initializes handlers when set.
		/// </summary>
		protected PlayerInput Input
		{
			get => input;
			set
			{
				if (input != null)
					input.onActionTriggered -= ReadInput;
				input = value;
				if (input == null) return;
				Input.onActionTriggered += ReadInput;
				Input.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
				actionHandlers = new Dictionary<InputAction, InputCallback>(GetHandlers());
			}
		}
		private Dictionary<InputAction, InputCallback> actionHandlers;

		protected virtual void Awake()
		{
			Input = GetComponent<PlayerInput>();
		}

		private void ReadInput(InputContext ctx)
		{
			if (!isActiveAndEnabled) return;
			if (actionHandlers.TryGetValue(ctx.action, out InputCallback callback))
				callback(ctx);
		}

		/// <summary>
		/// When overriding, you can yield return every handler (key-value pair) and use presets such as <see cref="GetInput(string, InputCallback)"/>
		/// </summary>
		protected virtual IEnumerable<InputHandler> GetHandlers() { yield break; }

		#region Presets
		/// <summary>
		/// Finds the actions with provided name and pairs it with the provided callback. 
		/// See <see cref="GetInput(string, InputCallback)"/> and <see cref="GetButton(string, Action)"/> for shortcuts.
		/// </summary>
		/// <param name="name">Name of the action</param>
		protected InputHandler GetInput(string name, InputCallback callback)
		{
			return new(Input.actions.FindAction(name), callback);
		}
		/// <summary>
		/// Same as <see cref="GetInput(string, InputCallback)"/>, but automatically reads the value provided by the input.
		/// </summary>
		/// <typeparam name="TValue">Type of the value</typeparam>
		/// <param name="name">Name of the action</param>
		/// <param name="callback">Use the value passed through the action with this callback.</param>
		protected InputHandler GetValue<TValue>(string name, Action<TValue> callback) where TValue : struct
		{
			return GetInput(name, ctx => callback(ctx.ReadValue<TValue>()));
		}
		/// <summary>
		/// Same as <see cref="GetInput(string, InputCallback)"/>, but automatically invokes an action if the input was performed.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		protected InputHandler GetButton(string name, Action callback)
		{
			return GetInput(name, ctx =>
			{
				if (ctx.performed)
					callback();
			});
		}
		/// <summary>
		 /// Same as <see cref="GetInput(string, InputCallback)"/>, but passes a boolean that is true if the action was performed and false if it was cancelled.
		 /// </summary>
		 /// <param name="name"></param>
		 /// <param name="callback"></param>
		 /// <returns></returns>
		protected InputHandler GetButton(string name, Action<bool> callback)
		{
			return GetInput(name, ctx =>
			{
				if (ctx.performed)
					callback(true);
				else if (ctx.canceled)
					callback(false);
			});
		}
		#endregion
	}
}
