using System;

namespace Obsidize.DependencyInjection
{
	/// <summary>
	/// Tracker for the state of a provided token.
	/// Tokens can be disposed of by either the injector or the provider.
	/// </summary>
	/// <typeparam name="T">The value type that this token supplies</typeparam>
	public class InjectionToken<T> : IDisposable, ITokenTypeRef
		where T : class
	{

		public event Action OnDispose;
		private readonly Func<T> _tokenFactory;

		public T Value => _tokenFactory.Invoke();
		public Type TokenType => typeof(T);

		public InjectionToken(Func<T> tokenFactory)
		{
			_tokenFactory = tokenFactory ?? throw new ArgumentNullException(nameof(tokenFactory));
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
			return $"InjectionToken<{TokenType.Name}> {Value}";
		}
	}
}
