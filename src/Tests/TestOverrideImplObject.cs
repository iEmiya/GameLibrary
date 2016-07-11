using System;
using System.IO;
using System.Threading.Tasks;
using GameLibrary.Tests.API;
using NUnit.Framework;

namespace GameLibrary.Tests
{
	[TestFixture]
	public class TestOverrideImplObject : TestBase
	{
		[Test, Timeout(60000)]
		public void CreateInstance()
		{
			IOverrideObject d;
			using (var instance = new HotswapCSharpScript().Load<TestOverrideImplObject>())
			{
				var ticks = DateTime.UtcNow.Ticks;
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

				while (true)
				{
					d = instance.CreateInstance<IOverrideObject>();
					if (d.Ticks == ticks) break;
					Task.Delay(1000);
				}

			}
			Assert.IsNotNull(d);
			Assert.IsInstanceOf<IOverrideObject>(d);
			Assert.Throws<NotImplementedException>(() => d.OverrideMethod(null));
		}

		[Test, Timeout(60000)]
		public void CreateInstanceAndImplMethod()
		{
			IOverrideObject d;
			using (var instance = new HotswapCSharpScript().Load<TestOverrideImplObject>())
			{
				var ticks = DateTime.UtcNow.Ticks;
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

				while (true)
				{
					d = instance.CreateInstance<IOverrideObject>();
					if (d.Ticks == ticks) break;
					Task.Delay(1000);
				}

			}
			Assert.IsNotNull(d);
			Assert.IsInstanceOf<IOverrideObject>(d);
			var uuid = Guid.NewGuid().ToString("D");
			var it = d.OverrideMethod(uuid);
			Assert.That(it, Is.EqualTo(uuid));
		}
	}
}
