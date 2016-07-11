using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace GameLibrary
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class HotswapClassAttribute : Attribute
	{
		public HotswapClassAttribute(Type interfaceType, string classFileSource, bool overwriteWhenBoot = false)
		{
			Contract.Assert(interfaceType != null);
			Contract.Assert(!string.IsNullOrEmpty(classFileSource));
			InterfaceType = interfaceType;
			ClassFileSource = Path.GetFullPath(classFileSource);
			OverwriteWhenBoot = overwriteWhenBoot;
		}

		public Type InterfaceType { get; }
		public string ClassFileSource { get; }
		public bool OverwriteWhenBoot { get; }
	}
}
