using System;

namespace GameLibrary.Tests.API
{
	public interface ICrossDomainObject
	{
		string Name { get; }

		Func<float, ICrossDomainObject> FindTarget { get; set; }
		Action<string> Log { get; set; }

		bool SendMessage(float distance, string msg);
		bool ReceiveMessage(string msg);
	}
}
