using System;
using System.Collections.Generic;
using UnityEngine;

namespace Obsidize.DependencyInjection
{
	/// <summary>
	/// Convenience to reduce boilerplate when 
	/// watching / requiring tokens on a MonoBehaviour.
	/// </summary>
	public class BehaviourInjectionContext : IDisposable
	{

		private readonly Dictionary<Type, IInjectionTokenWatcher> _watchers = new Dictionary<Type, IInjectionTokenWatcher>();
		private readonly Injector _injector;
		private readonly MonoBehaviour _consumer;
		private readonly float _defaultMaxWaitTime;

		public BehaviourInjectionContext(
			Injector injector,
			MonoBehaviour consumer,
			float defaultMaxWaitTime
		)
		{
			_injector = injector ?? throw new ArgumentNullException(nameof(injector));
			_consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
			_defaultMaxWaitTime = defaultMaxWaitTime;
		}

		public BehaviourInjectionContext(MonoBehaviour consumer, float defaultMaxWaitTime)
			: this(Injector.Main, consumer, defaultMaxWaitTime)
		{
		}

		public BehaviourInjectionContext(MonoBehaviour consumer)
			: this(consumer, BehaviourExtensions.defaultMaxRequireWaitTime)
		{
		}

		public void Dispose()
		{
			Clear();
		}

		public BehaviourInjectionContext Inject<T>(Action<T> listener) where T : class
		{
			return Inject(listener, _defaultMaxWaitTime);
		}

		public BehaviourInjectionContext Inject<T>(
			Action<T> listener,
			float maxWaitTimeSeconds
		)
		where T : class
		{
			_consumer.RequireInjectionToken<T>(_injector, maxWaitTimeSeconds);
			return InjectOptional(listener);
		}

		public BehaviourInjectionContext InjectOptional<T>(Action<T> listener)
			where T : class
		{

			var watcher = RegisterWatcher(listener);
			watcher.Watch();

			return this;
		}

		private void Clear()
		{

			if (_watchers.Count <= 0)
			{
				return;
			}

			foreach (var watcher in _watchers.Values)
			{
				watcher?.Unwatch();
			}

			_watchers.Clear();
		}

		private InjectionTokenWatcher<T> RegisterWatcher<T>(Action<T> listener)
			where T : class
		{

			var key = typeof(T);

			if (_watchers.ContainsKey(key))
			{
				throw new InvalidOperationException(
					$"{nameof(BehaviourInjectionContext)} cannot assign duplicate watcher for type {key.Name}"
				);
			}

			var watcher = new InjectionTokenWatcher<T>(_injector, listener);
			_watchers[key] = watcher;

			return watcher;
		}
	}
}
