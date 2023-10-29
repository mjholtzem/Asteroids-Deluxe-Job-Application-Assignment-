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

        [Header("FX")]
        [SerializeField] private GameObject _destroyFXPrefab;

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

            if(targetType == TargetType.Player && GameManager.Instance.Player.gameObject.activeInHierarchy == false) 
            {
                targetType = TargetType.Asteroid;
            }

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
                        AsteroidsBehaviour bestTarget = null;
                        foreach(var asteroid in GameManager.Instance.WaveManager.Asteroids)
                        {
                            var dist = (transform.position - asteroid.transform.position).sqrMagnitude;
                            if(dist < bestDist)
                            {
                                bestDist = dist;
                                bestTarget = asteroid;
                            }
                        }
                        foreach(var enemy in GameManager.Instance.WaveManager.Enemies)
                        {
                            if(enemy.ObjectType != ObjectType.DeathStar) continue;

                            var dist = (transform.position - enemy.transform.position).sqrMagnitude;
                            if(dist < bestDist)
                            {
                                bestDist = dist;
                                bestTarget = enemy;
                            }
                        }
                        return bestTarget != null ? bestTarget.transform.position : Vector2.zero;
                    }
            }

            return Vector2.zero;
        }

        protected override void OnCollisionDamage(AsteroidsBehaviour destructionSource, ObjectDestroyedMessage message)
        {
            Dispatch.Fire(message);
            if(_destroyFXPrefab) Instantiate(_destroyFXPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        protected override void Reset()
        {
            base.Reset();
        }
    }
}
