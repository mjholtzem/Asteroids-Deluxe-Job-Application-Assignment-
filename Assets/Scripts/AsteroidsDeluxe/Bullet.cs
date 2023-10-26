using UnityEngine;

namespace AsteroidsDeluxe
{
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(Destroyable))]

	public class Bullet : MonoBehaviour, ICanScreenWrap
	{
		[Header("References")]
		[SerializeField] private Movement _movement;
		[SerializeField] private SpriteRenderer _mainRenderer;
		[SerializeField] private Destroyable _destroyable;

		private float _autoDestroyTimer = Mathf.Infinity;

		public Renderer Renderer => _mainRenderer;
		public Vector2 Velocity => _movement.currentVelocity;

		private void OnEnable()
		{
			GameManager.Instance.ScreenWrapManager.RegisterTarget(this);
			_destroyable.onCollisionDamage.AddListener(OnBulletDestroyed);
		}

		private void OnDisable()
		{
			GameManager.Instance.ScreenWrapManager.RemoveTarget(this);
			_destroyable.onCollisionDamage.RemoveListener(OnBulletDestroyed);
		}

		public void Init(Vector2 velocity, float lifetime)
		{
			_movement.currentVelocity = velocity;
			_autoDestroyTimer = lifetime;
		}

		private void Update()
		{
			_autoDestroyTimer -= Time.deltaTime;
			if(_autoDestroyTimer <= 0) OnBulletDestroyed();
		}

		private void OnBulletDestroyed()
		{
			Destroy(gameObject);
		}
	}
}
