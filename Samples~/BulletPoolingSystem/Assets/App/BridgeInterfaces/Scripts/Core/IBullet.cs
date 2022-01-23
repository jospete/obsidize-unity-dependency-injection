namespace BulletPoolingExample.BridgeInterfaces
{
	/// <summary>
	/// Bare minimum template for what IBulletPool will return.
	/// 
	/// We don't want to just use the Bullet class implementation because:
	/// A) that would pollute the scope of what the consumer can do with the bullet (and cause spaghetti code), and
	/// B) using Bullet would make the BridgeInterfaces assembly definition depend on another module, which is not allowed
	/// </summary>
	public interface IBullet
	{
		void Launch();
	}
}
