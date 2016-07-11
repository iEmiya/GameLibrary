using System;
using GameLibrary.Tests.API;
using NUnit.Framework;

namespace GameLibrary.Tests
{
	[TestFixture]
	public class TestSingleImplObject : TestBase
	{
		[Test]
		public void CreateInstanceWithoutArguments()
		{
			ISingleImplObject d;
			using (var instance = new HotswapCSharpScript().Load<TestSingleImplObject>())
			{
				d = instance.CreateInstance<ISingleImplObject>();
			}
			Assert.IsNotNull(d);
			Assert.IsInstanceOf<SingleImplObject>(d);
			Assert.That(d.Name, Is.EqualTo(nameof(SingleImplObject)));
		}

		[Test]
		public void CreateInstanceWithoutArgumentsAndDispose()
		{
			var instance = new HotswapCSharpScript().Load<TestSingleImplObject>();
			var d = instance.CreateInstance<ISingleImplObject>();
			instance.Dispose();
			Assert.IsNotNull(d);
			Assert.Throws<ObjectDisposedException>(() => instance.CreateInstance<ISingleImplObject>());
		}

		[Test]
		public void CreateInstanceOneArgument(
			[Values("a","aa", "aaa")] string arg1)
		{
			ISingleImplObject d;
			using (var instance = new HotswapCSharpScript().Load<TestSingleImplObject>())
			{
				d = instance.CreateInstance<ISingleImplObject>(arg1);
			}
			Assert.IsNotNull(d);
			Assert.IsInstanceOf<SingleImplObject>(d);
			Assert.That(d.Name, Is.EqualTo(arg1));
		}

		[Test, Pairwise]
		public void CreateInstanceTwoArguments(
			[Values("a", "aa", "aaa")] string arg1,
			[Values("b", "bb", "bbb")] string arg2)
		{
			ISingleImplObject d;
			using (var instance = new HotswapCSharpScript().Load<TestSingleImplObject>())
			{
				d = instance.CreateInstance<ISingleImplObject>(arg1, arg2);
			}
			Assert.IsNotNull(d);
			Assert.IsInstanceOf<SingleImplObject>(d);
			Assert.That(d.Name, Is.EqualTo(arg1 + arg2));
		}

		[Test, Pairwise]
		public void CreateInstanceThrArguments(
			[Values("a", "aa", "aaa")] string arg1,
			[Values("b", "bb", "bbb")] string arg2,
			[Values("c", "cc", "ccc")] string arg3)
		{
			ISingleImplObject d;
			using (var instance = new HotswapCSharpScript().Load<TestSingleImplObject>())
			{
				d = instance.CreateInstance<ISingleImplObject>(arg1, arg2, arg3);
			}
			Assert.IsNotNull(d);
			Assert.IsInstanceOf<SingleImplObject>(d);
			Assert.That(d.Name, Is.EqualTo(arg1 + arg2 + arg3));
		}

		[Test, Pairwise]
		public void CreateInstanceFourArguments(
			[Values("a", "aa", "aaa")] string arg1,
			[Values("b", "bb", "bbb")] string arg2,
			[Values("c", "cc", "ccc")] string arg3,
			[Values("d", "dd", "ddd")] string arg4)
		{
			ISingleImplObject d;
			using (var instance = new HotswapCSharpScript().Load<TestSingleImplObject>())
			{
				d = instance.CreateInstance<ISingleImplObject>(arg1, arg2, arg3, arg4);
			}
			Assert.IsNotNull(d);
			Assert.IsInstanceOf<SingleImplObject>(d);
			Assert.That(d.Name, Is.EqualTo(arg1 + arg2 + arg3 + arg4));
		}

		[Test]
		public void SetName(
			[Values("a", "aa", "aaa")] string name)
		{
			ISingleImplObject d;
			using (var instance = new HotswapCSharpScript().Load<TestSingleImplObject>())
			{
				d = instance.CreateInstance<ISingleImplObject>();
			}
			Assert.IsNotNull(d);
			Assert.IsInstanceOf<SingleImplObject>(d);
			Assert.That(d.Name, Is.EqualTo(nameof(SingleImplObject)));

			d.SetName(name);
			Assert.That(d.Name, Is.EqualTo(name));
		}

		[Test]
		public void SetNameWithLog(
			[Values("a", "ab", "abc")] string name)
		{
			ISingleImplObject d;
			using (var instance = new HotswapCSharpScript().Load<TestSingleImplObject>())
			{
				d = instance.CreateInstance<ISingleImplObject>();
			}
			Assert.IsNotNull(d);
			Assert.IsInstanceOf<SingleImplObject>(d);
			Assert.That(d.Name, Is.EqualTo(nameof(SingleImplObject)));

			string reverse = null;
			d.Log = new Func<string, string>(x =>
			{
				char[] arr = x.ToCharArray();
				Array.Reverse(arr);
				reverse = new string(arr);
				return reverse;
			});
			d.SetName(name);
			Assert.That(d.Name, Is.EqualTo(name + reverse));
		}
	}
}
