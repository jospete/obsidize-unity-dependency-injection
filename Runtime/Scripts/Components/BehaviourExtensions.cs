using System.Collections;
using UnityEngine;

namespace Obsidize.DependencyInjection
{
	public static class BehaviourExtensions
	{

		public const float defaultMaxRequireWaitTime = 5f;

		public delegate void RequireCallback<T>(T value, bool timeout);

		private static void Identity<T>(T value, bool timeout)
		{
		}

		public static Coroutine RequireInjectionToken<T>(
			this MonoBehaviour behaviour,
			Injector injector,
			float maxWaitTimeSeconds = defaultMaxRequireWaitTime
		) where T : class
		{
			return RequireInjectionToken<T>(behaviour, injector, Identity, maxWaitTimeSeconds);
		}

		public static Coroutine RequireInjectionToken<T>(
			this MonoBehaviour behaviour,
			Injector injector,
			RequireCallback<T> callback,
			float maxWaitTimeSeconds
		) where T : class
		{
			return behaviour.StartCoroutine(
				RequireInjectionTokenAsync(injector, callback, maxWaitTimeSeconds)
			);
		}

		public static IEnumerator RequireInjectionTokenAsync<T>(
			Injector injector,
			RequireCallback<T> callback,
			float maxWaitTimeSeconds
		) where T : class
		{

			if (injector == null || callback == null)
			{
				yield break;
			}

			var waitTime = 0f;
			T value = default;

			while (waitTime < maxWaitTimeSeconds && (value = injector.Get<T>()) == null)
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
