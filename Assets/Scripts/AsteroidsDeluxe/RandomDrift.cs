using UnityEngine;

namespace AsteroidsDeluxe
{
    public class RandomDrift : RandomMovementController
    {
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

		public override void RandomizeVelocity()
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
	}
}
