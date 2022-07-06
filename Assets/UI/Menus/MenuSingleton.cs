using AssetFactory.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory.UI
{
	public class MenuSingleton<T> : MenuManager where T : MenuSingleton<T>
	{
		private static T inst;
		public static T Inst
		{
			get
			{
				if (Exists) return inst;
				Debug.LogError($"No instance of the singleton type {typeof(T)} has been registered.");
				return null;
			}
		}
		public static bool Exists => inst != null;

		protected virtual void Awake()
		{
			if (inst == null)
			{
				inst = (T)this;
			}
			else
			{
				Debug.LogWarning($"Singleton for {typeof(T)} already exists.");
				Destroy(gameObject);
			}
		}
	}
}
