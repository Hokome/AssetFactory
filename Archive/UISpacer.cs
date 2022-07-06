using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UISpacer : UIList
{
	[Header("Settings")]
	[SerializeField] float padding = 5f;
	[SerializeField] RectTransform.Axis axis = RectTransform.Axis.Vertical;
	[Tooltip("According to the axis")]
	[SerializeField] int direction = -1;
	[Space]
	[SerializeField] bool forceAnchoring = true;
	[SerializeField] Vector2 anchorMin = new Vector2(0.5f, 1.0f);
	[SerializeField] Vector2 anchorMax = new Vector2(0.5f, 1.0f);
	[Space]
	[SerializeField] bool forcePivot = true;
	[SerializeField] Vector2 pivot = new Vector2(0.5f, 1f);

	protected override void UpdateLayouts()
	{
		RectTransform rt = GetComponent<RectTransform>();
		if (forceAnchoring)
		{
			components.ForEach((t) =>
			{
				if (t == null)
					return;
				t.anchorMin = anchorMin;
				t.anchorMax = anchorMax;
			});
		}
		if (forcePivot)
		{
			components.ForEach((t) =>
			{
				if (t == null)
					return;
				t.pivot = pivot;
			});
		}

		float shift;

		if (axis == RectTransform.Axis.Vertical)
			shift = (rt.rect.height - padding * 2) / components.Count;
		else
			shift = (rt.rect.width - padding * 2) / components.Count;

		float multiplier = shift * direction;
		float offset;

		if (axis == RectTransform.Axis.Vertical)
		{
			offset = (padding + shift * (1 - pivot.y)) * direction;
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i] == null)
					break;
				components[i].anchoredPosition = new Vector2(0f, i * multiplier + offset);
			}
		}
		else
		{
			offset = (padding + shift * pivot.x) * direction;
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i] == null)
					break;
				components[i].anchoredPosition = new Vector2(i * multiplier + offset, 0f);
			}
		}
	}
}
