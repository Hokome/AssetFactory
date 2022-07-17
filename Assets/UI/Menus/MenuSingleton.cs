using AssetFactory.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory.UI
{
	public class MenuSingleton<T> : MenuManager, ISingleton<T> where T : MenuSingleton<T>
	{
		public static T Inst => ISingleton<T>.Inst;
		public static bool Exists => ISingleton<T>.Exists;
		protected virtual void Awake() => ISingleton<T>.Inst = (T)this;
	}
}
