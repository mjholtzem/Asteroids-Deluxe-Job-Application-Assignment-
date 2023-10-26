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

		private List<Bullet> _spawnedBullets = new();
		private float _cooldownTimer = 0;

		public bool CanFire => _spawnedBullets.Count < _maxBullets && _cooldownTimer <= 0;

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

		public void Fire(Vector2 inheritedVelocity)
		{
			if(CanFire == false) return;

			var bullet = Instantiate(_bulletPrefab, _spawnPosition.position, _spawnPosition.rotation);
			bullet.Init((_bulletSpeed * (Vector2)bullet.transform.up) + inheritedVelocity, _bulletLifetime);
			_spawnedBullets.Add(bullet);
		}
	}
}
