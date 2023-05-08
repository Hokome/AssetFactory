using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AssetFactory.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	[RequireComponent(typeof(CanvasGroup))]
	public class Countdown : MonoBehaviour
    {
		[SerializeField] private float unitDuration = 1f;
		[SerializeField] private float delay;
		[SerializeField] private float fade;

		private TextMeshProUGUI textUI;
		private CanvasGroup cGroup;

		private Coroutine coroutine;

		private void Awake()
		{
			textUI = GetComponent<TextMeshProUGUI>();
			cGroup = GetComponent<CanvasGroup>();
			cGroup.alpha = 0f;
		}

		public void StartCountdown(int duration, Action callback)
		{
			if (coroutine != null)
				StopCoroutine(coroutine);
			coroutine = StartCoroutine(CountdownCoroutine(duration, callback));
		}
		public void StopCountdown()
		{
			if (coroutine != null)
				StopCoroutine(coroutine);
			cGroup.alpha = 0f;
		}
		private IEnumerator CountdownCoroutine(int duration, Action callback)
		{
			LTDescr anim = null;
			for (int i = duration; i > 0; i--)
			{
				if (anim != null)
					LeanTween.cancel(anim.uniqueId);
				cGroup.alpha = 1f;
				cGroup.LeanAlpha(0f, fade).setDelay(delay);
				textUI.text = i.ToString();
				yield return new WaitForSeconds(unitDuration);
			}
			callback?.Invoke();
		}
	}
}
