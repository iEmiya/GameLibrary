using System;
using System.Collections.Generic;
using GameLibrary.Tests.API;
using NUnit.Framework;

namespace GameLibrary.Tests
{
	[TestFixture]
	public class TestHotswapCSharpScript : TestBase
	{
		#region Relations

		internal static readonly List<Type> Interfaces = new List<Type>()
		{
			typeof(ISingleImplObject),
			typeof(IMultiImplObject),
			typeof(IOverrideObject),
			typeof(ICrossDomainObject),
		};

		internal static readonly List<HotswapRelation> Relations = new List<HotswapRelation>()
		{
			new HotswapRelation(typeof(ISingleImplObject), typeof(SingleImplObject)),
			new HotswapRelation(typeof(IMultiImplObject), typeof(OneImplObject)),
			new HotswapRelation(typeof(IMultiImplObject), typeof(TwoImplObject)),
			new HotswapRelation(typeof(IOverrideObject), typeof(OverrideImplObject)),
			new HotswapRelation(typeof(IOverrideObject), typeof(DoubleImplObject)),
			new HotswapRelation(typeof(IMultiImplObject), typeof(DoubleImplObject)),
			new HotswapRelation(typeof(ICrossDomainObject), typeof(MouseCrossDomainObject)),
			new HotswapRelation(typeof(ICrossDomainObject), typeof(WolfCrossDomainObject)),
		};

		#endregion

		[Test]
		public void ContainByTSub([ValueSource(nameof(Interfaces))] Type interfaceType)
		{
			bool d;
			using (var instance = new HotswapCSharpScript())
			{
				instance.Load<TestHotswapCSharpScript>();
				d = instance.Contain(interfaceType);
			}
			Assert.True(d);
		}

		[Test]
		public void ContainByTSubAndDispose([ValueSource(nameof(Interfaces))] Type interfaceType)
		{
			var instance = new HotswapCSharpScript().Load<TestHotswapCSharpScript>();
			var d = instance.Contain(interfaceType);
			instance.Dispose();
			Assert.True(d);
			Assert.Throws<ObjectDisposedException>(() => instance.Contain(interfaceType));
		}

		[Test]
		public void ContainByTSub2T([ValueSource(nameof(Relations))] HotswapRelation relation)
		{
			Console.WriteLine($"{relation.InterfaceType}<-{relation.RealObjectType}");
			bool d;
			using (var instance = new HotswapCSharpScript())
			{
				instance.Load<TestHotswapCSharpScript>();
				d = instance.Contain(relation.InterfaceType, relation.RealObjectType);
			}
			Assert.True(d);
		}

		[Test]
		public void ContainByTSub2TAndDispose([ValueSource(nameof(Relations))] HotswapRelation relation)
		{
			Console.WriteLine($"{relation.InterfaceType}<-{relation.RealObjectType}");
			var instance = new HotswapCSharpScript().Load<TestHotswapCSharpScript>();
			var d = instance.Contain(relation.InterfaceType, relation.RealObjectType);
			instance.Dispose();
			Assert.True(d);
			Assert.Throws<ObjectDisposedException>(() => instance.Contain(relation.InterfaceType, relation.RealObjectType));
		}
	}
}
