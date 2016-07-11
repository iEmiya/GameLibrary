using System;

namespace GameLibrary.Tests.API
{
	public interface ISingleImplObject
	{
		string Name { get; }
		void SetName(string name);
		Func<string, string> Log { get; set; }
	}
}
