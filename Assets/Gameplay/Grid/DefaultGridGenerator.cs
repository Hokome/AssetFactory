using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory
{
	public class DefaultGridGenerator : GridGeneratorBase<DefaultTile>
	{
		[SerializeField] private DefaultTile prefab;
		[SerializeField] private Vector2Int size;
		[SerializeField] private bool generateOnStart = true;

		public override Vector2Int Size => size;

		private void Start()
		{
			if (generateOnStart)
				Generate(Size);
		}

		public override void Generate(Vector2Int size)
		{
			Grid.Size = size;

			for (int y = 0; y < size.y; y++)
			{
				for (int x = 0; x < size.x; x++)
				{
					Grid.CreateTile(x, y, prefab);
				}
			}
		}
	}
}
