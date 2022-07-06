using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class UIList : MonoBehaviour
{
	[Header("References")]
	public List<RectTransform> components = new List<RectTransform>();

	bool InvalidList() => components == null || components.Count < 1;

	void Start()
	{
		if (InvalidList())
			return;
		UpdateLayouts();
	}

#if UNITY_EDITOR
	void OnValidate()
	{
		if (InvalidList())
			return;
		EditorApplication.delayCall += () => UpdateLayouts();
	}
	void OnRectTransformDimensionsChange()
	{
		if (InvalidList())
			return;
		EditorApplication.delayCall += () => UpdateLayouts();
	}
#endif
	protected abstract void UpdateLayouts();
}
