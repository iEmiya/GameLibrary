using GameLibrary;
using GameLibrary.Tests.API;

// Not used namespaces because: CompilationErrorException as error CS7021: Cannot declare namespace in script code
[HotswapClass(typeof(IMultiImplObject), TwoImplObject.ClassFileSource)]
public class TwoImplObject : IMultiImplObject
{
	public const string ClassFileSource = "./Scripts/TwoImplObject.cs";

	public TwoImplObject()
	{
		Name = nameof(TwoImplObject);
	}

	public string Name { get; }
}
