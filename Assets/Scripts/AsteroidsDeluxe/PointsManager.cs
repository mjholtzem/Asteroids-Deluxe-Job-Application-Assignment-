using UnityEngine;

namespace AsteroidsDeluxe
{
    public class PointsManager : MonoBehaviour
    {
		[Header("Config")]
		[SerializeField] private int _pointsAsteroidLarge;
		[SerializeField] private int _pointsAsteroidMedium;
		[SerializeField] private int _pointsAsteroidSmall;

		private int _totalPoints = 0;

        private void Start()
        {
            Dispatch.Listen<ObjectDestroyedMessage>(OnObjectDestroyed);
        }

		private void OnDestroy()
		{
			Dispatch.Unlisten<ObjectDestroyedMessage>(OnObjectDestroyed);
		}

		private void OnObjectDestroyed(ObjectDestroyedMessage message)
		{
			//only give points for things shot by the player. We don't care about crashes or asteroids shot by enemies
			if(message.DestroyedByType != ObjectType.PlayerBullet) return;

			switch(message.DestroyedType)
			{
				case ObjectType.AsteroidLarge:
					AwardPoints(_pointsAsteroidLarge);
					break;
				case ObjectType.AsteroidMedium:
					AwardPoints(_pointsAsteroidMedium);
					break;
				case ObjectType.AsteroidSmall:
					AwardPoints(_pointsAsteroidSmall);
					break;
				default: break;
			}
		}

		private void AwardPoints(int points)
		{
			if(points <= 0) return;

			_totalPoints += points;
			Dispatch.Fire(new PointsAwardedMessage { totalPoints = _totalPoints, pointsAwarded = points});
			Debug.Log($"Points: awarded - {points}, total - {_totalPoints}");
		}
	}
}
