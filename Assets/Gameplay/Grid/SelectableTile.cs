using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory
{
	public class SelectableTile : TileBase
	{
		private bool selected;
		public virtual bool Selected
		{
			get => selected;
			set
			{
				selected = value;
				if (value)
					OnSelect();
				else
					OnDeselect();
			}
		}
		private bool hovered;
		public virtual bool Hovered
		{
			get => hovered;
			set
			{
				hovered = value;
				if (value)
					OnHover();
				else
					OnUnhover();
			}
		}


		protected virtual void OnSelect() { }
		protected virtual void OnDeselect() { }
		protected virtual void OnHover() { }
		protected virtual void OnUnhover() { }
	}
}
