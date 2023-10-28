using UnityEngine;

namespace AsteroidsDeluxe
{
    [RequireComponent(typeof(Gun))]
    public class Saucer : AsteroidsBehaviour
    {
        public enum TargetType
        {
            Player = 0,
            Asteroid = 5,
        }

        [SerializeField] private Gun _gun;
        [SerializeField] private float _sequenceDelay;
        [SerializeField] private TargetType[] _targetingSequence;

        private int _targetSequenceIndex = 0;
        private float _nextFireSequenceTime = Mathf.Infinity;

        protected override void OnEnable()
        {
            ResetFireSequence();
            base.OnEnable();
        }

        private void Update()
        {
            if(Time.time < _nextFireSequenceTime || _gun == null || _gun.CanFire == false) return;

            var bulletDirection = CalculateTargetPosition();
            _gun.Fire(Vector2.zero, bulletDirection);
            _targetSequenceIndex++;

            if(_targetSequenceIndex >= _targetingSequence.Length) ResetFireSequence();
        }

        private void ResetFireSequence()
        {
            _targetSequenceIndex = 0;
            _nextFireSequenceTime = Time.time + _sequenceDelay;
        }

        private Vector2 CalculateTargetPosition()
        {
            if(_targetingSequence == null || _targetingSequence.Length == 0) return Vector2.zero;

            var targetType = _targetingSequence[_targetSequenceIndex % _targetingSequence.Length];
            switch(targetType)
            {
                case TargetType.Player:
                    {
                       return GameManager.Instance.ScreenWrapManager.GetClosestPlayerPosition(transform.position);
                    }
                case TargetType.Asteroid:
                    {
                        //determine closest asteroid
                        var bestDist = Mathf.Infinity;
                        AsteroidsBehaviour bestAsteroid = null;
                        foreach(var asteroid in GameManager.Instance.WaveManager.Asteroids)
                        {
                            var dist = (transform.position - asteroid.transform.position).sqrMagnitude;
                            if(dist < bestDist)
                            {
                                bestDist = dist;
                                bestAsteroid = asteroid;
                            }
                        }

                        return bestAsteroid != null ? bestAsteroid.transform.position : Vector2.zero;
                    }
            }

            return Vector2.zero;
        }

        protected override void OnCollisionDamage(AsteroidsBehaviour destructionSource, ObjectDestroyedMessage message)
        {
            Dispatch.Fire(message);
            Destroy(gameObject);
        }

        protected override void Reset()
        {
            base.Reset();
        }
    }
}
