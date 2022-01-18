using BulletPoolingExample.BulletSystem;
using BulletPoolingExample.Entities.Player;
using Obsidize.DependencyInjection;
using UnityEngine;

namespace BulletPoolingExample.Globals
{

	public class MainInjectionModule : MonoBehaviour
	{

		private InjectionTokenSourceModuleContext _moduleContext;
		[SerializeField] private BulletPool _bulletPoolPrefab;
		[SerializeField] private Player _playerPrefab;

		private void Awake()
		{
			_moduleContext = new InjectionTokenSourceModuleContext()
				.Provide(_bulletPoolPrefab)
				.Provide(_playerPrefab);
		}

		private void OnDestroy()
		{
			_moduleContext.Dispose();
		}
	}
}
