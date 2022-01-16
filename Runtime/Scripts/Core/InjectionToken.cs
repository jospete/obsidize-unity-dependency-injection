using System;

namespace Obsidize.DependencyInjection
{
	/// <summary>
	/// Tracker for the state of a provided token.
	/// Tokens can be disposed of by either the injector or the provider.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class InjectionToken<T> : IDisposable
	{

		public event Action OnDispose;
		private readonly Func<T> _tokenFactory;

		public T Value => _tokenFactory.Invoke();
		public Type Type => typeof(T);

		public InjectionToken(Func<T> tokenFactory)
		{
			_tokenFactory = tokenFactory ?? throw new NullReferenceException(nameof(tokenFactory));
		}

		public InjectionToken(T value) : this(() => value)
		{
		}

		public void Dispose()
		{
			OnDispose?.Invoke();
			OnDispose = null;
		}

		public override string ToString()
		{
			return $"InjectionToken<{Type.Name}> {Value}";
		}
	}
}
