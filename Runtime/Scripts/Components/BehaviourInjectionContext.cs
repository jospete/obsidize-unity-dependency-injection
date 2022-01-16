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

		public BehaviourInjectionContext(Injector injector, MonoBehaviour consumer)
		{
			_injector = injector ?? throw new ArgumentNullException(nameof(injector));
			_consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
		}

		public BehaviourInjectionContext(MonoBehaviour consumer) : this(Injector.Main, consumer)
		{
		}

		public void Dispose()
		{
			Clear();
		}

		public BehaviourInjectionContext Inject<T>(
			Action<T> listener,
			float maxWaitTimeSeconds = BehaviourExtensions.defaultMaxRequireWaitTime
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
