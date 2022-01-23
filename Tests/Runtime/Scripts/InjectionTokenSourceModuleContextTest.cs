using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Obsidize.DependencyInjection.Testing.Editor
{
	public class InjectionTokenSourceModuleContextTest
	{

		interface ITokenA
		{
		}

		class TokenASource : InjectionTokenSource<ITokenA>, ITokenA
		{
		}

		[UnityTest]
		public IEnumerator CanProviderMultipleTokenTypesAndDynamicallyDisposeItself()
		{

			TokenASource _instantiated = null;
			var previousInstantiate = InjectionTokenSourceInstantiationContext.OnInstantiate;
			InjectionTokenSourceInstantiationContext.OnInstantiate = v => (_instantiated = (TokenASource)v);

			var injector = Injector.Main;
			var provider = injector.GetProvider<ITokenA>();
			var go = new GameObject();

			Assert.IsFalse(provider.HasToken);

			var source = go.AddComponent<TokenASource>();
			provider.DisposeCurrentToken();

			var context = new InjectionTokenSourceModuleContext(injector)
				.Provide(source, 10);

			Assert.IsFalse(injector.GetProvider<ITokenA>().HasToken);

			Action<ITokenA> listener = _ => { };
			injector.Watch(listener);

			yield return new WaitForEndOfFrame();
			Assert.AreEqual(source, _instantiated);

			UnityEngine.Object.Destroy(go);
			yield return new WaitForEndOfFrame();

			InjectionTokenSourceInstantiationContext.OnInstantiate = previousInstantiate;
			context.Dispose();
			injector.Clear();
		}
	}
}
