// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using SourceAFIS.Templates;

namespace NoID.Match.Database.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Fingerprint scanner is being setup.");
            MatchProbesTest test = new MatchProbesTest();
            test.FingerCaptured += FingerCaptured;
            test.GoodPairFound += GoodPairFound;
            test.DatabaseMatchFound += DatabaseMatchFound;
            test.DatabaseMatchError += DatabaseMatchError;

            try
            {
                Console.WriteLine("Fingerprint scanner detected.");
                test.LoadTestFingerPrintImages(@"F:\fingerprobes", true);
                Console.WriteLine("Loaded test fingerprints." + test.Count.ToString());
                //test.LoadTestMinutiaDatabase(@"C:\MatchDatabase\finger.hive.0001.biodb");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            Console.WriteLine("Scan you finger to trigger a search.");
            Console.ReadLine();
        }

        static void FingerCaptured(object sender, EventArgs e)
        {
            Template temp = (Template)sender;

        }

        static void GoodPairFound(object sender, EventArgs e)
        {
            Template temp = (Template)sender;

        }

        static void DatabaseMatchFound(object sender, EventArgs e)
        {
            Template temp = (Template)sender;

        }

        static void DatabaseMatchError(object sender, EventArgs e)
        {
            Template temp = (Template)sender;

        }
    }
}
