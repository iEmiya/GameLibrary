using System;

namespace GameLibrary.Tests
{
	public class HotswapRelation
	{
		public Type InterfaceType { get; }
		public Type RealObjectType { get; }

		internal HotswapRelation(Type interfaceType, Type realObjectType)
		{
			InterfaceType = interfaceType;
			RealObjectType = realObjectType;
		}
	}
}
