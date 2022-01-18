namespace Obsidize.DependencyInjection
{
	public interface IInjectionTokenSource : ITokenTypeRef
	{
		IInjectionTokenProvider Provider { get; }
	}
}