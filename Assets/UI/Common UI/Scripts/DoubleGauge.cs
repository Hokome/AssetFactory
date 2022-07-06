using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Originally from AssetFactory
namespace AssetFactory.UI
{
	public class DoubleGauge : MonoBehaviour
	{
		[SerializeField] private float min = 0f;
		[SerializeField] private float max = 1f;
		[SerializeField] private float value = 0.2f;
		[SerializeField] private float subValue = 0.4f;
		[Header("Display")]
		[SerializeField] private Color mainColor;
		[SerializeField] private Color subIncreaseColor;
		[SerializeField] private Color subDecreaseColor;
		[SerializeField] private float animationTime;
		[Header("References")]
		[SerializeField] private Slider front;
		[SerializeField] private Slider back;
		[SerializeField] private Image frontFill;
		[SerializeField] private Image backFill;

		private Slider sub;

		private LTDescr anim;
		private bool isAnimating;
		private bool isIncreasing;

		public float Value
		{
			get => value;
			set
			{
				float newValue = Mathf.Clamp(value, min, max);
				if (newValue == this.value) return;
				this.value = newValue;

				bool increasing = this.value > subValue;
				if (isAnimating)
				{
					if (isIncreasing != increasing)
						ForceUpdate();
					LeanTween.cancel(anim.uniqueId);
				}
				if (increasing)
				{
					isIncreasing = true;
					back.value = value;
					sub = front;
					backFill.color = subIncreaseColor;
				}
				else
				{
					isIncreasing = false;
					front.value = value;
					sub = back;
					backFill.color = subDecreaseColor;
				}

				isAnimating = true;
				anim = LeanTween.value(gameObject, UpdateSub, subValue, value, animationTime)
					.setOnComplete(ForceUpdate);
			}
		}
		public float SubValue
		{
			get => subValue;
			set
			{
				subValue = value;
				sub.value = value;
			}
		}

		public float Min
		{
			get => min;
			set
			{
				min = value;
				front.minValue = value;
				back.minValue = value;
			}
		}
		public float Max
		{
			get => max;
			set
			{
				max = value;
				front.maxValue = value;
				back.maxValue = value;
			}
		}

		public void ForceUpdate()
		{
			if (isAnimating)
				LeanTween.cancel(anim.uniqueId);
			isAnimating = false;

			subValue = value;
			front.value = value;
			back.value = value;
		}

		private void Start() => Init();
		private void OnValidate() => Init();
		private void Init()
		{
			if (front != null)
				front.value = value;
			if (back != null)
				back.value = subValue;
			
			Min = min;
			Max = max;
			
			if (frontFill != null)
				frontFill.color = mainColor;
			if (backFill != null)
				backFill.color = subDecreaseColor;
		}

		private void UpdateSub(float val)
		{
			SubValue = val;
		}

		//Old code
		//private Coroutine anim;
		//private bool recalculationNeeded;
		//private float increment;
		//private IEnumerator Animate()
		//{
		//Recalculation:
		//	float increment = value - subValue / animationTime;
		//	if (!MathEx.SameSign(increment, this.increment))
		//		ForceUpdate();
		//	this.increment = increment;
		//	recalculationNeeded = false;
		//	if (value > subValue)
		//	{
		//		back.value = value;
		//		sub = front;
		//		backFill.color = subIncreaseColor;
		//		while (subValue < value)
		//		{
		//			SubValue += increment * Time.deltaTime;
		//			yield return null;
		//			if (recalculationNeeded)
		//				goto Recalculation;
		//		}
		//	}
		//	else if (value < subValue)
		//	{
		//		front.value = value;
		//		sub = back;
		//		backFill.color = subDecreaseColor;
		//		while (subValue > value)
		//		{
		//			SubValue += increment * Time.deltaTime;
		//			yield return null;
		//			if (recalculationNeeded)
		//				goto Recalculation;
		//		}
		//	}
		//	anim = null;
		//}
	}
}
