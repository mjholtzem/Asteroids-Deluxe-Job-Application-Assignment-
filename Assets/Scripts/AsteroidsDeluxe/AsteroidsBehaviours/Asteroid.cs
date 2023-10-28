using UnityEngine;

namespace AsteroidsDeluxe
{
	[RequireComponent(typeof(RandomDrift))]
	public class Asteroid : AsteroidsBehaviour
    {
		[Header("References")]
		[SerializeField] RandomMovementController _randomMovementController;
		public RandomMovementController RandomMovementController => _randomMovementController;

		[Header("FX")]
		[SerializeField] private GameObject _destroyFXPrefab;

		[Header("Children")]
		[SerializeField] private Asteroid _childPrefab;
		[SerializeField] private int _spawnCount;

		protected override void OnEnable()
		{
			_randomMovementController.RandomizeVelocity();

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
		
			if(_destroyFXPrefab) Instantiate(_destroyFXPrefab, transform.position, Quaternion.identity);
			Destroy(gameObject);
		}
	}
}
