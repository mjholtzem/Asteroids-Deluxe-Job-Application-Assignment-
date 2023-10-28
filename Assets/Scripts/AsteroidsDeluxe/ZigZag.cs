using UnityEngine;

namespace AsteroidsDeluxe
{
    public class ZigZag : RandomMovementController
    {
        [SerializeField] private float _moveSpeed = 10;
        [SerializeField] private float _minDirectionChangeDelay = 2;
        [SerializeField] private float _maxDirectionChangeDelay = 5f;

        private float _nextDirectionChangeTime = Mathf.Infinity;

        private void OnEnable()
        {
            RandomizeVelocity();
        }

        private void Update()
        {
            if(Time.time >= _nextDirectionChangeTime) RandomizeVelocity();
        }

        public override void RandomizeVelocity()
        {
            //Randomize Velocity
            var direction = Random.insideUnitCircle.normalized;
            _movement.currentVelocity = _moveSpeed * direction;

            _nextDirectionChangeTime = Time.time + Random.Range(_minDirectionChangeDelay, _maxDirectionChangeDelay);
        }
    }
}