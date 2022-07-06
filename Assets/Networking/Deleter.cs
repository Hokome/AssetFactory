#pragma warning disable CS0414
using UnityEngine;

namespace AssetFactory
{
	public class Deleter : MonoBehaviour
	{
		[SerializeField] bool deleteOnServer = false;
		[SerializeField] bool deleteOnClient = false;
		[Tooltip("If empty, will delete game object.")]
		[SerializeField] Component[] delete;

		void Awake()
		{
#if UNITY_SERVER
			if (deleteOnServer)
				Delete();
#else
			if (deleteOnClient)
				Delete();
#endif
			Destroy(this);
		}
		void Delete()
		{
			if (delete == null || delete.Length < 1)
			{
				Destroy(gameObject);
			}
			else
			{
				foreach (Component c in delete)
				{
					Destroy(c);
				}
			}
		}
	}
}