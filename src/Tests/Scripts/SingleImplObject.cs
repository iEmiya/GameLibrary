using System;
using GameLibrary;
using GameLibrary.Tests.API;

// Not used namespaces because: CompilationErrorException as error CS7021: Cannot declare namespace in script code
[HotswapClass(typeof(ISingleImplObject), SingleImplObject.ClassFileSource)]
public class SingleImplObject : ISingleImplObject
{
	public const string ClassFileSource = "./Scripts/SingleImplObject.cs";

	public SingleImplObject()
	{
		Name = nameof(SingleImplObject);
	}

	public SingleImplObject(object arg1)
	{
		Name = (string)arg1;
	}

	public SingleImplObject(object arg1, object arg2)
	{
		Name = (string)arg1 + (string)arg2;
	}

	public SingleImplObject(object arg1, object arg2, object arg3)
	{
		Name = (string)arg1 + (string)arg2 + (string)arg3;
	}

	public SingleImplObject(object arg1, object arg2, object arg3, object arg4)
	{
		Name = (string)arg1 + (string)arg2 + (string)arg3 + (string)arg4;
	}

	public string Name { get; private set; }

	public void SetName(string name)
	{
		Name = name + Log?.Invoke(name);
	}

	public Func<string, string> Log { get; set; }
}
