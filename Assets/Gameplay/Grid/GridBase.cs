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

		public Vector2 Pivot
		{
			get => pivot;
			set
			{
				pivot = value;
				RecalculatePivotCache();
			}
		}
		public Vector2 LocalPivot { get; private set; }
		private Vector2 GridToWorldCache { get; set; }
		private Vector2 WorldToGridCache { get; set; }
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
				RecalculatePivotCache();
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

		protected virtual void Update()
		{
			if (transform.hasChanged)
				RecalculatePivotCache();
		}

		public T CreateTile(int x, int y, T prefab) => CreateTile(new(x, y), prefab);
		public virtual T CreateTile(Vector2Int position, T prefab)
		{
			T tile = Instantiate(prefab, transform);
			tile.transform.position = GridToWorld(position);
			tile.Initialize(position);
			tiles[position.x, position.y] = tile;
			return tile;
		}

		public Vector2 ClipToLocal(Vector2 clip) => ((clip + Vector2.one) / 2) * Size;
		public Vector2 GridToWorld(Vector2Int position)
		{
			Vector2 tpos = transform.TransformPoint(position + MathEx.HalfVector2);
			return tpos + GridToWorldCache;
		}
		public Vector2Int WorldToGrid(Vector2 position)
		{
			Vector2 c = (Vector2)transform.InverseTransformPoint(position) + WorldToGridCache;
			return Vector2Int.FloorToInt(c);
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

		private void RecalculatePivotCache()
		{
			LocalPivot = ClipToLocal(pivot);
			GridToWorldCache = transform.position - transform.TransformPoint(LocalPivot);
			WorldToGridCache = transform.position + transform.InverseTransformPoint(LocalPivot);
		}
	}
}
