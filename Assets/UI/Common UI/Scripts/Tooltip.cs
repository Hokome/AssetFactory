using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//Originally from AssetFactory
namespace AssetFactory.UI
{
	public class Tooltip : MonoBehaviour
	{
		[SerializeField] private int characterWrapLimit;
		[Space]
		[SerializeField] private TextMeshProUGUI headerUGUI;
		[SerializeField] private TextMeshProUGUI contentUGUI;
		[SerializeField] private UITweener tweener;
		[SerializeField] private LayoutElement layout;

		public UITweener Tweener => tweener;

		private void Update()
		{
			UpdatePosition();
		}

		private void UpdatePosition()
		{
			Vector2 point = Mouse.current.position.ReadValue();
			RectTransform rt = transform as RectTransform;

			//Clip coordinates to pivot
			float pivotX = point.x / Screen.width;
			float pivotY = point.y / Screen.height;

			//Snapping to closest corner
			pivotX = pivotX < 0.5f ? 0f : 1f;
			pivotY = pivotY < 0.5f ? 0f : 1f;

			rt.pivot = new Vector2(pivotX, pivotY);
			rt.anchoredPosition = point;
		}
		private void UpdateLayout()
		{
			int headerLength = headerUGUI.text.Length;
			int contentLength = contentUGUI.text.Length;

			layout.enabled = headerLength > characterWrapLimit || contentLength > characterWrapLimit;
		}

		public void SetText(string content, string header)
		{
			bool hasHeader = !string.IsNullOrEmpty(header);
			headerUGUI.gameObject.SetActive(hasHeader);
			UpdatePosition();

			if (hasHeader)
				headerUGUI.text = header;

			contentUGUI.text = content;

			UpdateLayout();
		}
	}
	[System.Serializable]
	public struct TooltipData
	{
		public TooltipData(string header, string content)
		{
			this.header = header;
			this.content = content;
		}
		public TooltipData(string content)
		{
			this.content = content;
			this.header = "";
		}

		public string header;
		[TextArea(2, 8)]
		public string content;

		public override string ToString()
		{
			return $"{header}: {content}";
		}
	}
}
