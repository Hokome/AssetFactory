using UnityEngine;

namespace AssetFactory
{
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton<T> where T : MonoSingleton<T>
    {
		public static T Inst => ISingleton<T>.Inst;
		public static bool Exists => ISingleton<T>.Exists;
		protected virtual void Awake() => ISingleton<T>.Inst = (T)this;
	}
}
