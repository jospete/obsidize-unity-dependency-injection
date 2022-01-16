using System;

namespace Obsidize.DependencyInjection
{
	public class Injector : IDisposable
	{

		private static readonly Injector _main = new Injector();
		public static Injector Main => _main;

		private readonly InjectionTokenProviderRegistry _registry;
		public InjectionTokenProviderRegistry Registry => _registry;

		public Injector(InjectionTokenProviderRegistry registry)
		{
			_registry = registry ?? throw new ArgumentNullException(nameof(registry));
		}

		public Injector() : this(new InjectionTokenProviderRegistry())
		{
		}

		public void Clear() => Registry.Clear();
		public void Dispose() => Registry.Dispose();
		public InjectionTokenProvider<T> GetProvider<T>() where T : class => Registry.ForType<T>();
		public T Get<T>() where T : class => GetProvider<T>().TokenValue;
		public void Watch<T>(Action<T> listener) where T : class => GetProvider<T>().AddListener(listener);
		public void Unwatch<T>(Action<T> listener) where T : class => GetProvider<T>().RemoveListener(listener);
	}
}
