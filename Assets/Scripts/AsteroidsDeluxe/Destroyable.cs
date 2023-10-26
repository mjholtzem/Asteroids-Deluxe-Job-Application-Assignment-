using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AsteroidsDeluxe
{
    public class Destroyable : MonoBehaviour
    {
        [SerializeField] private List<string> _collisionTags = new();

		public UnityEvent onCollisionDamage;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if(_collisionTags.Contains(collision.attachedRigidbody.tag) == false) return;

			onCollisionDamage?.Invoke();
		}
	}
}