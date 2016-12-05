using System.Configuration;
using System.IO;
using SourceAFIS.Simple;
using SourceAFIS.General;
using System.Collections.Generic;

namespace NoID.Biometrics
{
    class MatchDatabase
    {
        public MatchDatabase()
        { 
        }
        ~MatchDatabase()
        {
        }

        private Fingerprint _probe = null;
        
        static AfisEngine Afis = new AfisEngine();
        public int nextID = 1;

        List<Fingerprint> fingerprintList = new List<Fingerprint>();

        private static string DATABASE_PATH = ConfigurationManager.AppSettings.Get("DatabaseLocation");
       
        public void LoadTestFingerPrintImages(string imageDirectory)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(imageDirectory);
            Fingerprint fingerprint = null;
            if (dirInfo.Exists)
            {
                foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                {
                    foreach (DirectoryInfo dirSub in dir.GetDirectories())
                    {
                        foreach (DirectoryInfo dirSub2 in dirSub.GetDirectories()) { 
                            foreach (FileInfo file in dirSub2.GetFiles())
                            {
                                fingerprint = new Fingerprint();
                                fingerprint.AsBitmapSource = WpfIO.Load(file.FullName);
                                if (fingerprint.Image != null)
                                { 
                                    fingerprintList.Add(fingerprint);                        
                                    nextID = nextID + 1;
                                }
                            }
                        }
                    }   
                }
                _probe = fingerprint;
            }
        }

        public float SearchTest(Fingerprint probe = null)
        {
            if (probe == null)
                probe = _probe;

            Afis.ExtractFingerprint(probe);
            return Afis.IdentifyFinger(probe, fingerprintList);
        } 
    }
}
