using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Obsidize.DependencyInjection.Testing.Runtime
{
	public class GeneralUsageTest
	{

		interface SingletonProviderTest
		{
		}

		class TestProvider : InjectionTokenSource<SingletonProviderTest>, SingletonProviderTest
		{
			protected override SingletonProviderTest GetInjectionTokenValue() => this;
		}

		class TestConsumer : MonoBehaviour
		{

			public SingletonProviderTest singletonInstance;

			private void Awake()
			{
				Injector.Main.RequireAndWatch<SingletonProviderTest>(OnSingletonProvided);
			}

			private void OnDestroy()
			{
				Injector.Main.Unwatch<SingletonProviderTest>(OnSingletonProvided);
			}

			private void OnSingletonProvided(SingletonProviderTest instance)
			{
				singletonInstance = instance;
			}
		}

		[UnityTest]
		public IEnumerator CanWatchAndRequireTokensSimultaneously()
		{

			var provider = Injector.Main.GetProvider<SingletonProviderTest>();
			Assert.IsFalse(provider.HasToken);

			var go1 = new GameObject();
			var consumer = go1.AddComponent<TestConsumer>();
			Assert.IsNull(consumer.singletonInstance);

			var go2 = new GameObject();
			var tokenValue = go2.AddComponent<TestProvider>();
			Assert.IsTrue(provider.HasToken);
			Assert.AreEqual(tokenValue, consumer.singletonInstance);

			UnityEngine.Object.Destroy(go1);
			UnityEngine.Object.Destroy(go2);
			yield return new WaitForEndOfFrame();

			Assert.IsFalse(provider.HasToken);
			Injector.Main.Clear();
		}
	}
}
