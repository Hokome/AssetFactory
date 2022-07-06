using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UILinearContainer : UIList
{
	[Header("Settings")]
	[SerializeField] Vector2 padding = Vector2.one * 5f;
	[SerializeField] float spacing = 5f;
	[SerializeField] RectTransform.Axis axis = RectTransform.Axis.Vertical;

	protected override void UpdateLayouts() 
	{
		RectTransform rt = GetComponent<RectTransform>();
		float paddedSize;
		if (axis == RectTransform.Axis.Horizontal)
			paddedSize = 1f - (padding.x * 2f / rt.rect.width);
		else
			paddedSize = 1f - (padding.y * 2f / rt.rect.height);

		float increment = paddedSize / components.Count;
		float offset = (1f - paddedSize) / 2f;
		float hspace = spacing / 2f;

		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] == null)
				continue;
			RectTransform t = components[i];

			Vector2 omin = Vector2.zero;
			Vector2 omax = Vector2.zero;

			if (axis == RectTransform.Axis.Vertical)
			{
				t.anchorMin = new Vector2(0f, 1 - (i + 1) * increment - offset);
				t.anchorMax = new Vector2(1f, 1 - i * increment - offset);

				omin.x = padding.x;
				omax.x = -padding.x;

				if (i == 0)
				{
					omax.y = -padding.y;
				}
				else if (i < components.Count - 1)
				{
					omin.y = spacing;
				}
				else
				{
					omin.y = padding.y;
				}
			}
			else
			{
				t.anchorMin = new Vector2(i * increment + offset, 0f);
				t.anchorMax = new Vector2((i + 1) * increment + offset, 1f);

				omin.y = padding.y;
				omax.y = -padding.y;

				if (i == 0)
					omin.x = padding.x;
				if (i < components.Count - 1)
					omax.x = -spacing;
				else
					omax.x = -padding.x;
			}
			t.offsetMin = omin;
			t.offsetMax = omax;
		}
	}
}