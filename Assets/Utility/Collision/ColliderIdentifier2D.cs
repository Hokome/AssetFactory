using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory
{
	public class ColliderIdentifier2D : MonoBehaviour
	{
		[SerializeField] private string id = "a";
		[SerializeField] private LayerMask mask;
		[SerializeField] private bool trigger;

		private ITriggerOwner2D triggerOwner;
		private IColliderOwner2D colliderOwner;

		public string ID => id;
		public ITriggerOwner2D TriggerOwner
		{
			get
			{
				if (triggerOwner == null)
				{
					if (transform.parent.TryGetComponent(out ITriggerOwner2D o))
						triggerOwner = o;
					else
						throw new NullReferenceException($"Index collider is marked as trigger but has no assigned {nameof(ITriggerOwner2D)} and an owner was not found in the parent object.");
				}	
				return triggerOwner;
			}
			set => triggerOwner = value;
		}
		public IColliderOwner2D ColliderOwner
		{
			get
			{
				if (colliderOwner == null)
				{
					if (transform.parent.TryGetComponent(out IColliderOwner2D o))
						colliderOwner = o;
					else
						throw new NullReferenceException($"Index collider is marked as collider but has no assigned {nameof(IColliderOwner2D)} and an owner was not found in the parent object.");
				}

				return colliderOwner;
			}
			set => colliderOwner = value;
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			Notify(trigger, collision, true);
		}
		private void OnCollisionExit2D(Collision2D collision)
		{
			Notify(trigger, collision, false);
		}
		private void OnTriggerEnter2D(Collider2D collision)
		{
			Notify(trigger, collision, true);
		}
		private void OnTriggerExit2D(Collider2D collision)
		{
			Notify(trigger, collision, false);
		}
		private void Notify(bool notify, Collision2D collision, bool enter)
		{
			if (!notify) return;
			if (!mask.CheckLayer(collision.gameObject.layer)) return; 
			TriggerOwner.OnTrigger(new CollisionData2D(id, collision, enter));
		}
		private void Notify(bool notify, Collider2D collision, bool enter)
		{
			if (!notify) return;
			if (!mask.CheckLayer(collision.gameObject.layer)) return; 
			TriggerOwner.OnTrigger(new CollisionData2D(id, collision, enter));
		}
	}
}
