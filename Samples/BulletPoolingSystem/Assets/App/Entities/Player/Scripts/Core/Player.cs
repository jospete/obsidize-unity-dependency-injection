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

		private IBulletPool _bulletPool;

		public Vector3 BulletStartPosition => _bulletStart.position;
		public Vector3 AimDirection => BulletStartPosition - transform.position;

		protected override IPlayer GetInjectionTokenValue() => this;

		protected override void Awake()
		{
			Injector.Main.RequireAndWatch<IBulletPool>(OnBulletPoolUpdate);
			base.Awake();
		}

		protected override void OnDestroy()
		{
			Injector.Main.Unwatch<IBulletPool>(OnBulletPoolUpdate);
			base.OnDestroy();
		}

		private void OnBulletPoolUpdate(IBulletPool pool)
		{
			_bulletPool = pool;
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
