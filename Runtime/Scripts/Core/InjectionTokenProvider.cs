using System;
using UnityEngine;

namespace Obsidize.DependencyInjection
{
	/// <summary>
	/// Maintains the cycling of tokens for a target type.
	/// A token can be provided multiple different times during the injector's lifetime,
	/// as long as each token is disposed of before a new one is provided.
	/// </summary>
	/// <typeparam name="T">The type of token that will be provided</typeparam>
	public class InjectionTokenProvider<T> : IInjectionTokenProvider
		where T : class
	{

		private event Action<T> OnProvision;
		private InjectionToken<T> _token;

		public event Action<IInjectionTokenProvider> OnDispose;
		public event Action<IInjectionTokenProvider> OnTokenRequest;
		public event Action<IInjectionTokenProvider> OnTokenChange;

		public Type TokenType => typeof(T);
		public bool HasToken => _token != null;
		public bool HasTokenListeners => ListenerCount > 0;
		public int ListenerCount => OnProvision?.GetInvocationList()?.Length ?? 0;
		public T TokenValue => _token?.Value ?? default;
		public object DynamicTokenValue => TokenValue;

		private void HandleTokenDispose()
		{
			_token = null;
			OnTokenChange?.Invoke(this);
		}

		public override string ToString()
		{
			return $"InjectionTokenProvider<{TokenType.Name}> [Token = {_token?.ToString() ?? "null"}]";
		}

		public bool ProvideValue(T value)
		{
			return Provide(new InjectionToken<T>(value));
		}

		public bool ProvideValueWithOverwrite(T value)
		{
			return ProvideWithOverwrite(new InjectionToken<T>(value));
		}

		public bool ProvideWithOverwrite(InjectionToken<T> token)
		{
			DisposeCurrentToken();
			return Provide(token);
		}

		public void DisposeCurrentToken()
		{
			if (HasToken)
			{
				_token.Dispose();
			}
		}

		public void Dispose()
		{

			OnDispose?.Invoke(this);

			DisposeCurrentToken();

			OnProvision = null;
			OnDispose = null;
			OnTokenRequest = null;
			OnTokenChange = null;
		}

		public void RemoveListener(Action<T> listener)
		{

			if (listener != null)
			{
				OnProvision -= listener;
			}
		}

		public void AddListener(Action<T> listener)
		{

			if (listener == null)
			{
				return;
			}

			OnProvision += listener;

			if (HasToken)
			{
				listener.Invoke(_token.Value);
			}
			else
			{
				OnTokenRequest?.Invoke(this);
			}
		}

		public bool Provide(InjectionToken<T> token)
		{

			if (token == null)
			{
				return false;
			}

			if (!HasToken)
			{

				_token = token;
				_token.OnDispose += HandleTokenDispose;
				OnProvision?.Invoke(_token.Value);
				OnTokenChange?.Invoke(this);

				return true;
			}

			// Same instance provided twice for some reason,
			// so just tell the caller it is successfully set and
			// don't repeat the reigstry logic.
			if (_token == token)
			{
				return true;
			}

			Debug.LogWarning(
				$"Provide([token]) Error - Provider for {TokenType.Name}" +
				$" already contains instance {TokenValue}" +
				$", and will ignore duplicate provision from {token}"
			);

			Debug.LogWarning(
				"If you want to overwrite the existing token, use ProvideWithOverwrite()"
			);

			return false;
		}
	}
}
