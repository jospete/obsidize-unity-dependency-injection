using UnityEngine;

namespace BulletPoolingExample.BridgeInterfaces
{
	/// <summary>
	/// Bare minimum template for what a player can do / what information it has available.
	/// This is made with the same mindset as IBulletPool, where smaller interface = better.
	/// </summary>
	public interface IPlayer
	{
		Vector3 BulletStartPosition { get; }
		Vector3 AimDirection { get; }
	}
}
