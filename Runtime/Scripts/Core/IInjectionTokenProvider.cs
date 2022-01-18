using System;

namespace Obsidize.DependencyInjection
{
	/// <summary>
	/// Baseline non-generic type used for storing providers in a collection.
	/// </summary>
	public interface IInjectionTokenProvider : IDisposable, ITokenTypeRef
	{
		event Action<IInjectionTokenProvider> OnDispose;
		event Action<IInjectionTokenProvider> OnTokenRequest;
		bool HasToken { get; }
		bool HasTokenListeners { get; }
	}
}
