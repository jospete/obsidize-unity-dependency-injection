using System;
using System.Collections;
using UnityEngine;

namespace Obsidize.DependencyInjection
{
	public class Injector : IDisposable
	{

		public delegate void RequireCallback<T>(T value, bool timeout);

		private static readonly Injector _main = new Injector();
		public static Injector Main => _main;

		private readonly InjectionTokenProviderRegistry _registry;
		public InjectionTokenProviderRegistry Registry => _registry;

		private float _defultRequireWait = 5f;
		public float DefaultRequireWaitTime
		{
			get => _defultRequireWait;
			set => _defultRequireWait = Mathf.Max(1f, value);
		}

		public Injector(InjectionTokenProviderRegistry registry)
		{
			_registry = registry ?? throw new NullReferenceException(nameof(registry));
		}

		public Injector() : this(new InjectionTokenProviderRegistry())
		{
		}

		public void Clear() => Registry.Clear();
		public T Get<T>() => Registry.GetTokenValue<T>();
		public InjectionTokenProvider<T> GetProvider<T>() => Registry.ForType<T>();
		public void Watch<T>(Action<T> listener) => Registry.AddTokenListener(listener);
		public void Unwatch<T>(Action<T> listener) => Registry.RemoveTokenListener(listener);
		public IEnumerator Require<T>(RequireCallback<T> callback) => Require<T>(callback, DefaultRequireWaitTime);
		public IEnumerator Require<T>(Action<T> listener) => Require<T>(listener, DefaultRequireWaitTime);
		public IEnumerator RequireAndWatch<T>(Action<T> listener) => RequireAndWatch<T>(listener, DefaultRequireWaitTime);
		public void Dispose() => Registry.Dispose();

		public IEnumerator RequireAndWatch<T>(Action<T> listener, float maxWaitTimeSeconds)
		{
			Watch(listener);
			return Require(listener, maxWaitTimeSeconds);
		}

		public IEnumerator Require<T>(Action<T> listener, float maxWaitTimeSeconds)
		{
			return Require<T>((value, _) => listener(value), maxWaitTimeSeconds);
		}

		public IEnumerator Require<T>(RequireCallback<T> callback, float maxWaitTimeSeconds)
		{

			if (callback == null)
			{
				yield break;
			}

			var waitTime = 0f;
			T value = default;

			while (waitTime < maxWaitTimeSeconds && (value = Get<T>()) == null)
			{
				yield return new WaitForEndOfFrame();
				waitTime += Time.deltaTime;
			}

			var timeout = waitTime >= maxWaitTimeSeconds;

			if (timeout)
			{
				Debug.LogWarning(
					$"{typeof(T).Name} token is required in the current scene" +
					$", but none was provided after {maxWaitTimeSeconds} seconds"
				);
			}

			callback.Invoke(value, timeout);
		}
	}
}
