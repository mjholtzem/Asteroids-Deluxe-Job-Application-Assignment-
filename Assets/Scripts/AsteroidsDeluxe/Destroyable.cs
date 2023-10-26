using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AsteroidsDeluxe
{
    public class Destroyable : MonoBehaviour
    {
        [SerializeField] private List<string> _collisionTags = new();

		public UnityEvent<AsteroidsBehaviour, ObjectDestroyedMessage> onCollisionDamage;

		private AsteroidsBehaviour _asteroidsBehavour;

		public void Init(AsteroidsBehaviour asteroidsBehaviour)
		{
			_asteroidsBehavour = asteroidsBehaviour;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if(_collisionTags.Contains(collision.attachedRigidbody.tag) == false) return;

			AsteroidsBehaviour destructionSource = collision.attachedRigidbody.GetComponent<AsteroidsBehaviour>();
			var message = new ObjectDestroyedMessage
			{
				destroyedObject = _asteroidsBehavour,
				destroyedByObject = destructionSource
			};

			onCollisionDamage?.Invoke(destructionSource, message);
		}
	}
}