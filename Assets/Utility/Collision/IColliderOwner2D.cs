using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory
{
	public struct CollisionData2D
	{
		public string ID { get; }
		public bool Trigger { get; }
		public bool IsEnter { get; }
		public bool ISExit => !IsEnter;
		public Collider2D Collider { get; }
		public Collision2D Collision { get; }

		public CollisionData2D(string id, Collider2D col, bool isEnter)
		{
			ID = id;
			Collider = col;
			IsEnter = isEnter;
			Trigger = true;

			Collision = null;
		}
		public CollisionData2D(string id, Collision2D col, bool isEnter)
		{
			ID = id;
			Collision = col;
			IsEnter = isEnter;
			Trigger = false;

			Collider = null;
		}
	}
	public interface IColliderOwner2D
	{
		void OnCollision(CollisionData2D data);
	}
	public interface ITriggerOwner2D
	{
		void OnTrigger(CollisionData2D data);
	}
	public interface IColliderTriggerOwner2D : IColliderOwner2D, ITriggerOwner2D { }

	public class ColliderList
	{
		private Dictionary<string, Collider> colliders = new();
		public void UpdateCollider(CollisionData2D data)
		{
			if (!colliders.ContainsKey(data.ID))
				colliders.Add(data.ID, new Collider(data));
			colliders[data.ID].Colliding = data.IsEnter;
		}

		public bool IsColliding(string id) => colliders.TryGetValue(id, out Collider c) && c.Colliding;

		private class Collider
		{
			public readonly string id;
			public readonly Collider2D collider;
			public bool Colliding { get; set; }

			public Collider(CollisionData2D data)
			{
				id = data.ID;
				collider = data.Collider;
			}
		}
	}
}
