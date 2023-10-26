using UnityEngine;

namespace AsteroidsDeluxe
{
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(Gun))]
    [RequireComponent(typeof(Destroyable))]
	public class Player : MonoBehaviour, ICanScreenWrap
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer _mainRenderer;
        [SerializeField] private Movement _movement;
        [SerializeField] private Gun _gun;
        [SerializeField] private Destroyable _destroyable;

        [Header("Movement")]
        [SerializeField] private float _turnSpeed;
        [SerializeField] private float _boostAcceleration;

        [Header("FX")]
        [SerializeField] private Transform _boostFX;

		public Renderer Renderer => _mainRenderer;
		public Vector2 Velocity => _movement.currentVelocity;

		private void Start()
        {
            Init();
        }

        public void Init(bool isRespawn = false)
        {
            _movement.currentVelocity = Vector2.zero;
            _movement.currentAngularVelocity = 0;
            _boostFX.gameObject.SetActive(false);

            if(isRespawn) return;

            _destroyable.onCollisionDamage.AddListener(OnDamageCollision);
            GameManager.Instance.ScreenWrapManager.RegisterTarget(this);
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

		private void OnDamageCollision()
        {
            //check for shield?

            Debug.Log("Ship was destroyed!!!");
            Dispatch.Fire(new PlayerDestroyedMessage());
            gameObject.SetActive(false);
        }
	}
}
