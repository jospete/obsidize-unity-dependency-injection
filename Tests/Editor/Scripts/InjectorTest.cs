using NUnit.Framework;
using System;

namespace Obsidize.DependencyInjection.Testing.Editor
{
	public class InjectorTest
	{

		class MockToken
		{
		}

		[Test]
		public void CanCreateCustomInjectorInstances()
		{
			var injector = new Injector();
			Assert.AreNotEqual(injector, Injector.Main);
			Assert.AreNotEqual(injector.Registry, Injector.Main.Registry);
		}

		[Test]
		public void BlowsUpWhenGivenABadReigstryReference()
		{
			Assert.Throws<ArgumentNullException>(() => new Injector(null));
		}

		[Test]
		public void CanRetrieveTokenValues()
		{
			var injector = new Injector();
			var tokenValue = new MockToken();
			var token = new InjectionToken<MockToken>(tokenValue);
			var provider = injector.GetProvider<MockToken>();

			provider.Provide(token);

			Assert.AreEqual(tokenValue, injector.Get<MockToken>());
			Assert.AreEqual(provider, injector.GetProvider<MockToken>());
		}

		[Test]
		public void OffersAnOptionToWatchForTokenValueChanges()
		{

			var injector = new Injector();

			MockToken _ref = null;
			Action<MockToken> updateTokenRef = (MockToken t) => _ref = t;

			injector.Watch(updateTokenRef);
			Assert.AreEqual(null, _ref);

			var tokenValue = new MockToken();
			injector.GetProvider<MockToken>().Provide(new InjectionToken<MockToken>(tokenValue));
			Assert.AreEqual(tokenValue, _ref);

			injector.Unwatch(updateTokenRef);

			var tokenValue2 = new MockToken();
			injector.GetProvider<MockToken>().ProvideWithOverwrite(new InjectionToken<MockToken>(tokenValue2));
			Assert.AreEqual(tokenValue, _ref);
		}
	}
}
