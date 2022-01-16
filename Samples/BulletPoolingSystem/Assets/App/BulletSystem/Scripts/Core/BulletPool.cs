using BulletPoolingExample.BridgeInterfaces;
using Obsidize.DependencyInjection;
using System.Collections.Generic;
using UnityEngine;

namespace BulletPoolingExample.BulletSystem
{
	/// <summary>
	/// IBulletPool implementation that is provided to the DI system.
	/// 
	/// Note: this uses a custom-rolled pooling system for the sake of clarity,
	/// but ideally you should use unity's 2021+ ObjectPool system instead.
	/// 
	/// Tip: try deleting the scene instance, and then re-inserting it.
	/// You will notice that the Player script re-binds to the new instance 
	/// automatically since it is using a Watch() listener (Huzzah!)
	/// </summary>
	[DisallowMultipleComponent]
	public class BulletPool : InjectionTokenSource<IBulletPool>, IBulletPool
	{

		[SerializeField] private Bullet _bulletPrefab;
		[SerializeField] private int _maxInstances = 25;

		private readonly List<Bullet> _instances = new List<Bullet>();
		private readonly Stack<Bullet> _available = new Stack<Bullet>();

		protected override IBulletPool GetInjectionTokenValue() => this;

		protected override void Awake()
		{
			InstantiateBullets();
			base.Awake();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			DestroyBullets();
		}

		private void OnValidate()
		{
			_maxInstances = Mathf.Max(1, _maxInstances);
		}

		public IBullet GetBullet()
		{
			return _available.Count > 0 ? _available.Pop() : null;
		}

		private void OnReleaseInstance(Bullet instance)
		{
			if (instance == null)
			{
				return;
			}

			instance.gameObject.SetActive(false);
			_available.Push(instance);
		}

		private void InstantiateBullets()
		{

			for (int i = 0; i < _maxInstances; i++)
			{

				// Note: we don't attach the bullet as a child of this
				// object so that this object does not interfere with the
				// position / trajectory of the bullet when it launches.
				var instance = Instantiate(_bulletPrefab);
				instance.gameObject.SetActive(false);
				instance.onRelease = OnReleaseInstance;

				_instances.Add(instance);
				_available.Push(instance);
			}
		}

		private void DestroyBullets()
		{

			_available.Clear();

			foreach (var bullet in _instances)
			{
				if (bullet == null) continue;
				Destroy(bullet.gameObject);
			}

			_instances.Clear();
		}
	}
}
