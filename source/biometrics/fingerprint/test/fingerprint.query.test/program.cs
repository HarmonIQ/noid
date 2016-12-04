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
            string cmd = "";
            DateTime start = DateTime.Now;
            matchDB.LoadTestFingerPrintImages(FINGERPRINT_IMAGE_PATH);
            Console.WriteLine("Finished loading in " + (DateTime.Now - start).TotalSeconds.ToString() + " seconds");
            Console.WriteLine("Loaded " + matchDB.nextID);
            while(cmd.ToLower() != "q")
            {
                cmd = Console.ReadLine();
                if (cmd.ToLower() == "s")
                    Search();
            }
            
        }
        private static void Search()
        {
            DateTime start = DateTime.Now;
            float score = matchDB.SearchTest();
            Console.WriteLine("Match found. Finished searching in " + (DateTime.Now - start).TotalSeconds.ToString() + " seconds");
            Console.WriteLine("Score = " + score.ToString());
        }
    }
}
