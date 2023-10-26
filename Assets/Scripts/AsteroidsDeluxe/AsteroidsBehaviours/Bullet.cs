using UnityEngine;

namespace AsteroidsDeluxe
{
	public class Bullet : AsteroidsBehaviour
	{
		private float _autoDestroyTimer = Mathf.Infinity;

		public void Init(Vector2 velocity, float lifetime)
		{
			_movement.currentVelocity = velocity;
			_autoDestroyTimer = lifetime;
		}

		private void Update()
		{
			_autoDestroyTimer -= Time.deltaTime;
			if(_autoDestroyTimer <= 0)
			{
				OnCollisionDamage(this, new ObjectDestroyedMessage { destroyedObject = this, destroyedByObject = null});
			}
		}
		protected override void OnCollisionDamage(AsteroidsBehaviour destructionSource, ObjectDestroyedMessage message)
		{
			Destroy(gameObject);
		}
	}
}
