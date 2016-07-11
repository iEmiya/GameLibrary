using System;
using GameLibrary;
using GameLibrary.Tests.API;

// Not used namespaces because: CompilationErrorException as error CS7021: Cannot declare namespace in script code
[HotswapClass(typeof(IOverrideObject), DoubleImplObject.ClassFileSource, true)]
[HotswapClass(typeof(IMultiImplObject), DoubleImplObject.ClassFileSource, true)]
public class DoubleImplObject : IOverrideObject, IMultiImplObject
{
	public const string ClassFileSource = "./Scripts/DoubleImplObject.cs";

	public long Ticks => DateTime.UtcNow.Ticks;

	public string OverrideMethod(string value)
	{
		var tmp = Name;
		Name = value;
		return tmp;
	}

	public string Name { get; set; }
}
