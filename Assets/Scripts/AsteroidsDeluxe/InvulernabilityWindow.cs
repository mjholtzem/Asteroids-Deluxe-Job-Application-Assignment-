using System;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class InvulernabilityWindow : MonoBehaviour
{
	[SerializeField][Min(0)] private float _invulnerabilityWindow = .5f;
	[SerializeField] private Rigidbody2D _rigidbody;

    private void OnEnable()
    {
		HandleInvulerabilityWindow();
    }

	private async void HandleInvulerabilityWindow()
	{
		_rigidbody.simulated = false;
		await Task.Delay(TimeSpan.FromSeconds(_invulnerabilityWindow));
		_rigidbody.simulated = true;
	}

	private void Reset()
	{
		_rigidbody = GetComponent<Rigidbody2D>();
	}
}
