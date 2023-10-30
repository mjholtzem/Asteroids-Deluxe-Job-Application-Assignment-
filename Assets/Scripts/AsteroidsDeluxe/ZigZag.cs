using UnityEngine;

namespace AsteroidsDeluxe
{
    public class ZigZag : RandomMovementController
    {
        [SerializeField] private float _moveSpeed = 10;
        [SerializeField] private float _minDirectionChangeDelay = 2;
        [SerializeField] private float _maxDirectionChangeDelay = 5f;

        private float _nextDirectionChangeTime = Mathf.Infinity;
        private int _moveCount = 0;

        private void OnEnable()
        {
            _moveCount = 0;
            RandomizeVelocity();
        }

        private void Update()
        {
            if(Time.time >= _nextDirectionChangeTime) RandomizeVelocity();
        }

        public override void RandomizeVelocity()
        {
            //Randomize Velocity

            //every other move it will go back towards the center. This will help it from hanging around the edges hopefully
            var direction = (_moveCount % 2 == 0) 
                ? -(Vector2)transform.position.normalized 
                : Random.insideUnitCircle.normalized;
            _movement.currentVelocity = _moveSpeed * direction;

            _nextDirectionChangeTime = Time.time + Random.Range(_minDirectionChangeDelay, _maxDirectionChangeDelay);
            _moveCount++;
        }
    }
}