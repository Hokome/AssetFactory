using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class IntGauge : MonoBehaviour
{
	[SerializeField] int value;
	public int Value { get => value; set
		{
			this.value = Mathf.Min(value, max);
			Display();
		} }
	[SerializeField] int max;
	public int Max { get => max; set 
		{ 
			max = value; 
			GenerateSprites();
		} }

	[Header("Display")]
	[SerializeField] float padding;
	[SerializeField] bool reverse = false;
	[Space]
	[SerializeField] Image prefab;
	[Space]
	[SerializeField] Sprite enabledSprite;
	[SerializeField] Sprite disabledSprite;
	[Space]
	[SerializeField] Color enabledColor = Color.white; 
	[SerializeField] Color disabledColor = Color.white;

	Image[] s;

	void Start()
	{
		GenerateSprites();
		CleanTransforms();
	}

	public void Display()
	{
		int i = 0;
		while (i < value)
		{
			DisplayImage(s[i], enabledColor, enabledSprite);
			i++;
		}
		while (i < max)
		{
			DisplayImage(s[i], disabledColor, disabledSprite);
			i++;
		}
	}

	void DisplayImage(Image i, Color c, Sprite s)
	{
		if (s != null)
			i.sprite = s;
		i.color = c;
	}

	Queue<GameObject> toClean = new Queue<GameObject>();

	public void CleanTransforms()
	{
		for (int i = transform.childCount; i > 0; i--)
		{
#if UNITY_EDITOR
			if (Application.isPlaying)
				Destroy(transform.GetChild(0).gameObject);
			else
				DestroyImmediate(transform.GetChild(0).gameObject);
#else
			Destroy(transform.GetChild(0).gameObject);
#endif
		}
	}

#if UNITY_EDITOR
	void OnValidate()
	{
		EditorApplication.delayCall += () => GenerateSprites();
	}
#endif

	void GenerateSprites()
	{
		CleanTransforms();
		if (prefab == null)
			return;
		s = new Image[max];

		float width;
		Vector2 v;
		if (reverse)
		{
			width = -(prefab.transform as RectTransform).rect.width - padding;
			v = new Vector2(1f, .5f);
		}
		else
		{
			width = (prefab.transform as RectTransform).rect.width + padding;
			v = new Vector2(0f, .5f);
		}

		for (int i = 0; i < max; i++)
		{
#if UNITY_EDITOR
			s[i] = PrefabUtility.InstantiatePrefab(prefab, transform) as Image;
#else
			s[i] = Instantiate(prefab, transform);
#endif
			RectTransform t = s[i].GetComponent<RectTransform>();

			t.anchorMax = t.anchorMin = t.pivot = v;
			t.anchoredPosition = Vector2.right * i * width;
		}
		Value = Value;
	}
}