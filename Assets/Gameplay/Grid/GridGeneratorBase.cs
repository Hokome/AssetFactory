using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory
{
	public abstract class GridGeneratorBase<T> : MonoBehaviour where T : TileBase
	{
		[SerializeField] private GridBase<T> grid;
		public GridBase<T> Grid
		{
			get
			{
				if (grid == null)
					grid = GetComponent<GridBase<T>>();
				return grid;
			}
			protected set => grid = value;
		}
		public abstract Vector2Int Size { get; }
		public abstract void Generate(Vector2Int size);
	}
}
