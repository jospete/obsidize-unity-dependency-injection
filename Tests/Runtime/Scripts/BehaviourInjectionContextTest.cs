using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Obsidize.DependencyInjection.Testing.Runtime
{
	public class BehaviourInjectionContextTest
	{

		class TokenA
		{
		}

		class TokenB
		{
		}

		class TestBehaviour : MonoBehaviour
		{
		}

		[UnityTest]
		public IEnumerator CanInjectMultipleTokenTypesAndDynamicallyDisposeItself()
		{

			var injector = new Injector();
			var go = new GameObject();
			var consumer = go.AddComponent<TestBehaviour>();

			TokenA tokenA = null;
			TokenB tokenB = null;

			var injectionContext = new BehaviourInjectionContext(injector, consumer, 1f)
				.Inject<TokenA>(v => tokenA = v)
				.Inject<TokenB>(v => tokenB = v);

			Assert.IsNull(tokenA);
			Assert.IsNull(tokenB);

			var a1 = new TokenA();
			var b1 = new TokenB();

			injector.GetProvider<TokenA>().ProvideValue(a1);
			injector.GetProvider<TokenB>().ProvideValue(b1);

			Assert.AreEqual(a1, tokenA);
			Assert.AreEqual(b1, tokenB);

			injectionContext.Dispose();

			var a2 = new TokenA();
			var b2 = new TokenB();

			injector.GetProvider<TokenA>().ProvideValueWithOverwrite(a2);
			injector.GetProvider<TokenB>().ProvideValueWithOverwrite(b2);

			Assert.AreEqual(a1, tokenA);
			Assert.AreEqual(b1, tokenB);

			UnityEngine.Object.Destroy(go);
			yield return new WaitForEndOfFrame();

			injector.Clear();
		}
	}
}
