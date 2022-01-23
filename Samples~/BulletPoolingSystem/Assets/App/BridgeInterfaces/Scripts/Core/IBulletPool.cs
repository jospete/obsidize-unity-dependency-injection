namespace BulletPoolingExample.BridgeInterfaces
{
	/// <summary>
	/// Bare minimum template for what a bullet pool should do.
	/// 
	/// Smaller interface definitions are better in the bridged module, because 
	/// they restrict the consumer to only "intended access" options.
	/// </summary>
	public interface IBulletPool
	{
		IBullet GetBullet();
	}
}
