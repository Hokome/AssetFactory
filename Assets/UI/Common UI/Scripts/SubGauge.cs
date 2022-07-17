using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AssetFactory
{
	public class SubGauge : MonoBehaviour
	{
		[SerializeField] private Slider slider;
		[SerializeField] private Image fill;
		public Image Fill
		{
			get
			{
				if (fill == null)
					fill = GetComponentInChildren<Image>();
				return fill;
			}
			private set => fill = value;
		}
		public Slider Slider
		{
			get
			{
				if (slider == null)
					slider = GetComponent<Slider>();
				return slider;
			}
			set => slider = value;
		}
		public float Value
		{
			get => slider.value;
			set => slider.value = value;
		}
		
		public Color Color
		{
			get => Fill.color;
			set => Fill.color = value;
		}
		public float Max
		{
			get => Slider.maxValue;
			set => Slider.maxValue = value;
		}
		public float Min
		{
			get => Slider.minValue;
			set => Slider.minValue = value;
		}
	}
}
