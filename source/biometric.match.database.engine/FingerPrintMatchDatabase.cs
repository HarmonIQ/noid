// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.IO;
using System.Configuration;
using SourceAFIS.Simple;
using SourceAFIS.General;
using System.Collections.Generic;
using SourceAFIS.Templates;
using SourceAFIS.Extraction;
using NoID.FHIR.Profile;

namespace NoID.Match.Engine.FingerPrint
{
    class FingerPrintMatchDatabase
    {
        static AfisEngine Afis = new AfisEngine();
        private static int MATCH_THRESHOLD = 50;
        private static string DATABASE_PATH = ConfigurationManager.AppSettings.Get("DatabaseLocation");

        private Extractor Extractor = new Extractor();
        private Fingerprint _probe = null;
        private Exception _exception;
        public ulong nextID = 1;
        private List<Template> fingerprintList = new List<Template>();
        private List<FingerPrintMinutias> dbFingerprintMinutiaList = new List<FingerPrintMinutias>();

        public FingerPrintMatchDatabase()
        {
        }
        ~FingerPrintMatchDatabase()
        {
        }

        public bool AddFingerPrintMinutias(FingerPrintMinutias newFingerPrintMinutias)
        {
            try
            {
                dbFingerprintMinutiaList.Add(newFingerPrintMinutias);
            }
            catch (Exception ex)
            {
                _exception = ex;
                return false;
            }
            return true;
        }

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
                        foreach (DirectoryInfo dirSub2 in dirSub.GetDirectories())
                        {
                            foreach (FileInfo file in dirSub2.GetFiles())
                            {
                                fingerprint = new Fingerprint();
                                fingerprint.AsBitmapSource = WpfIO.Load(file.FullName);
                                if (fingerprint.Image != null)
                                {
                                    Afis.ExtractFingerprint(fingerprint);
                                    fingerprintList.Add(fingerprint.GetTemplate());
                                    FingerPrintMinutias dbFingerprintMinutia = new FingerPrintMinutias(nextID.ToString(), fingerprint.GetTemplate(), PatientFHIRProfile.LateralitySnoMedCode.Left, PatientFHIRProfile.CaptureSiteSnoMedCode.IndexFinger);
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

        public float SearchByFingerprint(Fingerprint fingerprintProbe = null)
        {
            if (fingerprintProbe == null)
                fingerprintProbe = _probe;

            Afis.ExtractFingerprint(fingerprintProbe);
            Afis.Threshold = MATCH_THRESHOLD;
            return Afis.IdentifyFinger(fingerprintProbe.GetTemplate(), fingerprintList);
        }

        public float SearchByFingerPrintMinutias(FingerPrintMinutias fingerPrintMinutias = null)
        {
            Template templateProbe = FingerprintMinutiaConvertor.ConvertFingerPrintMinutias(fingerPrintMinutias);
            Afis.Threshold = MATCH_THRESHOLD;
            return Afis.IdentifyFinger(templateProbe, fingerprintList);
        }
    }
}
