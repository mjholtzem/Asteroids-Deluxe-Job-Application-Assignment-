using UnityEngine;

namespace AsteroidsDeluxe
{
    public class Shield : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private float _shieldDuration;

        private float _shieldTimer = 0;
        public bool CanTurnOn => _shieldTimer > 0;
        private bool _isOn;
        public bool IsOn => _isOn;

        public void Init()
        {
            _shieldTimer = _shieldDuration;
            Dispatch.Fire(new ShieldUpdateMessage { remainingShield = 1 });
        }

        public void TurnOn()
        {
            if(CanTurnOn == false) return;

            _isOn = true;
            gameObject.SetActive(true);
        }

        public void TurnOff()
        {
            _isOn = false;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if(_isOn == false) return;

            _shieldTimer = Mathf.Max(0, _shieldTimer - Time.deltaTime);
            Dispatch.Fire(new ShieldUpdateMessage { remainingShield = _shieldTimer/_shieldDuration });
            if(_shieldTimer <= 0) TurnOff();
        }
    }
}