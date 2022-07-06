using System;
using UnityEngine;

namespace AssetFactory
{
	[Serializable]
	public class BufferedBool
	{
		public BufferedBool()
		{
			Value = false;
			bufferTime = -1f;
		}
		public BufferedBool(float duration) : this()
		{
			bufferDuration = duration;
		}


		public float bufferDuration  = 0.1f;
		private float bufferTime;
		private bool value;

		public bool Value
		{
			get
			{
				Update();
				return value;
			}
			private set
			{
				this.value = value;
			}
		}

		public void Buffer(bool value)
		{
			bufferTime = Time.time;
			Value = value;
		}

		public void Update()
		{
			if (bufferTime < 0f || Time.time - bufferTime > bufferDuration)
				Value = false;
		}

		public void Reset()
		{
			bufferTime = -1f;
			Update();
		}

		public override string ToString()
		{
			if (Value)
				return $"{value} ({bufferDuration - (Time.time - bufferTime)})";
			else
				return value.ToString();
		}

		public static implicit operator bool(BufferedBool b) => b.Value;
		public static implicit operator BufferedBool(float f)
		{
			return new BufferedBool(f);
		}
	}
}