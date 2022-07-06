using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory.UI.Tweening
{
    public class FadeTweener : UITweener
    {
		public override LTDescr Animate(bool forward)
		{
			return LeanTween.alphaCanvas(cGroup, forward ? 1f : 0f, duration)
				.setEase(easeType)
				.setIgnoreTimeScale(ignoreTimeScale);
		}
	}
}
