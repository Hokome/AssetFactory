using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Originally from AssetFactory
namespace AssetFactory.UI
{
	//public enum AnimationType
	//{
	//	Fade,
	//	Scale
	//}
	public abstract class UITweener : MonoBehaviour
	{
		//[SerializeField] private AnimationType animationType;
		[SerializeField] protected LeanTweenType easeType = LeanTweenType.linear;
		[SerializeField] protected float duration = 0.1f;
		[SerializeField] protected bool ignoreTimeScale;
		[Space]
		public bool forward = true;

		protected CanvasGroup cGroup;

		private void Awake()
		{
			cGroup = GetComponent<CanvasGroup>();
		}

		public virtual LTDescr Animate() => Animate(forward);
		public virtual LTDescr Animate(bool forward) => null;
	}
}