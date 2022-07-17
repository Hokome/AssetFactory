using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory
{
	public abstract class GridBase<T> : MonoBehaviour where T : TileBase
	{
		[SerializeField] private Vector2 pivot = Vector2.zero;
		private T[,] tiles;

		public Vector2 Pivot => pivot;
		public Vector2 LocalPivot => throw new NotImplementedException();
		public Vector2 HalfSize => (Vector2)Size * 0.5f;
		public Vector2 WorldPivot => ClipToWorld(pivot);

		/// <summary>
		/// Recreates the array and copies its content when assigned.
		/// </summary>
		public Vector2Int Size
		{
			get => new(tiles.GetLength(0), tiles.GetLength(1));
			set
			{
				T[,] array = new T[value.x, value.y];
				if (tiles != null)
				{
					for (int y = 0; y < tiles.GetLength(1) && y < value.y; y++)
					{
						for (int x = 0; x < tiles.GetLength(0) && x < value.y; x++)
						{
							array[x, y] = tiles[x, y];
						}
					}
				}
				tiles = array;
			}
		}


		public T this[int x, int y] => tiles[x, y];
		public T this[Vector2Int v]
		{
			get
			{
				if (!InRange(v)) return null;
				return tiles[v.x, v.y];
			}
		}
		public T this[int index] => tiles[index % Size.x, index / Size.y];

//#if UNITY_EDITOR
//		private void OnValidate()
//		{
//			if (!UnityEditor.EditorApplication.isPlaying) return;
//		}
//#endif
		
		public T CreateTile(int x, int y, T prefab) => CreateTile(new(x, y), prefab);
		public virtual T CreateTile(Vector2Int position, T prefab)
		{
			Vector2 localPosition = position - LocalPivot;
			T tile = Instantiate(prefab, localPosition, Quaternion.identity, transform);
			tile.Initialize(position);
			return tile;
		}
		public Vector2 ClipToLocal(Vector2 position) => (position * HalfSize) - HalfSize;
		public Vector2 ClipToWorld(Vector2 position) => transform.TransformPoint(ClipToLocal(position));

		public Vector2 GridToWorld(Vector2Int position) => GridToWorld(position, MathEx.HalfVector2);
		public Vector2 GridToWorld(Vector2Int position, Vector2 pivot)
		{
			Vector2 worldPosition = transform.TransformPoint(position - LocalPivot);
			return worldPosition + ClipToWorld(pivot);
		}
		public Vector2Int WorldToGrid(Vector2 position)
		{
			position = transform.InverseTransformPoint(position);
			position -= WorldPivot;
			return Vector2Int.FloorToInt(position);
		}
		public bool InRange(Vector2Int position) 
			=> position.x >= 0 && position.x < Size.x
			&& position.y >= 0 && position.y < Size.y;

		public IEnumerable<T> GetRow(int row)
		{
			for (int i = 0; i < Size.x; i++)
			{
				yield return tiles[i, row];
			}
		}
		public IEnumerable<T> GetColumn(int column)
		{
			for (int i = 0; i < Size.y; i++)
			{
				yield return tiles[column, i];
			}
		}
		public IEnumerable<T> AsEnumerable()
		{
			for (int y = 0; y < Size.y; y++)
			{
				for (int x = 0; x < Size.x; x++)
				{
					yield return tiles[x, y];
				}
			}
		}
	}
}
