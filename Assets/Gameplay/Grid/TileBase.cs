using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory
{
	public class TileBase : MonoBehaviour
	{
		public Vector2Int GridPosition { get; private set; }

		public void Initialize(int x, int y) => new Vector2Int(x, y);
		public virtual void Initialize(Vector2Int position)
		{
			GridPosition = position;
			name = $"[{position.x}, {position.y}]";
		}
	}
}
