using UnityEngine;

namespace AsteroidsDeluxe
{
	[RequireComponent(typeof(RandomDrift))]
	public class Asteroid : AsteroidsBehaviour
    {
		[Header("References")]
		[SerializeField] RandomDrift _randomDrift;
		public RandomDrift RandomDrift => _randomDrift;

		[Header("Children")]
		[SerializeField] private Asteroid _childPrefab;
		[SerializeField] private int _spawnCount;

		protected override void OnEnable()
		{
			_randomDrift.RandomizeVelocity();
			base.OnEnable();
		}

		protected override void OnCollisionDamage(AsteroidsBehaviour destructionSource, ObjectDestroyedMessage destructionMessage)
		{
			if(_spawnCount > 0 && _childPrefab != null)
			{
				for(int i = 0; i < _spawnCount; i++)
				{
					GameManager.Instance.WaveManager.SpawnAsteroid(_childPrefab, transform.position);
				}
			}

			Dispatch.Fire(destructionMessage);
		
			Destroy(gameObject);
		}
	}
}
