using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

//Originally from AssetFactory
namespace AssetFactory.UI
{
	public class TextDisplay : MonoBehaviour
	{
		[SerializeField] protected TMP_Text uiText;
		public int charactersPerFrame = 4;

		protected string currentText;
		protected Coroutine coroutine;

		void Start()
		{
			if (uiText == null)
				uiText = GetComponentInChildren<TMP_Text>();
		}

		public virtual void InstantDisplay(string text)
		{
			currentText = text;
			uiText.text = text;
		}
		public virtual void FinishText()
		{
			if (string.IsNullOrEmpty(currentText))
				return;
			else
			{
				if (coroutine != null)
					StopCoroutine(coroutine);
				InstantDisplay(currentText);
			}
		}
		public virtual void DisplayText(string text)
		{
			if (coroutine != null)
				StopCoroutine(coroutine);
			currentText = text;
			coroutine = StartCoroutine(Display(text));
		}

		protected virtual IEnumerator Display(string text)
		{
			int i = 0;
			while (i < text.Length)
			{
				uiText.text = text.Substring(0, Mathf.Min(i += charactersPerFrame, text.Length));
				yield return new WaitForFixedUpdate();
			}
		}
	}
}