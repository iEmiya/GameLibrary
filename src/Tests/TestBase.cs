using System;
using System.IO;
using GameLibrary.Tests.Hotswap;
using NUnit.Framework;

namespace GameLibrary.Tests
{
	public abstract class TestBase
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			var dir = Path.GetDirectoryName(new Uri(typeof(TestHotswapCSharpScript).Assembly.CodeBase).LocalPath);
			Directory.SetCurrentDirectory(dir);
		}
	}
}
