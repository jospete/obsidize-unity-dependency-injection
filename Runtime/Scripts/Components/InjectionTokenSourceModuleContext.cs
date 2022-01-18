using System;
using System.Collections.Generic;

namespace Obsidize.DependencyInjection
{
	/// <summary>
	/// Container of InjectionTokenSource prefabs that 
	/// allow for on-demand token source creation.
	/// 
	/// The prefabs given to Provide() will not be instantiated
	/// unless they are explicitly asked for by their target provider 
	/// (via the OnTokenRequest event).
	/// 
	/// See InjectionTokenSourceInstantiationContext for more granular details.
	/// </summary>
	public class InjectionTokenSourceModuleContext : IDisposable
	{

		private readonly Dictionary<Type, InjectionTokenSourceInstantiationContext> _contexts
			= new Dictionary<Type, InjectionTokenSourceInstantiationContext>();

		private readonly Injector _injector;

		public InjectionTokenSourceModuleContext(Injector injector)
		{
			_injector = injector;
		}

		public InjectionTokenSourceModuleContext() : this(Injector.Main)
		{
		}

		public void Dispose()
		{

			if (_contexts.Count <= 0)
			{
				return;
			}

			foreach (var context in _contexts.Values)
			{
				context?.Dispose();
			}

			_contexts.Clear();
		}

		public InjectionTokenSourceModuleContext Provide<T>(
			InjectionTokenSource<T> sourcePrefab,
			int maxAllowedInstantiations = 1
		) where T : class
		{

			var key = typeof(T);

			if (_contexts.ContainsKey(key))
			{
				throw new InvalidOperationException($"Duplicate provision of token type {key}");
			}

			var context = _contexts[key] = new InjectionTokenSourceInstantiationContext(
				_injector.GetProvider<T>(),
				sourcePrefab,
				maxAllowedInstantiations
			);

			context.BeginWatchingTokenRequests();

			return this;
		}
	}
}
