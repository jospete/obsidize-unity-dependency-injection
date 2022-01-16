using System;
using System.Collections.Generic;

namespace Obsidize.DependencyInjection
{
	/// <summary>
	/// Tracker for a collection of token providers.
	/// </summary>
	public class InjectionTokenProviderRegistry : IDisposable
	{

		public event Action<IInjectionTokenProvider> OnProviderAdd;
		public event Action<IInjectionTokenProvider> OnProviderRemove;

		private readonly Dictionary<Type, IInjectionTokenProvider> _providers = new Dictionary<Type, IInjectionTokenProvider>();
		private bool _disposed = false;

		public bool Contains(IInjectionTokenProvider injector) => injector != null && _providers.ContainsKey(injector.TokenType);
		public bool TryGetProvider(Type type, out IInjectionTokenProvider provider) => _providers.TryGetValue(type, out provider);

		private void Remove(IInjectionTokenProvider provider)
		{

			if (provider != null)
			{
				OnProviderRemove?.Invoke(provider);
				_providers.Remove(provider.TokenType);
			}
		}

		public void Dispose()
		{

			if (_disposed)
			{
				return;
			}

			_disposed = true;
			Clear();
			OnProviderAdd = null;
			OnProviderRemove = null;
		}

		public void Clear()
		{

			if (_providers.Count <= 0)
			{
				return;
			}

			foreach (var provider in _providers.Values)
			{
				if (provider == null) continue;
				provider.OnDispose -= Remove;
				OnProviderRemove?.Invoke(provider);
				provider.Dispose();
			}

			_providers.Clear();
		}

		public InjectionTokenProvider<T> ForType<T>() where T : class
		{

			if (_disposed)
			{
				throw new InvalidOperationException(
					$"{nameof(InjectionTokenProviderRegistry)} cannot be used after it has been disposed"
				);
			}

			var type = typeof(T);

			// Automatically create a provider internally,
			// so consumers don't have to worry about whether or not it exists yet
			if (!TryGetProvider(type, out var result))
			{
				result = _providers[type] = new InjectionTokenProvider<T>();
				result.OnDispose += Remove;
				OnProviderAdd?.Invoke(result);
			}

			return result as InjectionTokenProvider<T>;
		}
	}
}
