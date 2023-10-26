using UnityEngine;

namespace AsteroidsDeluxe
{
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(Destroyable))]
	public abstract class AsteroidsBehaviour : MonoBehaviour, ICanScreenWrap
	{
		[SerializeField] private ObjectType _objectType;
		public ObjectType ObjectType => _objectType;

		[Header("Base References")]
		[SerializeField] private Renderer _mainRenderer;
		[SerializeField] protected Movement _movement;
		[SerializeField] protected Destroyable _destroyable;

		public Renderer Renderer => _mainRenderer;
		public Vector2 Velocity => _movement.currentVelocity;

		protected virtual void OnEnable()
		{
			_destroyable.onCollisionDamage.AddListener(OnCollisionDamage);
			GameManager.Instance.ScreenWrapManager.RegisterTarget(this);
		}

		protected virtual void OnDisable()
		{
			_destroyable.onCollisionDamage.RemoveListener(OnCollisionDamage);
			GameManager.Instance.ScreenWrapManager.RemoveTarget(this);
		}

		protected abstract void OnCollisionDamage();

		protected virtual void Reset()
		{
			_movement = GetComponent<Movement>();
		}
	}
}