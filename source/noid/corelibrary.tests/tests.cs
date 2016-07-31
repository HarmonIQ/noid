using System;
using NoID.Base.Data;
using NoID.Base.Algorithms;

namespace NoID.Base.Tests
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.Write (TestSHAHash("Test Hash"));
			Console.Read ();
		}

		private static string TestSHAHash(string input)
		{
			sha256 SHA = new sha256(input,"salt-test");
			return SHA.Hash;
		}
	}
}