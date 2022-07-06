using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gauge
{
	Slider d;
	public Slider Display
	{
		get => d; set
		{
			d = value;
			FullUpdate();
		}
	}

	float v;
	public float Value 
	{ 
		get => v; set
		{
			v = Mathf.Clamp(value, 0f, max);
			HalfUpdate();
		} 
	}

	float max;
	public float Max
	{
		get => max; set
		{
			max = value;
			Value = v;
			FullUpdate();
		}
	}

	public float Ratio { get => v / max; }
	public bool IsFull { get => v >= max; }

	void HalfUpdate()
	{
		if (d == null)
			return;
		d.value = v;
	}

	void FullUpdate()
	{
		if (d == null)
			return;
		d.maxValue = max;
		HalfUpdate();
	}
}
