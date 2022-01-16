using NUnit.Framework;
using System;

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

		[Test]
		public void HasShorthandMethodsForInjectorListeners()
		{
			var registry = new InjectionTokenProviderRegistry();
			var injector = registry.ForType<SampleTokenValue>();

			var providedValue = new SampleTokenValue();
			var token = new InjectionToken<SampleTokenValue>(providedValue);

			var providedValue2 = new SampleTokenValue();
			var token2 = new InjectionToken<SampleTokenValue>(providedValue2);

			SampleTokenValue capturedValue = null;
			Action<SampleTokenValue> captureDelegate = injected => capturedValue = injected;

			registry.AddTokenListener(captureDelegate);

			Assert.AreEqual(null, capturedValue);

			injector.Provide(token);
			Assert.AreEqual(providedValue, capturedValue);

			injector.ProvideWithOverwrite(token2);
			Assert.AreEqual(providedValue2, capturedValue);

			registry.RemoveTokenListener(captureDelegate);
			injector.ProvideWithOverwrite(token);
			Assert.AreEqual(providedValue2, capturedValue);
		}
	}
}