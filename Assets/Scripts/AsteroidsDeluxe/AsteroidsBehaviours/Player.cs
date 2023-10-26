using UnityEngine;

namespace AsteroidsDeluxe
{
	[RequireComponent(typeof(Gun))]
	public class Player : AsteroidsBehaviour
    {
        [Header("References")]
        [SerializeField] private Gun _gun;

        [Header("Movement")]
        [SerializeField] private float _turnSpeed;
        [SerializeField] private float _boostAcceleration;

        [Header("FX")]
        [SerializeField] private Transform _boostFX;

		protected override void Start()
        {
            base.Start();
            Init();
        }

        public void Init()
        {
            _movement.currentVelocity = Vector2.zero;
            _movement.currentAngularVelocity = 0;
            _boostFX.gameObject.SetActive(false);
        }

        private void Update()
        {
            UpdateTurn();
            UpdateBoost();
            UpdateGun();
        }

        private void UpdateTurn()
        {
            var input = Input.GetAxisRaw("Horizontal");
            var turnSpeed = 0f;
            if(input > 0) turnSpeed = -_turnSpeed;
            else if(input < 0) turnSpeed = _turnSpeed;

            _movement.currentAngularVelocity = turnSpeed;

		}

        private void UpdateBoost()
        {
            var isBoosting = Input.GetAxisRaw("Vertical") > 0;
            if(isBoosting)
            {
                //Apply Boost Acceleration in direction of the player
                _movement.currentVelocity += (_boostAcceleration * Time.deltaTime) * (Vector2)transform.up;
            }

            if(_boostFX) _boostFX.gameObject.SetActive(isBoosting);
        }

        private void UpdateGun()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                _gun.Fire(_movement.currentVelocity);
            }
        }

		protected override void OnCollisionDamage(AsteroidsBehaviour destructionSource, ObjectDestroyedMessage destructionMessage)
		{
			//check for shield?

			Debug.Log("Ship was destroyed!!!");
			Dispatch.Fire(destructionMessage);
			gameObject.SetActive(false);
		}
	}
}
