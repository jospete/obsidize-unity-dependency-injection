using System;

namespace Obsidize.DependencyInjection
{
	/// <summary>
	/// Baseline for maintaining a collection of watchers.
	/// </summary>
	public interface IInjectionTokenWatcher
	{
		Type TokenType { get; }
		void Watch();
		void Unwatch();
	}
}
