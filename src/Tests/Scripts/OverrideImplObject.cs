using System;
using GameLibrary;
using GameLibrary.Tests.API;

// Not used namespaces because: CompilationErrorException as error CS7021: Cannot declare namespace in script code
[HotswapClass(typeof(IOverrideObject), OverrideImplObject.ClassFileSource, true)]
public class OverrideImplObject : IOverrideObject
{
	public const string ClassFileSource = "./Scripts/OverrideImplObject.cs";

	public OverrideImplObject() { }

	public long Ticks => DateTime.UtcNow.Ticks;

	public string OverrideMethod(string value)
	{
		throw new NotImplementedException();
	}
}
