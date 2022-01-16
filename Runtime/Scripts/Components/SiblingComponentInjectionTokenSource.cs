using UnityEngine;

namespace Obsidize.DependencyInjection
{
	/// <summary>
	/// Convenience intermediate for providing Component-based tokens (e.g. Camera)
	/// </summary>
	/// <typeparam name="T">The component token type that will be provided</typeparam>
	public abstract class SiblingComponentInjectionTokenSource<T> : InjectionTokenSource<T>
		where T : Component
	{

		private T _tokenValue;

		protected override T GetInjectionTokenValue() => _tokenValue;

		protected override void Awake()
		{
			_tokenValue = GetComponent<T>();
			base.Awake();
		}
	}
}
