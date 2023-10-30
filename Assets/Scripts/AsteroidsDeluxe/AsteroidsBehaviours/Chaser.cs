using System.Collections.Generic;
using UnityEngine;

namespace AsteroidsDeluxe
{
    public class Chaser : AsteroidsBehaviour
    {
		[Header("Movement")]
		[SerializeField] private float _turnSpeed;
		[SerializeField] private float _boostAcceleration;

		[Header("Children")]
		[SerializeField] private List<Chaser> _children = new();
		public List<Chaser> Children => _children;

		private Player _player;

		protected override void Start()
		{
			base.Start();

			_player = GameManager.Instance.Player;
		}

		private void Update()
		{
			UpdateTurn();
			UpdateBoost();
		}

		private void UpdateTurn()
		{
			if(_player == null) return;

			if(_player.gameObject.activeInHierarchy == false)
			{
				_movement.currentAngularVelocity = 0;
				return;
			}

			var playerPos = GameManager.Instance.ScreenWrapManager.GetClosestPlayerPosition(transform.position);
			var dot = Vector2.Dot(transform.right, (playerPos - (Vector2)transform.position).normalized);
			var turnSpeed = 0f;
			if(dot > .05f) turnSpeed = -_turnSpeed;
			else if(dot < .05f) turnSpeed = _turnSpeed;

			_movement.currentAngularVelocity = turnSpeed;

		}

		private void UpdateBoost()
		{
			_movement.currentVelocity += (_boostAcceleration * Time.deltaTime) * (Vector2)transform.up;
		}

		protected override void OnCollisionDamage(AsteroidsBehaviour destructionSource, ObjectDestroyedMessage message)
		{
			//Launch Chaser children
			foreach(var child in _children)
			{
				child.transform.SetParent(null, true);
				child.Movement.enabled = true;
				child.enabled = true;
			}

			Dispatch.Fire(message);
			_destroyFX.Play();
			Destroy(gameObject);
		}
	}
}
