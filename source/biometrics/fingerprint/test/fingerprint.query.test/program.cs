using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace NoID.Biometrics
{
    class Program
    {
        private static string FINGERPRINT_IMAGE_PATH = ConfigurationManager.AppSettings.Get("FingerprintLocation");
        private static MatchDatabase matchDB = new MatchDatabase();
        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;
            matchDB.LoadTestFingerPrintImages(FINGERPRINT_IMAGE_PATH);
            Console.WriteLine("Finished loading in " + (DateTime.Now - start).TotalSeconds.ToString() + " seconds");
            Console.WriteLine("Loaded " + matchDB.nextID);
            start = DateTime.Now;
            float score = matchDB.SearchTest();
            Console.WriteLine("Finished searching in " + (DateTime.Now - start).TotalSeconds.ToString() + " seconds");
            Console.WriteLine("Score = " + score.ToString());
            Console.Read();
        }
    }
}
