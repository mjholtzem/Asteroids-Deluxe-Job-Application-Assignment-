using UnityEngine;

namespace AsteroidsDeluxe
{
	public class Asteroid : AsteroidsBehaviour
    {
		[Header("Movement")]
		[SerializeField][Min(0)]
		private float _velocityMin;
		[SerializeField][Min(0)]
		private float _velocityMax;
		[Space(5)]
		[SerializeField][Min(0)]
		private float _angularVelocityMin;
		[SerializeField][Min(0)]
		private float _angularVelocityMax;

		[Header("Children")]
		[SerializeField] private Asteroid _childPrefab;
		[SerializeField] private int _spawnCount;

		protected override void OnEnable()
		{
			RandomizeVelocity();
			base.OnEnable();
		}

		public void RandomizeVelocity()
		{
			//Randomize Velocity
			var speed = Random.Range(_velocityMin, _velocityMax);
			var direction = Random.insideUnitCircle.normalized;
	
			_movement.currentVelocity = speed * direction;

			//Randomize Angular Velocity
			var angularVelocity = Random.Range(_angularVelocityMin, _angularVelocityMax);
			if(Random.value > .5f) angularVelocity *= -1;
			_movement.currentAngularVelocity = angularVelocity;
		}

		protected override void OnCollisionDamage()
		{
			if(_spawnCount > 0 && _childPrefab != null)
			{
				for(int i = 0; i < _spawnCount; i++)
				{
					GameManager.Instance.WaveManager.SpawnAsteroid(_childPrefab, transform.position);
				}
			}

			Dispatch.Fire(new AsteroidDestroyedMessage { asteroid = this });
			Dispatch.Fire(new ObjectDestroyedMessage { type = ObjectType });
			Destroy(gameObject);
		}
	}
}
