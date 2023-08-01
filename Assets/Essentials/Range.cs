using System;
using UnityEngine;

namespace AssetFactory
{
	[Serializable]
	public struct Range
	{
		public static Range Zero => new(0, 0);
		public static Range One => new(0, 1);
		public static Range OneNegative => new(-1, 1);
		public float min, max;

		public Range(float max) : this(0f, max) { }
		public Range(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		public float Clamp(float value) => Mathf.Clamp(value, min, max);
		public float Random() => UnityEngine.Random.Range(min, max);
	}

	[Serializable]
	public struct RangeInt
	{
		public static RangeInt Zero => new(0, 0);
		public static RangeInt One => new (0, 1);
		public int min, max;

		public RangeInt(int max) : this(0, max) { }
		public RangeInt(int min, int max)
		{
			this.min = min;
			this.max = max;
		}

		public int Clamp(int value) => Mathf.Clamp(value, min, max);
		public int Random() => UnityEngine.Random.Range(min, max);
	}
}
