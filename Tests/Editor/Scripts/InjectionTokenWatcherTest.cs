using NUnit.Framework;
using System;

namespace Obsidize.DependencyInjection.Testing.Editor
{
	public class InjectionTokenWatcherTest
	{

		class SampleToken
		{
		}

		[Test]
		public void HasCoreMetadataReferencesForTheTokenBeingWatched()
		{
			Action<SampleToken> watcherDelegate = _ => { };
			var injector = new Injector();
			var watcher = new InjectionTokenWatcher<SampleToken>(injector, watcherDelegate);

			Assert.AreEqual(watcher.TokenType, typeof(SampleToken));
			Assert.AreEqual(watcher.SourceInjector, injector);

			Assert.DoesNotThrow(() => watcher.Watch());
			Assert.DoesNotThrow(() => watcher.Unwatch());
		}

		[Test]
		public void UsesTheMainInjectorByDefault()
		{
			Action<SampleToken> watcherDelegate = _ => { };
			var watcher = new InjectionTokenWatcher<SampleToken>(watcherDelegate);

			Assert.AreEqual(watcher.SourceInjector, Injector.Main);
		}
	}
}
