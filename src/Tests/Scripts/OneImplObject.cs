using GameLibrary;
using GameLibrary.Tests.API;

// Not used namespaces because: CompilationErrorException as error CS7021: Cannot declare namespace in script code
[HotswapClass(typeof(IMultiImplObject), OneImplObject.ClassFileSource)]
public class OneImplObject : IMultiImplObject
{
	public const string ClassFileSource = "./Scripts/OneImplObject.cs";

	public OneImplObject()
	{
		Name = nameof(OneImplObject);
	}

	public string Name { get; }
}
