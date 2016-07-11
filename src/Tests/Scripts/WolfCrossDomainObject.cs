using System;
using GameLibrary;
using GameLibrary.Tests.API;

// Not used namespaces because: CompilationErrorException as error CS7021: Cannot declare namespace in script code
[HotswapClass(typeof(ICrossDomainObject), WolfCrossDomainObject.ClassFileSource, true)]
public class WolfCrossDomainObject : ICrossDomainObject
{
	public const string ClassFileSource = "./Scripts/WolfCrossDomainObject.cs";

	public string Name => "Wolf";

	public Func<float, ICrossDomainObject> FindTarget { get; set; }
	public Action<string> Log { get; set; }

	public bool SendMessage(float distance, string msg)
	{
		if (distance < 1.0f) distance = 1.0f;
		if (distance > 50.0f) distance = 50.0f;
		var target = FindTarget?.Invoke(distance);
		if (target == null) return false;
		
		return target.ReceiveMessage(msg);
	}

	public bool ReceiveMessage(string msg)
	{
		Log?.Invoke($"{Name} receive {msg}");
		return true;
	}
}