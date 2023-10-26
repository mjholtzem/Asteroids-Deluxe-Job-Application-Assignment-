using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsteroidsDeluxe
{
	/// <summary>
	/// generic component for processing linear motion, angular motion, and drag
	/// this could all be handled using rigidbody but for a simple arcade game like asteroids I think it is probably
	/// worth it to handle it explicitly and have full control
	/// </summary>
    public class Movement : MonoBehaviour
    {
		public Vector2 currentVelocity;
		public float currentAngularVelocity;

		[Range(0f, 5f)]
		[SerializeField] private float _drag = 0;
		[SerializeField] private float _maxVelocity = -1;

		private void Update()
		{
			UpdateTurn();
			UpdateVelocity();
			UpdatePosition();
		}

		private void UpdateTurn()
		{
			transform.Rotate(new Vector3(0, 0, currentAngularVelocity * Time.deltaTime));
		}

		private void UpdateVelocity()
		{
			//Apply Drag
			currentVelocity -= currentVelocity * _drag * Time.deltaTime;

			//Constrain Velocity
			if(_maxVelocity < 0) return;
			if(currentVelocity.sqrMagnitude > _maxVelocity * _maxVelocity)
			{
				currentVelocity = currentVelocity.normalized * _maxVelocity;
			}
		}

		private void UpdatePosition()
		{
			transform.localPosition += (Vector3)(currentVelocity * Time.deltaTime);
		}
	}
}
