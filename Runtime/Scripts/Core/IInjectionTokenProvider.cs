using System;

namespace Obsidize.DependencyInjection
{
	/// <summary>
	/// Baseline non-generic type used for storing providers in a collection.
	/// </summary>
	public interface IInjectionTokenProvider : IDisposable
	{
		public event Action<IInjectionTokenProvider> OnDispose;
		public Type TokenType { get; }
	}
}
