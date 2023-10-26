using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AutoDestroyParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    private void Update()
    {
        if(_particleSystem.IsAlive() == false) Destroy(gameObject);
    }

    private void Reset()
	{
		_particleSystem = GetComponent<ParticleSystem>();
	}
}
