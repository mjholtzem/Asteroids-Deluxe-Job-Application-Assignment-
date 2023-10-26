using System.Collections.Generic;
using UnityEngine;

namespace AsteroidsDeluxe
{
	public class ScreenWrapManager : MonoBehaviour
	{
		private Vector2 _processedScreenSize = Vector2.zero;
		private List<ICanScreenWrap> _targets = new();
		private Bounds _cameraBounds = new Bounds();
		public Bounds CameraBounds => _cameraBounds;

		private void Awake()
		{
			ProcessScreenSize();
		}

		private void ProcessScreenSize()
		{
			_processedScreenSize = new Vector2(Screen.width, Screen.height);
			Debug.Log($"Processing Screen Size {_processedScreenSize}");

			var camera = Camera.main;
			var bl = camera.ViewportToWorldPoint(new Vector2(0, 0));
			var tr = camera.ViewportToWorldPoint(new Vector2(1, 1));

			_cameraBounds = new Bounds(camera.transform.position, new Vector3(tr.x - bl.x, tr.y - bl.y, Mathf.Infinity));
		}

		private void Update()
		{
			if(Screen.width != _processedScreenSize.x
				|| Screen.height != _processedScreenSize.y)
			{
				ProcessScreenSize();
			}

			foreach(var target in _targets) UpdateTarget(target);
		}

		public void RegisterTarget(ICanScreenWrap target)
		{
			if(_targets.Contains(target))
			{
				Debug.LogError($"Target with transform {target.transform.name} is already registered");
				return;
			}

			_targets.Add(target);
		}

		public void RemoveTarget(ICanScreenWrap target)
		{
			var index = _targets.IndexOf(target);
			if(index < 0)
			{
				Debug.LogError($"Target with transform {target.transform.name} is not registered");
				return;
			}

			_targets.RemoveAt(index);
		}

		private void UpdateTarget(ICanScreenWrap target)
		{
			var bounds = target.Renderer.bounds;
			if(_cameraBounds.Intersects(bounds)) return;

			//target is off-screen. Let's figure out if we need to warp them depending on velocity compared to which side they escaped
			if(target.Velocity == Vector2.zero) return;

			//when wrapping an object, we assume the world is centered on (0,0) as well as the camera
			//to make this more robust we could account for the cameras position when "flipping" the targets position

			var targetPosition = target.transform.localPosition;

			//check left/right edge
			if(bounds.max.x < _cameraBounds.min.x && target.Velocity.x < 0 || bounds.min.x > _cameraBounds.max.x && target.Velocity.x > 0)
			{
				target.transform.localPosition = new Vector3(targetPosition.x * -1, targetPosition.y, targetPosition.z);
				return;
			}

			//check bottom/top edge
			if(bounds.max.y < _cameraBounds.min.y && target.Velocity.y < 0 || bounds.min.y > _cameraBounds.max.y && target.Velocity.y > 0)
			{
				target.transform.localPosition = new Vector3(targetPosition.x, targetPosition.y * -1, targetPosition.z);
				return;
			}
		}
	}
}
