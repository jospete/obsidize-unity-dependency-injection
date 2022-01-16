using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Obsidize.DependencyInjection.Testing.Runtime
{
	public class SiblingComponentInjectionTokenSourceTest
	{
		class ProvidableBehaviour : MonoBehaviour
		{
		}

		class TestSiblingSource : SiblingComponentInjectionTokenSource<ProvidableBehaviour>
		{
		}

		[UnityTest]
		public IEnumerator AutoRegistersAndDeregistersTheProvidedTokenValue()
		{

			var provider = Injector.Main.GetProvider<ProvidableBehaviour>();
			Assert.IsFalse(provider.HasToken);

			var gameObject = new GameObject();
			var tokenValue = gameObject.AddComponent<ProvidableBehaviour>();
			gameObject.AddComponent<TestSiblingSource>();

			Assert.IsTrue(provider.HasToken);
			Assert.AreEqual(tokenValue, provider.TokenValue);

			UnityEngine.Object.Destroy(gameObject);
			yield return new WaitForEndOfFrame();

			Assert.IsFalse(provider.HasToken);
			Injector.Main.Registry.Clear();
		}
	}
}
