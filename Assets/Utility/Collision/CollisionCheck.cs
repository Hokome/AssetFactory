using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory
{
	public abstract class CollisionCheck : MonoBehaviour
	{
		[SerializeField] protected LayerMask mask;

		protected void Draw(Rect r, Vector2 missing, bool a, bool b, bool c)
		{
			Color color = a ? Color.green : b ? Color.cyan : c ? Color.gray : Color.red;

			Vector2 sw = r.min;
			Vector2 se = new Vector2(r.xMax, r.yMin);

			Vector2 ne = r.max;
			Vector2 nw = new Vector2(r.xMin, r.yMax);

			if (missing != Vector2.up)
				Debug.DrawLine(nw, ne, color);
			if (missing != Vector2.down)
				Debug.DrawLine(sw, se, color); 
			if (missing != Vector2.left)
				Debug.DrawLine(sw, nw, color);
			if (missing != Vector2.right)
				Debug.DrawLine(se, ne, color);
		}
	}
}
