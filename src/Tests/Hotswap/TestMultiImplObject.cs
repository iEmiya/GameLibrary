using GameLibrary.Tests.API;
using NUnit.Framework;

namespace GameLibrary.Tests.Hotswap
{
	[TestFixture]
	public class TestMultiImplObject : TestBase
	{
		[Test]
		public void CreateInstanceOneImplObject()
		{
			IMultiImplObject d;
			using (var instance = new HotswapCSharpScript().Load<TestMultiImplObject>())
			{
				d = instance.CreateInstance<IMultiImplObject, OneImplObject>();
			}
			Assert.IsNotNull(d);
			Assert.IsInstanceOf<OneImplObject>(d);
			Assert.That(d.Name, Is.EqualTo(nameof(OneImplObject)));
		}

		[Test]
		public void CreateInstanceTwoImplObject()
		{
			IMultiImplObject d;
			using (var instance = new HotswapCSharpScript().Load<TestMultiImplObject>())
			{
				d = instance.CreateInstance<IMultiImplObject, TwoImplObject>();
			}
			Assert.IsNotNull(d);
			Assert.IsInstanceOf<TwoImplObject>(d);
			Assert.That(d.Name, Is.EqualTo(nameof(TwoImplObject)));
		}
	}
}
