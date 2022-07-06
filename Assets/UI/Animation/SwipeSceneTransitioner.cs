using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory.UI
{
    public class SwipeSceneTransitioner : SceneTransitioner
    {
		[SerializeField] private float swipeDuration;
		[SerializeField] private RectTransform swipeObject;

		protected override LTDescr StartAnimation()
		{
			return null;
			////Setting the start position
			//Utility.SetAnchorX(swipeObject, 0f);
			//swipeObject.pivot = new Vector2(1f, 0.5f);
			//swipeObject.anchoredPosition = Vector2.zero;
			//Vector2 pos = swipeObject.position;
			//Debug.Break();
			////Changing anchors and animating
			//Utility.SetAnchorX(swipeObject, 1f);
			//swipeObject.position = pos;
			//return LeanTween.move(swipeObject.gameObject, Vector2.zero, swipeDuration);
		}
		protected override void EndAnimation()
		{
			////Setting the start position
			//Utility.SetAnchorX(swipeObject, 0f);
			//swipeObject.pivot = new Vector2(1f, 0.5f);
			//swipeObject.anchoredPosition = Vector2.zero;
			//Vector2 pos = swipeObject.position;

			////Changing anchors and animating
			//Utility.SetAnchorX(swipeObject, 1f);
			//swipeObject.position = pos;
			//LeanTween.move(swipeObject.gameObject, Vector2.zero, swipeDuration);
		}
	}
}
