using System.Collections.Generic;
using UnityEngine;

namespace AsteroidsDeluxe
{
	public class Gun : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] private Bullet _bulletPrefab;
		[SerializeField] private Transform _spawnPosition;

		[Header("Config")]
		[SerializeField] private int _maxBullets = 4;
		[SerializeField] private float _fireCooldown = .1f;
		[SerializeField] private float _bulletSpeed = 10f;
		[SerializeField] private float _bulletLifetime = 1f;
		[SerializeField] private float _maxBulletSpread = 0f;

		private List<Bullet> _spawnedBullets = new();
		private float _cooldownTimer = 0;

		public bool CanFire => (_spawnedBullets.Count < _maxBullets || _maxBullets < 0) 
			&& _cooldownTimer <= 0;

        private void Update()
		{
			CleanupBullets();

			if(_cooldownTimer <= 0) return;
			_cooldownTimer -= Time.deltaTime;

		}

		/// <summary>
		/// remove any bullets that have been destroyed
		/// destruction of bullets will be handled eslewhere
		/// </summary>
		private void CleanupBullets()
		{
			for(int i = 0; i < _spawnedBullets.Count; i++)
			{
				if(_spawnedBullets[i] == null || _spawnedBullets[i].gameObject == null)
				{
					_spawnedBullets.RemoveAt(i);
					i--;
				}
			}
		}

		public void Fire(Vector2 inheritedVelocity, Vector2? targetPosition = null)
		{
			if(CanFire == false) return;

			var rotation = _spawnPosition.rotation;
			if(targetPosition != null)
            {
				var direction = ((Vector3)targetPosition.Value - _spawnPosition.position).normalized;
				rotation = Quaternion.LookRotation(Vector3.forward, direction);
            }

			if(_maxBulletSpread > 0)
            {
				rotation *= Quaternion.Euler(0, 0, Random.Range(-_maxBulletSpread, _maxBulletSpread));
            }

			var bullet = Instantiate(_bulletPrefab, _spawnPosition.position, rotation);

			bullet.Init((_bulletSpeed * (Vector2)bullet.transform.up) + inheritedVelocity, _bulletLifetime);
			_spawnedBullets.Add(bullet);

			_cooldownTimer = _fireCooldown;
		}
	}
}
