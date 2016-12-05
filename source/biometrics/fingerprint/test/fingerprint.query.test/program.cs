using System;
using System.Configuration;
using System.IO;
using SourceAFIS.Simple;
using SourceAFIS.General;
using System.Collections.Generic;

namespace NoID.Biometrics
{
    class Program
    {
        private static string FINGERPRINT_IMAGE_PATH = ConfigurationManager.AppSettings.Get("FingerprintLocation");
        private static string FINGER_PROBE_IMAGE_PATH = ConfigurationManager.AppSettings.Get("ProbeLocation");
        
        private static MatchDatabase matchDB = new MatchDatabase();
        private static MatchProbes matchProbes = new MatchProbes();

        static void Main(string[] args)
        {
            string cmd = "";
            DateTime start = DateTime.Now;
            matchDB.LoadTestFingerPrintImages(FINGERPRINT_IMAGE_PATH);
            matchProbes.LoadProbeImages(FINGER_PROBE_IMAGE_PATH);

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
            Fingerprint probe = matchProbes.GetNextProbe();
            float score = matchDB.SearchTest(probe);
            Console.WriteLine("Match found. Finished searching in " + (DateTime.Now - start).TotalSeconds.ToString() + " seconds");
            Console.WriteLine("Score = " + score.ToString());
        }
    }
}
