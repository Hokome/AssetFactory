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
		//{
		//	LTDescr ltdescr;
		//	switch (animationType)
		//	{
		//		case AnimationType.Fade:
		//			ltdescr = LeanTween.alphaCanvas(cg, inverted ? 0f : 1f, duration).setEase(easeType);
		//			break;
		//		case AnimationType.Scale:
		//			ltdescr = LeanTween.scale(gameObject, inverted ? Vector3.zero : Vector3.one, duration).setEase(easeType);
		//			break;
		//		default:
		//			Debug.LogError("Animation does not exist");
		//			return null;
		//	}
		//	ltdescr.setEase(easeType);
		//	return ltdescr.setOnComplete(OnComplete.Invoke);
		//}
		public void DestroyMe() => Destroy(gameObject);
	}
}