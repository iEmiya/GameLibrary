using System;
using System.IO;
using GameLibrary.Tests.API;
using NUnit.Framework;

namespace GameLibrary.Tests.Hotswap
{
	[TestFixture]
	public class TestOverrideImplObject : TestBase
	{
		[Test]
		public void CreateInstance()
		{
			var ticks = DateTime.UtcNow.Ticks;
			IOverrideObject d;
			using (var instance = new HotswapCSharpScript().Load<TestOverrideImplObject>())
			{
				
				var classFileSource = Path.GetFullPath(OverrideImplObject.ClassFileSource);
				File.WriteAllText(classFileSource,
@"using System;
using GameLibrary;
using GameLibrary.Tests.API;

// Not used namespaces because: CompilationErrorException as error CS7021: Cannot declare namespace in script code
[HotswapClass(typeof(IOverrideObject), OverrideImplObject.ClassFileSource, true)]
public class OverrideImplObject : IOverrideObject
{
	public const string ClassFileSource = ""./Scripts/OverrideImplObject.cs"";

	public long Ticks => " + ticks + @";

	public OverrideImplObject() { }

		public string OverrideMethod(string value)
		{
			throw new NotImplementedException();
		}
	}
");
				instance.ReloadScript();
				d = instance.CreateInstance<IOverrideObject>();
			}
			Assert.IsNotNull(d);
			Assert.IsInstanceOf<IOverrideObject>(d);
			Assert.That(d.Ticks, Is.EqualTo(ticks));
			Assert.Throws<NotImplementedException>(() => d.OverrideMethod(null));
		}

		[Test]
		public void CreateInstanceAndImplMethod()
		{
			var ticks = DateTime.UtcNow.Ticks;
			IOverrideObject d;
			using (var instance = new HotswapCSharpScript().Load<TestOverrideImplObject>())
			{
				var classFileSource = Path.GetFullPath(OverrideImplObject.ClassFileSource);
				File.WriteAllText(classFileSource,
@"using System;
using GameLibrary;
using GameLibrary.Tests.API;

// Not used namespaces because: CompilationErrorException as error CS7021: Cannot declare namespace in script code
[HotswapClass(typeof(IOverrideObject), OverrideImplObject.ClassFileSource, true)]
public class OverrideImplObject : IOverrideObject
{
	public const string ClassFileSource = ""./Scripts/OverrideImplObject.cs"";

	public long Ticks => " + ticks + @";

	public OverrideImplObject() { }

		public string OverrideMethod(string value)
		{
			return value;
		}
	}
");
				instance.ReloadScript();
				d = instance.CreateInstance<IOverrideObject>();
			}
			Assert.IsNotNull(d);
			Assert.IsInstanceOf<IOverrideObject>(d);
			Assert.That(d.Ticks, Is.EqualTo(ticks));
			var uuid = Guid.NewGuid().ToString("D");
			var it = d.OverrideMethod(uuid);
			Assert.That(it, Is.EqualTo(uuid));
		}
	}
}
