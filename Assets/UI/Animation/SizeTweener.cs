using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Originally from AssetFactory
namespace AssetFactory.UI.Tweening
{
    public class SizeTweener : UITweener,
		IPointerEnterHandler, IPointerExitHandler, 
		IPointerUpHandler, IPointerDownHandler,
		ISelectHandler, IDeselectHandler
    {
		[SerializeField] private float scale = 1.1f;
		[SerializeField] private bool animateOnHover = true;
		[SerializeField] private bool animateOnSelect = true;
		[SerializeField] private bool animateOnClick = false;

		public override LTDescr Animate(bool forward)
		{
			return LeanTween.scale(gameObject, forward ? Vector3.one * scale : Vector3.one, duration)
				.setEase(easeType)
				.setIgnoreTimeScale(ignoreTimeScale);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (animateOnClick && enabled)
				Animate(false);
		}
		public void OnPointerDown(PointerEventData eventData)
		{
			if (animateOnClick && enabled)
				Animate(true);
		}
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (animateOnHover && enabled)
				Animate(true);
		}
		public void OnPointerExit(PointerEventData eventData)
		{
			if (animateOnHover && enabled)
				Animate(false);
		}

		public void OnSelect(BaseEventData eventData)
		{
			if (animateOnSelect && enabled)
				Animate(true);
		}

		public void OnDeselect(BaseEventData eventData)
		{
			if (animateOnSelect && enabled)
				Animate(false);
		}
	}
}
