using NUnit.Framework;

namespace Obsidize.DependencyInjection.Testing.Editor
{
	public class InjectorRegistryTest
	{

		class SampleTokenValue
		{
		}

		[Test]
		public void CanClearAllRegisteredInjectors()
		{
			var registry = new InjectionTokenProviderRegistry();
			var injector = registry.ForType<SampleTokenValue>();

			Assert.AreNotEqual(null, injector);
			Assert.AreEqual(true, registry.Contains(injector));

			registry.Clear();
			Assert.AreEqual(false, registry.Contains(injector));
		}

		[Test]
		public void CanSafelyDisposeOfInjectorsWithoutCausingRegistryLeaks()
		{
			var registry = new InjectionTokenProviderRegistry();
			var injector = registry.ForType<SampleTokenValue>();

			Assert.AreNotEqual(null, injector);
			Assert.AreEqual(true, registry.Contains(injector));

			injector.Dispose();
			Assert.AreEqual(false, registry.Contains(injector));
		}
	}
}