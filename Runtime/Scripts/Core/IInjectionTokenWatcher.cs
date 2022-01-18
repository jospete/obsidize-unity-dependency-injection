using System;

namespace Obsidize.DependencyInjection
{
	/// <summary>
	/// Baseline for maintaining a collection of watchers.
	/// </summary>
	public interface IInjectionTokenWatcher : ITokenTypeRef
	{
		void Watch();
		void Unwatch();
	}
}
