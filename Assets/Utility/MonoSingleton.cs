using UnityEngine;

namespace AssetFactory
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
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
