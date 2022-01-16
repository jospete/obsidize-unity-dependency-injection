using NUnit.Framework;
using System;

namespace Obsidize.DependencyInjection.Testing.Editor
{
	public class InjectionTokenProviderTest
	{

		class SampleValue
		{
		}

		[Test]
		public void ImplementsTheToStringOverride()
		{
			var provider = new InjectionTokenProvider<SampleValue>();
			var providerStr = provider.ToString();
			Assert.IsTrue(!string.IsNullOrEmpty(providerStr) && providerStr.StartsWith("InjectionTokenProvider"));
		}

		[Test]
		public void DoesNothingWhenPassedAnInvalidListenerDelegate()
		{
			var provider = new InjectionTokenProvider<SampleValue>();
			Assert.AreEqual(0, provider.ListenerCount);

			provider.AddListener(null);
			Assert.AreEqual(0, provider.ListenerCount);
		}

		[Test]
		public void ReturnsFalseWhenProvidedAnInvalidTokenInstance()
		{
			var provider = new InjectionTokenProvider<SampleValue>();
			Assert.AreEqual(false, provider.Provide(null));
		}

		[Test]
		public void ReturnsTrueWhenProvidedAnValidTokenInstance()
		{
			var provider = new InjectionTokenProvider<SampleValue>();
			var value = new SampleValue();
			var token = new InjectionToken<SampleValue>(value);

			Assert.AreEqual(true, provider.Provide(token));
		}

		[Test]
		public void ReturnsTrueWhenProvidedTheSameTokenInstance()
		{
			var provider = new InjectionTokenProvider<SampleValue>();
			var value = new SampleValue();
			var token = new InjectionToken<SampleValue>(value);

			Assert.AreEqual(true, provider.Provide(token));
			Assert.AreEqual(true, provider.Provide(token));
		}

		[Test]
		public void ReturnsFalseWhenProvidedASecondTokenInstance()
		{
			var provider = new InjectionTokenProvider<SampleValue>();
			var value = new SampleValue();
			var token = new InjectionToken<SampleValue>(value);
			var token2 = new InjectionToken<SampleValue>(value);

			Assert.AreEqual(true, provider.Provide(token));
			Assert.AreEqual(false, provider.Provide(token2));
		}

		[Test]
		public void ProvidesAnExistingTokenImmediatelyToAddedListeners()
		{
			var provider = new InjectionTokenProvider<SampleValue>();
			var value = new SampleValue();
			var token = new InjectionToken<SampleValue>(value);
			SampleValue capturedInstance = null;
			Action<SampleValue> captureDelegate = injected => capturedInstance = injected;

			provider.Provide(token);
			Assert.AreEqual(null, capturedInstance);

			provider.AddListener(captureDelegate);
			Assert.AreEqual(value, capturedInstance);
		}
	}
}
