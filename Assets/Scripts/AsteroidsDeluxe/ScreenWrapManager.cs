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

		private Vector2[] _projectedPlayerPositions = new Vector2[9];

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

			UpdateProjectedPlayerPositions();
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

		/// <summary>
		/// will compare the srcPosition to all of the "player projected positions"
		/// useful for tracking the player across the edges of the screen
		/// </summary>
		/// <param name="srcPosition"></param>
		/// <returns>the closts position to the srcPosition</returns>
		public Vector2 GetClosestPlayerPosition(Vector2 srcPosition)
		{
			if(_projectedPlayerPositions == null) return GameManager.Instance.Player.transform.position;

			var bestDist = Mathf.Infinity;
			var bestPos = Vector2.zero;

			foreach(var pos in _projectedPlayerPositions)
			{
				var dist = (srcPosition - pos).sqrMagnitude;
                if (dist < bestDist)
                {
					bestDist = dist;
					bestPos = pos;
                }
            }

			return bestPos;
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

		private void UpdateProjectedPlayerPositions()
		{
			var player = GameManager.Instance.Player;
			if(player == null) return;

			//start with the basic position
			var playerPos = player.transform.localPosition;
			_projectedPlayerPositions[0] = playerPos;

			//now reflect that position over each edge and diagonal

			var px = playerPos.x;
			var py = playerPos.y;

			//Left
			_projectedPlayerPositions[1] = new Vector2(_cameraBounds.min.x - (_cameraBounds.max.x - px), py);
			//Right
			_projectedPlayerPositions[2] = new Vector2(_cameraBounds.max.x + (px - _cameraBounds.min.x), py);
			//Up
			_projectedPlayerPositions[3] = new Vector2(px, _cameraBounds.max.y + (py - _cameraBounds.min.y));
			//Down
			_projectedPlayerPositions[4] = new Vector2(px, _cameraBounds.min.y - (_cameraBounds.max.y - py));

			//Diagonal positions are just combinations
			//TL
			_projectedPlayerPositions[5] = new Vector2(_projectedPlayerPositions[1].x, _projectedPlayerPositions[3].y);
			//TR
			_projectedPlayerPositions[6] = new Vector2(_projectedPlayerPositions[2].x, _projectedPlayerPositions[3].y);
			//BR
			_projectedPlayerPositions[7] = new Vector2(_projectedPlayerPositions[2].x, _projectedPlayerPositions[4].y);
			//BL
			_projectedPlayerPositions[8] = new Vector2(_projectedPlayerPositions[1].x, _projectedPlayerPositions[4].y);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;

			//Draw Projected Positions
			if(_projectedPlayerPositions != null)
			{
				for(int i = 1; i < _projectedPlayerPositions.Length; i++)
				{
					var position = _projectedPlayerPositions[i];
					Gizmos.DrawSphere(position, .25f);
				}
			}

			//Draw Tracked Bounds
			foreach(var target in _targets)
			{
				var bounds = target.Renderer.bounds;
				Gizmos.DrawWireCube(bounds.center, bounds.size);
			}

			Gizmos.color = Color.white;

		}
	}
}
