using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Originally from AssetFactory
namespace AssetFactory.UI
{
	public class FadeSceneTransitioner : SceneTransitioner
	{
		[SerializeField] private float fadeDuration = 1f;
		[SerializeField] CanvasGroup loadingScreen;

		protected override LTDescr StartAnimation()
		{
			loadingScreen.blocksRaycasts = true;
			return LeanTween.alphaCanvas(loadingScreen, 1f, fadeDuration).setIgnoreTimeScale(true);
		}
		protected override void EndAnimation()
		{
			LeanTween.alphaCanvas(loadingScreen, 0f, fadeDuration).setIgnoreTimeScale(true)
				.setOnComplete(() => loadingScreen.blocksRaycasts = false);
		}
	}
}