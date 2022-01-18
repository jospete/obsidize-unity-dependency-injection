using UnityEngine;

namespace Obsidize.DependencyInjection
{
	/// <summary>
	/// Stub for a camera token provided in the scene.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	public class CameraTokenSource : SiblingComponentInjectionTokenSource<Camera>
	{
	}
}
