using System.Configuration;
using System.IO;
using SourceAFIS.Simple;
using SourceAFIS.General;
using System.Collections.Generic;
using SourceAFIS.Templates;
using SourceAFIS.Extraction;

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

        private static int MATCH_THRESHOLD = 50;
        private Fingerprint _probe = null;
        
        static AfisEngine Afis = new AfisEngine();
        public ulong nextID = 1;

        List<Template> fingerprintList = new List<Template>();
        List<PatientFingerprintMinutia> dbFingerprintMinutiaList = new List<PatientFingerprintMinutia>();

        private static string DATABASE_PATH = ConfigurationManager.AppSettings.Get("DatabaseLocation");

        Extractor Extractor = new Extractor();

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
                                    Afis.ExtractFingerprint(fingerprint);        
                                    fingerprintList.Add(fingerprint.GetTemplate());
                                    PatientFingerprintMinutia dbFingerprintMinutia = new PatientFingerprintMinutia(nextID, 1, fingerprint.GetTemplate());
                                    dbFingerprintMinutiaList.Add(dbFingerprintMinutia);
                                    nextID = nextID + 1;
                                }
                            }
                        }
                    }   
                }
                _probe = fingerprint;
            }
            SerializeDatabaseProto serialize = new SerializeDatabaseProto();
            serialize.WriteToDisk(DATABASE_PATH + @"finger.hive.0001.biodb", dbFingerprintMinutiaList);
        }

        public float SearchTest(Fingerprint probe = null)
        {
            if (probe == null)
                probe = _probe;

            Afis.ExtractFingerprint(probe);
            Afis.Threshold = MATCH_THRESHOLD;
            return Afis.IdentifyFinger(probe.GetTemplate(), fingerprintList);
        }
    }
}
