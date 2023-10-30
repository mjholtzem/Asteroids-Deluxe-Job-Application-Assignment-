using UnityEngine;

namespace AsteroidsDeluxe
{
	[RequireComponent(typeof(Gun))]
	public class Player : AsteroidsBehaviour
    {
        [Header("References")]
        [SerializeField] private Gun _gun;
        [SerializeField] private Shield _shield;

        [Header("Movement")]
        [SerializeField] private float _turnSpeed;
        [SerializeField] private float _boostAcceleration;

        [Header("FX")]
        [SerializeField] private Transform _boostFX;

        private bool _isBoosting = false;

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
            _shield.Init();
            _shield.TurnOff();

            _isBoosting = false;
        }

        private void Update()
        {
            UpdateTurn();
            UpdateBoost();
            UpdateGun();
            UpdateShield();
        }

        private void UpdateTurn()
        {
            var input = -Input.GetAxisRaw("Horizontal");
            var turnSpeed = input * _turnSpeed;
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
            
            if(_isBoosting != isBoosting)
            {
                if(isBoosting) AudioManager.Instance.StartBoost();
                else AudioManager.Instance.StopBoost();
                _boostFX.gameObject.SetActive(isBoosting);

                _isBoosting = isBoosting;
            }
        }

        private void UpdateGun()
        {
            if(Input.GetKeyDown(KeyCode.Space) && _gun.CanFire)
            {
                _gun.Fire(_movement.currentVelocity);
            }
        }

        private void UpdateShield()
        {
            if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) _shield.TurnOn();
            else if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)) _shield.TurnOff();
        }

		protected override void OnCollisionDamage(AsteroidsBehaviour destructionSource, ObjectDestroyedMessage destructionMessage)
		{
            //check for shield?
            if(_shield.IsOn) return;

            _isBoosting = false;
            AudioManager.Instance.StopBoost();

			Debug.Log("Ship was destroyed!!!");
			Dispatch.Fire(destructionMessage);
            if(_destroyFX) _destroyFX.Play();
            gameObject.SetActive(false);
		}
	}
}
