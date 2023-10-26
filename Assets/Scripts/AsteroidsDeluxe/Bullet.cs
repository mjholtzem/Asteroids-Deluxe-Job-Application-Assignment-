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
			if(_autoDestroyTimer <= 0) OnCollisionDamage();
		}

		protected override void OnCollisionDamage()
		{
			Destroy(gameObject);
		}
	}
}
