using System;
using NoID.Base.Data;
using NoID.Base.Algorithms;

namespace NoID.Base.Tests
{
	class NoIDTests
	{
		public static void Main (string[] args)
		{
			HashTests htest = new HashTests ();
			Console.WriteLine (htest.HashValue);
			Console.Read ();
		}
	}
}