using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory
{
	public class TileHighlight : MonoBehaviour
	{
		public virtual void Highlight(TileBase tile) 
		{
			if (tile == null)
			{
				gameObject.SetActive(false);
				return;
			}
			gameObject.SetActive(true);
			transform.position = tile.transform.position;
		}
	}
}
