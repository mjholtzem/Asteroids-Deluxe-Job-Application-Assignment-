using UnityEngine;

namespace AsteroidsDeluxe
{
	[RequireComponent(typeof(Movement))]
    public class RandomDrift : MonoBehaviour
    {
		[Header("References")]
		[SerializeField] private Movement _movement;

		[Header("Movement")]
		[SerializeField]
		[Min(0)]
		private float _velocityMin;
		[SerializeField]
		[Min(0)]
		private float _velocityMax;
		[Space(5)]
		[SerializeField]
		[Min(0)]
		private float _angularVelocityMin;
		[SerializeField]
		[Min(0)]
		private float _angularVelocityMax;

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

		private void Reset()
		{
			_movement = GetComponent<Movement>();
		}
	}
}
