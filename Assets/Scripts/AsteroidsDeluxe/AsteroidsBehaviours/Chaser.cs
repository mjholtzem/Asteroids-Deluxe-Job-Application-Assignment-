using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AsteroidsDeluxe
{
    public class Chaser : AsteroidsBehaviour
    {
		[Header("Config")]
		[Min(0)]
		[SerializeField] private float _invulnerabilityWindow = .25f;

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

		protected override void OnEnable()
		{
			base.OnEnable();
			HandleInvulerabilityWindow();
		}

		private async void HandleInvulerabilityWindow()
		{
			Debug.Log($"Vulnerability Window - {gameObject.name} - {_invulnerabilityWindow}");
			Rigidbody.simulated = false;
			await Task.Delay(TimeSpan.FromSeconds(_invulnerabilityWindow));
			Debug.Log($"Vulnerability Window - {gameObject.name} - turning on rigidbody");
			Rigidbody.simulated = true;
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

			var dot = Vector2.Dot(transform.right, (_player.transform.position - transform.position).normalized);
			var turnSpeed = 0f;
			if(dot > .05f) turnSpeed = -_turnSpeed;
			else if(dot < .05f) turnSpeed = _turnSpeed;

			_movement.currentAngularVelocity = turnSpeed;

		}

		private void UpdateBoost()
		{
			if(_player == null || _player.gameObject.activeInHierarchy == false) return;

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

			Destroy(gameObject);
		}
	}
}
