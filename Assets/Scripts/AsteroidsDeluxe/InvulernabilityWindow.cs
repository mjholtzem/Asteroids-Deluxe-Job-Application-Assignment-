using System;
using System.Threading.Tasks;
using UnityEngine;

namespace AsteroidsDeluxe
{
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(AsteroidsBehaviour))]
	public class InvulernabilityWindow : MonoBehaviour
	{
		[SerializeField][Min(0)] private float _invulnerabilityWindow = .5f;
		[SerializeField] private Rigidbody2D _rigidbody;
		[SerializeField] private AsteroidsBehaviour _asteroidsBehaviour;

		private bool _wasEnabled = false;

		private void Update()
		{
			//detect when the root behaviour has been enabled and trigger invulnerability.
			//this is somewhat on the hacky side and ideally we might want to use UniRx or something
			//to actually just be notified of it but this is an ok way to go as well
			if(_asteroidsBehaviour == null) return;

			if(_wasEnabled == _asteroidsBehaviour.enabled) return;

			_wasEnabled = _asteroidsBehaviour.enabled;
			if(_asteroidsBehaviour.enabled) HandleInvulerabilityWindow();
		}

        private void OnDisable()
        {
			_wasEnabled = false;
        }

        private async void HandleInvulerabilityWindow()
		{
			_rigidbody.simulated = false;
			await Task.Delay(TimeSpan.FromSeconds(_invulnerabilityWindow));
			_rigidbody.simulated = true;
		}

		private void Reset()
		{
			_rigidbody = GetComponent<Rigidbody2D>();
			_asteroidsBehaviour = GetComponent<AsteroidsBehaviour>();
		}
	}
}
