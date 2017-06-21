// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using NoID.Biometrics.Managers;
using DPUruNet;

namespace NoID.Match.Database.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Fingerprint scanner is being setup.");
            try
            {
                MatchProbesTest test = new MatchProbesTest();
                Console.WriteLine("Fingerprint scanner detected.");
                test.LoadTestFingerPrintImages(@"F:\fingerprobes");
                Console.WriteLine("Loaded 100 test fingerprints.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            Console.WriteLine("Scan you finger to trigger a search.");
            Console.ReadLine();
        }
    }
}
