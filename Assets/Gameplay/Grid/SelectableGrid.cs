using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AssetFactory
{
	public class SelectableGrid<T> : GridBase<T> where T : SelectableTile
	{
		//[System.Serializable]
		//private struct SpriteInfo
		//{
		//	public Sprite sprite;
		//	public Color color;
		//	public SortingLayer layer;
		//	public int order;

		//	public SpriteRenderer Instantiate(string name, Transform parent)
		//	{
		//		GameObject obj = new(name);
		//		obj.transform.parent = parent;
		//		obj.transform.localScale = Vector3.one;
		//		SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
		//		renderer.sprite = sprite;
		//		renderer.color = color;
		//		renderer.sortingLayerID = layer.id;
		//		renderer.sortingOrder = order;
		//		return renderer;
		//	}
		//}

		//[SerializeField] private SpriteInfo selectionSprite;
		//[SerializeField] private SpriteInfo hoverSprite;

		[SerializeField] protected TileHighlight selectHighlight;
		[SerializeField] protected TileHighlight hoverHighlight;
		[SerializeField] private Camera cam;
		public Camera Camera
		{
			get
			{
				if (cam == null)
					cam = Camera.main;
				return cam;
			}
			private set => cam = value;
		}

		private T selection;
		private T hover;

		public T Selection
		{
			get => selection;
			protected set
			{
				if (selection != null)
					selection.Selected = false;
				selection = value;
				if (selection != null)
					selection.Selected = true;
				selectHighlight.Highlight(selection);
			}
		}
		public T Hover
		{
			get => hover;
			protected set
			{
				if (hover != null)
					hover.Selected = false;
				hover = value;
				if (hover != null)
					hover.Selected = true;
				hoverHighlight.Highlight(hover);
			}
		}

		protected virtual void Start()
		{
			hoverHighlight = Instantiate(hoverHighlight, transform);
			selectHighlight = Instantiate(selectHighlight, transform);
		}
		protected virtual void Update()
		{
			Vector2 mousePos = Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
			Vector2Int hoverPos = WorldToGrid(mousePos);

			//Debug.Log(hoverPos);

			Hover = this[hoverPos];

			if (Mouse.current.leftButton.wasPressedThisFrame)
				LeftClick(this[hoverPos]);
			if (Mouse.current.rightButton.wasPressedThisFrame)
				RightClick(this[hoverPos]);
		}
		public virtual void LeftClick(T tile)
		{
			Selection = tile;
		}
		public virtual void RightClick(T tile)
		{
			Selection = null;
		}
	}
}
