using System;
using UnityEngine;

namespace Obsidize.DependencyInjection
{
	/// <summary>
	/// Wraps an IInjectionTokenSource and watches for token requests.
	/// When the source's provider receives a token request, this container
	/// will attempt to automatically spawn the token for that request.
	/// </summary>
	public class InjectionTokenSourceInstantiationContext : IDisposable
	{

		public static bool VerboseInstantiations { get; set; } = false;
		public static Func<Component, Component> OnInstantiate = UnityEngine.Object.Instantiate;

		private readonly IInjectionTokenProvider _provider;
		private readonly Component _instantiationTarget;
		private bool _isWatchingRequests;
		private int _maxAllowedInstantiations;
		private int _instantiationCount;

		public InjectionTokenSourceInstantiationContext(
			IInjectionTokenProvider tokenSource,
			Component instantiationTarget,
			int maxAllowedInstantiations
		)
		{

			_provider = tokenSource ?? throw new ArgumentNullException(nameof(tokenSource));
			if (maxAllowedInstantiations <= 0) throw new ArgumentException("Number of allowed instantiations must be at least 1");

			_instantiationTarget = instantiationTarget;
			_maxAllowedInstantiations = maxAllowedInstantiations;
			_isWatchingRequests = false;
			_instantiationCount = 0;
		}

		public InjectionTokenSourceInstantiationContext(
			IInjectionTokenProvider tokenSource,
			int maxAllowedInstantiations
		) : this(tokenSource, tokenSource as Component, maxAllowedInstantiations)
		{
		}

		public InjectionTokenSourceInstantiationContext(
			IInjectionTokenProvider tokenSource
		) : this(tokenSource, tokenSource as Component, 1)
		{
		}

		public void Dispose()
		{
			EndWatchingTokenRequests();
		}

		public void BeginWatchingTokenRequests()
		{

			if (_isWatchingRequests)
			{
				return;
			}

			_isWatchingRequests = true;
			_provider.OnTokenRequest += HandleTokenRequest;

			// If there are already listeners attached to the provider,
			// try to spawn the token immediately.
			if (_provider.HasTokenListeners)
			{
				HandleTokenRequest(_provider);
			}
		}

		public void EndWatchingTokenRequests()
		{

			if (!_isWatchingRequests)
			{
				return;
			}

			_isWatchingRequests = false;
			_provider.OnTokenRequest -= HandleTokenRequest;
		}

		private void HandleTokenRequest(IInjectionTokenProvider provider)
		{

			if (provider == null || provider.HasToken || provider != _provider)
			{
				return;
			}

			if (_instantiationCount >= _maxAllowedInstantiations)
			{
				Debug.LogWarning(
					$"Skipping instantiation of {provider.TokenType}" +
					$" due to instantiation limit of {_maxAllowedInstantiations}"
				);
				return;
			}

			var result = OnInstantiate?.Invoke(_instantiationTarget);

			if (VerboseInstantiations)
			{
				Debug.Log($"Automatically instantiated token from injection request -> {result}");
			}

			_instantiationCount++;
		}
	}
}
