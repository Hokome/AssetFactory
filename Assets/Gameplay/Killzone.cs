using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory
{
	[RequireComponent(typeof(Collider2D))]
	public class Killzone : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.TryGetComponent(out IKillable k))
				k.Kill();
		}
	}
}
