using BulletPoolingExample.BridgeInterfaces;
using Obsidize.DependencyInjection;
using UnityEngine;

namespace BulletPoolingExample.Entities.Player
{
	/// <summary>
	/// IPlayer implementation that is provided to the DI system.
	/// 
	/// Note that this implementation also depends on IBulletPool,
	/// but will NOT cause circular assembly reference errors since
	/// the DI system and bridge interfaces act as a hard-line between
	/// the BulletSystem module and the Entities.Player module.
	/// </summary>
	[DisallowMultipleComponent]
	public class Player : InjectionTokenSource<IPlayer>, IPlayer
	{

		[SerializeField] private Transform _bulletStart;

		public Vector3 BulletStartPosition => _bulletStart.position;
		public Vector3 AimDirection => BulletStartPosition - transform.position;

		private BehaviourInjectionContext _injectionContext;
		private IBulletPool _bulletPool;

		protected override void Awake()
		{
			base.Awake();
			_injectionContext = new BehaviourInjectionContext(this)
				.Inject<IBulletPool>(OnUpdateBulletPool);
		}

		private void OnUpdateBulletPool(IBulletPool v)
		{
			_bulletPool = v;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_injectionContext.Dispose();
		}

		private void Update()
		{
			CheckForFire();
		}

		private void CheckForFire()
		{

			if (!Input.GetMouseButtonDown(0))
			{
				return;
			}

			if (_bulletPool == null)
			{
				return;
			}

			var bullet = _bulletPool.GetBullet();

			if (bullet == null)
			{
				return;
			}

			bullet.Launch();
		}
	}
}
