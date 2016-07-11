using System;
using GameLibrary.Tests.API;
using NUnit.Framework;

namespace GameLibrary.Tests
{
	[TestFixture]
	public class TestDoubleImplObject : TestBase
	{
		[Test]
		public void CreateInstanceMutant()
		{
			IOverrideObject one;
			IMultiImplObject two;
			using (var instance = new HotswapCSharpScript().Load<TestDoubleImplObject>())
			{
				one = instance.CreateInstance<IOverrideObject, DoubleImplObject>();
				two = instance.CreateInstance<IMultiImplObject, DoubleImplObject>();
			}
			Assert.IsNotNull(one);
			Assert.IsNotNull(two);


			// IOverrideObject => IMultiImplObject
			var uuid = Guid.NewGuid().ToString("D");
			Assert.IsNull(one.OverrideMethod(uuid));

			var multiImpl = one as IMultiImplObject;
			Assert.IsNotNull(multiImpl);
			Assert.That(multiImpl.Name, Is.EqualTo(uuid));



			// IMultiImplObject => IOverrideObject
			Assert.IsNull(two.Name);
			var overrideObj = two as IOverrideObject;
			Assert.IsNotNull(multiImpl);

			uuid = Guid.NewGuid().ToString("D");
			Assert.IsNull(overrideObj.OverrideMethod(uuid));
			Assert.That(two.Name, Is.EqualTo(uuid));
		}
	}
}
