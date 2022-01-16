using System;

namespace Obsidize.DependencyInjection
{
	public class InjectionTokenWatcher<T> : IInjectionTokenWatcher
		where T : class
	{

		private readonly Injector _injector;
		private readonly Action<T> _listener;

		public Type TokenType => typeof(T);
		public Injector SourceInjector => _injector;
		public Action<T> Listener => _listener;

		public InjectionTokenWatcher(Injector injector, Action<T> listener)
		{
			_injector = injector ?? throw new ArgumentNullException(nameof(injector));
			_listener = listener ?? throw new ArgumentNullException(nameof(listener));
		}

		public InjectionTokenWatcher(Action<T> listener) : this(Injector.Main, listener)
		{
		}

		public void Watch() => _injector.Watch(Listener);
		public void Unwatch() => _injector.Unwatch(Listener);
	}
}
