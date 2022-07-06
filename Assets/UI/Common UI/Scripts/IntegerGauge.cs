using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AssetFactory.UI
{
	[RequireComponent(typeof(LayoutGroup))]
	public class IntegerGauge : MonoBehaviour
	{
		[SerializeField] private int value;
		[SerializeField] private int max;
		[SerializeField] private Image prefab;
		
		private List<Image> images;
		//private LayoutGroup lg;

		public int Value
		{
			get => value;
			set
			{
				this.value = Mathf.Min(value, max);
				UpdateDisplay();
			}
		}
		public int Max
		{
			get => max;
			set
			{
				max = value;
				Generate();
				Value = Value;
			}
		}

		//private LayoutGroup Group
		//{
		//	get
		//	{
		//		if (lg == null)
		//			lg = GetComponent<LayoutGroup>();
		//		return lg;
		//	}
		//}

		private void Awake()
		{
			Generate();
		}

		private void Generate()
		{
			if (images != null)
			{
				images.ForEach(i => Destroy(i.gameObject));
			}
			images = new List<Image>(max);
			for (int i = 0; i < max; i++)
			{
				images.Add(Instantiate(prefab, transform));
			}
			UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			int i = 0;
			while (i < value)
			{
				EnableImage(images[i], true);
				i++;
			}
			while (i < max)
			{
				EnableImage(images[i], false);
				i++;
			}
		}
		protected virtual void EnableImage(Image image, bool value)
		{
			image.gameObject.SetActive(value);
		}
	}
}
