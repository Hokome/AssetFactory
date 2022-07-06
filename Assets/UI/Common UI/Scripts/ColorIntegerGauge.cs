using AssetFactory.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AssetFactory.UI
{
	public class ColorIntegerGauge : IntegerGauge
	{
		[SerializeField] private Color on;
		[SerializeField] private Color off;

		protected override void EnableImage(Image image, bool value)
		{
			image.color = value ? on : off;
		}
	}
}
