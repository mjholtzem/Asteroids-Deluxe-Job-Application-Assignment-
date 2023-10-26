using UnityEngine;

namespace AsteroidsDeluxe
{
	public interface ICanScreenWrap
	{
		public Transform transform { get; }
		public Renderer Renderer { get; }
		public Vector2 Velocity { get; }
	}
}
