using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetFactory.UI;

//Orignially from AssetFactory
namespace AssetFactory
{
	public class ErrorPrompt : MonoBehaviour
	{
		public static ErrorPrompt Inst { get; private set; }
		public bool IsShowing { get; private set; } = false;

		[SerializeField] float messageDuration = 0f;
		[Space]
		[SerializeField] TextDisplay display;
		[SerializeField] private CanvasGroup group;

		private void Awake()
		{
			if (Inst == null)
			{
				Inst = this;
			}
			else
			{
				Destroy(gameObject);
			}
		}

		Coroutine coroutine;
		public void DisplayMessage(string message)
		{
			if (IsShowing)
				return;
			coroutine = StartCoroutine(ShowMessage(message));
		}
		public void HideMessage()
		{
			if (coroutine != null) StopCoroutine(coroutine);
			group.alpha = 0f;
			group.interactable = false;
			group.blocksRaycasts = false;
			IsShowing = false;
		}

		IEnumerator ShowMessage(string message)
		{
			display.InstantDisplay(message);

			group.alpha = 1f;
			group.interactable = true;
			group.blocksRaycasts = true;
			IsShowing = true;

			if (messageDuration < 0f)
			{
				yield return new WaitForSeconds(messageDuration);
			}
			else
			{
				yield break;
			}

			HideMessage();
			yield break;
		}
	}
}