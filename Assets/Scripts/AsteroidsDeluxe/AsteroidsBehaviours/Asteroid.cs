using DG.Tweening;
using UnityEngine;

namespace AsteroidsDeluxe
{
	[RequireComponent(typeof(RandomDrift))]
	public class Asteroid : AsteroidsBehaviour
    {
		[Header("References")]
		[SerializeField] RandomDrift _randomDrift;
		public RandomDrift RandomDrift => _randomDrift;

		[Header("FX")]
		[SerializeField] private GameObject _destroyFXPrefab;

		[Header("Children")]
		[SerializeField] private Asteroid _childPrefab;
		[SerializeField] private int _spawnCount;

		protected override void OnEnable()
		{
			_randomDrift.RandomizeVelocity();

			//Tween In
			//transform.localScale = Vector3.one * .5f;
			//transform.DOScale(1, .15f).SetEase(Ease.InQuad);

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
