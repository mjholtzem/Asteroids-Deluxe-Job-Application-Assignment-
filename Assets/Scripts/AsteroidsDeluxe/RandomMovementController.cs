using UnityEngine;

namespace AsteroidsDeluxe
{
    [RequireComponent(typeof(Movement))]
    public abstract class RandomMovementController : MonoBehaviour
    {
        [SerializeField] protected Movement _movement;

        public abstract void RandomizeVelocity();

        protected virtual void Reset()
        {
            _movement = GetComponent<Movement>();
        }
    }
}