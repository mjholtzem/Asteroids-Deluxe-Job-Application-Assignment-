using UnityEngine;
using DG.Tweening;

namespace AsteroidsDeluxe
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioSource _boostAudioSource;
        [SerializeField] private AudioSource _saucerAudioSource;


        public void PlaySound(AudioClip clip, float volume = 1, float pitch = 1)
        {
            if(clip == null) return;
            _audioSource.pitch = pitch;
            _audioSource.PlayOneShot(clip, volume);
        }

        public void StartBoost()
        {
            _boostAudioSource.volume = 0;
            _boostAudioSource.Play();
            _boostAudioSource.DOFade(.75f, .5f).SetEase(Ease.InQuad);
        }

        public void StopBoost()
        {
            _boostAudioSource.DOKill();
            _boostAudioSource.Stop();
        }

        public void StartSaucer()
        {
            _saucerAudioSource.volume = 0;
            _saucerAudioSource.Play();
            _saucerAudioSource.DOFade(.5f, .5f).SetEase(Ease.InQuad);
        }

        public void StopSaucer()
        {
            _saucerAudioSource.Stop();
        }

        private void Reset()
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }
}