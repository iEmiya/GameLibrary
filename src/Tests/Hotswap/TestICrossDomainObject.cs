using System;
using GameLibrary.Tests.API;
using NUnit.Framework;

namespace GameLibrary.Tests.Hotswap
{
	[TestFixture]
	public class TestICrossDomainObject : TestBase
	{
		[Test]
		public void CreateInstances()
		{
			ICrossDomainObject mouse;
			ICrossDomainObject wolf;

			using (var instance = new HotswapCSharpScript().Load<TestMultiImplObject>())
			{
				mouse = instance.CreateInstance<ICrossDomainObject, MouseCrossDomainObject>();
			}

			using (var instance = new HotswapCSharpScript().Load<TestMultiImplObject>())
			{
				wolf = instance.CreateInstance<ICrossDomainObject, WolfCrossDomainObject>();
			}

			string log = null;
			mouse.Log = new Action<string>(msg => log = msg);
			wolf.FindTarget = new Func<float, ICrossDomainObject>(dist => mouse);
			wolf.SendMessage(2.0f, "wooooo....");

			Assert.That(log, Is.EqualTo("Mouse receive wooooo...."));
		}
	}
}
