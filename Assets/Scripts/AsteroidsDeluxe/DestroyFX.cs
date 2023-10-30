using UnityEngine;

namespace AsteroidsDeluxe
{
    public class DestroyFX : MonoBehaviour
    {
        [Header("FX")]
        [SerializeField] private GameObject _destroyFXPrefab;

        [Header("Audio")]
        [SerializeField] private AudioClip _destructionSound;
        [SerializeField] private float _destructionSoundVolume;
        [SerializeField] private float _destructionSoundPitch;

       public void Play()
        {
            if(_destroyFXPrefab) Instantiate(_destroyFXPrefab, transform.position, Quaternion.identity);
            if(_destructionSound) AudioManager.Instance.PlaySound(_destructionSound, _destructionSoundVolume, _destructionSoundPitch);
        }
    }
}