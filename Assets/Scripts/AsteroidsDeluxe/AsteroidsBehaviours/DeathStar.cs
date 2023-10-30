using System.Collections.Generic;
using UnityEngine;

namespace AsteroidsDeluxe
{
	public class DeathStar : AsteroidsBehaviour
	{
		[Header("References")]
		[SerializeField] private RandomDrift _randomDrift;
		public RandomDrift RandomDrift => _randomDrift;

		[Header("Children")]
		[SerializeField] private List<Chaser> _children = new();
		public List<Chaser> Children => _children;

		protected override void OnEnable()
		{
			_randomDrift.RandomizeVelocity();
			base.OnEnable();
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

		protected override void Reset()
		{
			_randomDrift = GetComponent<RandomDrift>();
			base.Reset();
		}
	}
}
