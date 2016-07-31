using System;
using NoID.Base.Data;
using NoID.Base.Algorithms;

namespace NoID.Base.Tests
{
	class HashTests
	{
		public string HashValue = "";

		public HashTests()
		{
			TestHashing ();
		}

		private void TestHashing()
		{
			HashValue = TestSHAHash("Johny-Smith-1971");
		}

		private static string TestSHAHash(string input)
		{
			sha256 SHA = new sha256(input, "salt-test");
			return SHA.Hash;
		}
	}
}

