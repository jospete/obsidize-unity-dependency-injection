using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Obsidize.DependencyInjection.Testing.Runtime
{
	public class InjectionTokenSourceTest
	{

		interface SingletonProviderTest
		{
		}

		class TestProvider : InjectionTokenSource<SingletonProviderTest>, SingletonProviderTest
		{
			protected override SingletonProviderTest GetInjectionTokenValue() => this;
		}

		[UnityTest]
		public IEnumerator AutoRegistersAndDeregistersTheProvidedTokenValue()
		{

			var provider = Injector.Main.GetProvider<SingletonProviderTest>();
			Assert.IsFalse(provider.HasToken);

			var gameObject = new GameObject();
			gameObject.AddComponent<TestProvider>();
			Assert.IsTrue(provider.HasToken);

			UnityEngine.Object.Destroy(gameObject);
			yield return new WaitForEndOfFrame();

			Assert.IsFalse(provider.HasToken);
			Injector.Main.Clear();
		}

		[UnityTest]
		public IEnumerator AutomaticallyDestroysDuplicateInstancesByDefault()
		{

			var provider = Injector.Main.GetProvider<SingletonProviderTest>();
			Assert.IsFalse(provider.HasToken);

			var gameObject1 = new GameObject();
			var provider1 = gameObject1.AddComponent<TestProvider>();

			yield return new WaitForEndOfFrame();

			var gameObject2 = new GameObject();
			var provider2 = gameObject2.AddComponent<TestProvider>();

			yield return new WaitForEndOfFrame();

			Assert.IsTrue(provider1 != null);
			Assert.IsTrue(provider2 == null);

			UnityEngine.Object.Destroy(gameObject1);
			UnityEngine.Object.Destroy(gameObject2);
			yield return new WaitForEndOfFrame();

			Injector.Main.Clear();
		}
	}
}
