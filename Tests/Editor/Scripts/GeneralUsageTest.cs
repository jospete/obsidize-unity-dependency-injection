using NUnit.Framework;

namespace Obsidize.DependencyInjection.Testing.Editor
{
	public class GeneralUsageTest
	{

		class TestTokenValue
		{
			public bool someProperty = false;
		}

		[Test]
		public void CanCreateAnInjectorRegistry()
		{
			var registry = new InjectionTokenProviderRegistry();
			Assert.AreNotEqual(null, registry);
		}

		[Test]
		public void CanAccessInjectorsByType()
		{
			var registry = new InjectionTokenProviderRegistry();
			var injector = registry.ForType<TestTokenValue>();
			Assert.AreNotEqual(null, injector);
		}

		[Test]
		public void CanDisposeAnInjectionToken()
		{

			var value = new TestTokenValue();
			var token = new InjectionToken<TestTokenValue>(value);
			var disposed = false;

			token.OnDispose += () => disposed = true;
			Assert.AreEqual(false, disposed);

			token.Dispose();
			Assert.AreEqual(true, disposed);
		}

		[Test]
		public void CanAddAListenerToAnInjector()
		{
			var registry = new InjectionTokenProviderRegistry();
			var injector = registry.ForType<TestTokenValue>();
			var providedTokenValue = new TestTokenValue();
			var providedToken = new InjectionToken<TestTokenValue>(providedTokenValue);
			TestTokenValue capturedValue = null;

			Assert.AreEqual(false, injector.HasToken);
			Assert.AreEqual(injector.TokenType, providedToken.TokenType);
			Assert.AreEqual(providedTokenValue, providedToken.Value);

			injector.AddListener(injected => capturedValue = injected);
			Assert.AreEqual(null, capturedValue);

			injector.Provide(providedToken);
			Assert.AreEqual(providedTokenValue, capturedValue);
		}

		[Test]
		public void OnlyInstantiatesOneInjectorPerType()
		{

			var registry = new InjectionTokenProviderRegistry();
			var injector = registry.ForType<TestTokenValue>();
			var injector2 = registry.ForType<TestTokenValue>();

			Assert.AreEqual(injector2, injector);
		}
	}
}