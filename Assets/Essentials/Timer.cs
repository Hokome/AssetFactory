using System;
using System.Collections;
using UnityEngine;

namespace AssetFactory
{
	[Serializable]
    public struct Timer
    {
		public float duration;
		public float StartTime { get; private set; }
		public Timer(float duration)
		{
			this.duration = duration;
			StartTime = float.NegativeInfinity;
			LastCoroutine = null;
		}

		public float TimeElapsed => Time.time - StartTime;
		public float TimeLeft => duration - TimeElapsed;
		public float TimeElapsedRatio => TimeElapsed / duration;
		public float TimeLeftRatio => TimeLeft / duration;

		public bool HasStarted => StartTime > 0f;
		public bool HasFinished => TimeElapsed >= duration;

		public void Start() => StartTime = Time.time;

		public Coroutine LastCoroutine { get; private set; }

		public Coroutine StartCoroutine(MonoBehaviour mb) => StartCoroutine(mb, null);

		public Coroutine StartCoroutine(MonoBehaviour mb, Action callback)
		{
			LastCoroutine = mb.StartCoroutine(Coroutine(callback));
			return LastCoroutine;
		}

		//public void StopCoroutine(MonoBehaviour mb)
		//{
		//	if (LastCoroutine != null)
		//		mb.StopCoroutine(LastCoroutine);
		//}

		public void Reset() => StartTime = float.NegativeInfinity;

		private IEnumerator Coroutine(Action callback)
		{
			yield return this;
			callback?.Invoke();
		}

		public static implicit operator WaitForSeconds(Timer t) => new(t.duration);
		public static explicit operator WaitForSecondsRealtime(Timer t) => new(t.duration);
    }
}
