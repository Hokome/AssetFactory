using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DynamicLanguageText : MonoBehaviour
{
	[SerializeField] TMP_Text txt;
	[SerializeField] string key;

	IEnumerator Start()
	{
		if (txt == null)
		{
			if (TryGetComponent(out TMP_Text ttxt))
			{
				txt = ttxt;
				if (key == null)
				{
					key = ttxt.text;
				}
			}
			else
			{
				Debug.LogError("No text component found");
				Destroy(this);
				yield break;
			}
		}
		else
		{
			if (key == null)
			{
				key = txt.text;
			}
		}

		yield return new WaitWhile(() => Language.current == null);
		try
		{
			txt.text = Language.current.pairs[key];
		}
		catch (KeyNotFoundException)
		{
			Debug.LogError("The key was not found in the current language file");
		}
	}
}
