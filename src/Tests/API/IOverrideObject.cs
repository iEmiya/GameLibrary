namespace GameLibrary.Tests.API
{
	public interface IOverrideObject
	{
		long Ticks { get; }
		string OverrideMethod(string value);
	}
}
