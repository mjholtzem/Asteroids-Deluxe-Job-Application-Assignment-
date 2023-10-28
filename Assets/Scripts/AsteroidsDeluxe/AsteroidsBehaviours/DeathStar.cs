using System.Collections.Generic;
using System.Xml.Serialization;
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

		[Header("FX")]
		[SerializeField] private GameObject _destroyFXPrefab;

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
			if(_destroyFXPrefab) Instantiate(_destroyFXPrefab, transform.position, Quaternion.identity);
			Destroy(gameObject);
		}

		protected override void Reset()
		{
			_randomDrift = GetComponent<RandomDrift>();
			base.Reset();
		}
	}
}
