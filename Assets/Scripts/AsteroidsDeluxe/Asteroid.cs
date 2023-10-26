using UnityEngine;

namespace AsteroidsDeluxe
{
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(Destroyable))]
	public class Asteroid : MonoBehaviour, ICanScreenWrap
    {
		[SerializeField] private ObjectType _type;

        [Header("References")]
        [SerializeField] private Movement _movement;
		[SerializeField] private SpriteRenderer _mainRenderer;
		[SerializeField] private Destroyable _destroyable;

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

		public Renderer Renderer => _mainRenderer;
		public Vector2 Velocity => _movement.currentVelocity;
		public Destroyable Destroyable => _destroyable;

		private void OnEnable()
		{
			GameManager.Instance.ScreenWrapManager.RegisterTarget(this);
			_destroyable.onCollisionDamage.AddListener(OnAsteroidDestroyed);
			RandomizeVelocity();
		}

		private void OnDisable()
		{
			GameManager.Instance.ScreenWrapManager.RemoveTarget(this);
			_destroyable.onCollisionDamage.RemoveListener(OnAsteroidDestroyed);
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

		private void OnAsteroidDestroyed()
		{
			if(_spawnCount > 0 && _childPrefab != null)
			{
				for(int i = 0; i < _spawnCount; i++)
				{
					GameManager.Instance.WaveManager.SpawnAsteroid(_childPrefab, transform.position);
				}
			}

			Dispatch.Fire(new AsteroidDestroyedMessage { asteroid = this});
			Dispatch.Fire(new ObjectDestroyedMessage { type = _type });
			Destroy(gameObject);
		}
	}
}
