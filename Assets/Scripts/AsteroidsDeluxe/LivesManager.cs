using System;
using UnityEngine;

namespace AsteroidsDeluxe
{
	public class LivesManager : MonoBehaviour
	{
		[SerializeField] private int _startingLives = 3;
		[SerializeField] private int _pointsPerLife = 10000;
		private int _playerLives = 0;
		public int PlayerLives => _playerLives;

		public void Init()
		{
			_playerLives = _startingLives;
			Dispatch.Fire(new LivesChangedMessage { currentLives = 3, deltaLives = 3 });
	
	}

		private void Start()
		{
			Dispatch.Listen<ObjectDestroyedMessage>(OnObjectDestroyed);
			Dispatch.Listen<PointsAwardedMessage>(OnPointsAwarded);
		}

        private void OnPointsAwarded(PointsAwardedMessage message)
        {
			if(message.totalPoints / _pointsPerLife == (message.totalPoints - message.pointsAwarded) / _pointsPerLife) return;
			_playerLives++;
			Dispatch.Fire(new LivesChangedMessage { currentLives = _playerLives, deltaLives = 1 });
		}

        private void OnDestroy()
		{
			Dispatch.Unlisten<ObjectDestroyedMessage>(OnObjectDestroyed);
			Dispatch.Unlisten<PointsAwardedMessage>(OnPointsAwarded);
		}

		private void OnObjectDestroyed(ObjectDestroyedMessage message)
		{
			if(message.DestroyedType != ObjectType.PlayerShip) return;

			_playerLives--;
			Dispatch.Fire(new LivesChangedMessage { currentLives = _playerLives, deltaLives = -1 });
		}
	}
}
