using BulletPoolingExample.BridgeInterfaces;
using Obsidize.DependencyInjection;
using System;
using UnityEngine;

namespace BulletPoolingExample.BulletSystem
{
	/// <summary>
	/// Implementation details for a bullet.
	/// 
	/// These are hidden behind IBullet, so consumers of this class will 
	/// only use the intended external functionality (i.e. Launch())
	/// 
	/// Note that this implementation depends on IPlayer, which is provided in the DI system.
	/// The implementation does not need to care about who provides the IPlayer instance, or when it is provided.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public class Bullet : MonoBehaviour, IBullet
	{

		[SerializeField] private float _lifetime = 3f;
		[SerializeField] private float _speed = 15f;

		private Rigidbody _physicsBody;
		private float _lifetimeRemaining;
		private IPlayer _player;

		public Action<Bullet> onRelease;

		private void Awake()
		{
			_physicsBody = GetComponent<Rigidbody>();
			Injector.Main.RequireAndWatch<IPlayer>(OnUpdatePlayer);
		}

		private void OnDestroy()
		{
			Injector.Main.Unwatch<IPlayer>(OnUpdatePlayer);
		}

		private void OnValidate()
		{
			_lifetime = Mathf.Max(1f, _lifetime);
			_speed = Mathf.Max(1f, _speed);
		}

		private void Update()
		{

			if (_lifetimeRemaining <= 0f)
			{
				return;
			}

			_lifetimeRemaining = Mathf.Max(_lifetimeRemaining - Time.deltaTime, 0f);

			if (_lifetimeRemaining <= 0f)
			{
				onRelease?.Invoke(this);
			}
		}

		private void OnUpdatePlayer(IPlayer player)
		{
			_player = player;
		}

		public void Launch()
		{

			if (_player == null)
			{
				onRelease?.Invoke(this);
				return;
			}

			_lifetimeRemaining = _lifetime;
			transform.position = _player.BulletStartPosition;
			transform.forward = _player.AimDirection;

			gameObject.SetActive(true);

			_physicsBody.velocity = _player.AimDirection.normalized * _speed;
		}
	}
}
