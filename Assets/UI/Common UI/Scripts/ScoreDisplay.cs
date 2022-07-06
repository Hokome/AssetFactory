using TMPro;
using UnityEngine;

namespace AssetFactory.UI
{
	public class ScoreDisplay : MonoBehaviour
	{
		[SerializeField] private TMP_Text textBox;

		public string Text
		{
			get => textBox.text;
			set => textBox.text = value;
		}
	}
}
