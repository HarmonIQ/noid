// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.IO;
using System.Collections.Generic;
using DPUruNet;
using SourceAFIS.Simple;
using SourceAFIS.General;
using SourceAFIS.Templates;
using NoID.FHIR.Profile;
using NoID.Utilities;
using NoID.Biometrics.Managers;
using NoID.Match.Database.FingerPrint;
using System.Configuration;

namespace NoID.Match.Database.Tests
{
    public class MatchProbesTest
    {
        private DigitalPersona biometricDevice;
        private Exception _exception;
        private readonly static int MATCH_THRESHOLD = 50;
        private static AfisEngine Afis = new AfisEngine();
        List<Fingerprint> _probeList = new List<Fingerprint>();
        public int _ordinal = -1;
        private Fingerprint _currentProbe = null;
        private Fingerprint _probe = null;
        public ulong nextID = 1;
        private string _dabaseFilePath;
        private List<FingerPrintMinutias> dbFingerprintMinutiaList = new List<FingerPrintMinutias>();
        private FingerPrintMatchDatabase dbMinutia;
        private string _databasePath = ConfigurationManager.AppSettings["DatabaseLocation"].ToString();
        private string _lateralityCode = ConfigurationManager.AppSettings["Laterality"].ToString();
        private string  _captureSiteCode = ConfigurationManager.AppSettings["CaptureSite"].ToString();

        public MatchProbesTest()
        {
            dbMinutia = new FingerPrintMatchDatabase(_databasePath, _lateralityCode, _captureSiteCode);
            if (!(SetupScanner()))
            {
                if ((_exception == null))
                {
                    throw new Exception("Unknown scanner setup error.");
                }
                else
                {
                    throw _exception;
                }
            }
        }

        ~MatchProbesTest()
        {
        }

        private bool SetupScanner()
        {
            bool status = false;
            try
            {
                biometricDevice = new DigitalPersona();
                if (biometricDevice.StartCaptureAsync(this.OnCaptured))
                {
                    status = true;
                }
                else
                {
                    _exception = biometricDevice.Exception;
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
            return status;
        }

        private void OnCaptured(CaptureResult captureResult)
        {
            Person currentCapture = new Person();
            Fingerprint newFingerPrint = new Fingerprint();
            foreach (Fid.Fiv fiv in captureResult.Data.Views)
            {
                newFingerPrint.AsBitmap = ImageUtilities.CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height);
            }
            Afis.ExtractFingerprint(newFingerPrint);
            Template tmpNew = newFingerPrint.GetTemplate();
            string patientNoID = IdentifyFinger(tmpNew);
            if (patientNoID.Length == 0)
            {
                tmpNew.NoID = "NoID" + nextID;
                dbMinutia.AddTemplate(tmpNew);
                nextID++;
            }
        }

        public Fingerprint GetNextProbe()
        {
            if (_ordinal < 0)
                return null;

            if (_ordinal >= _probeList.Count)
                _ordinal = 0; //start over

            _currentProbe = _probeList[_ordinal];
            ++_ordinal;

            return _currentProbe;
        }

        public void LoadProbeImages(string imageDirectory)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(imageDirectory);
            Fingerprint fingerprint = null;
            if (dirInfo.Exists)
            {
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    fingerprint = new Fingerprint();
                    fingerprint.AsBitmapSource = WpfIO.Load(file.FullName);
                    if (fingerprint.Image != null)
                    {
                        _probeList.Add(fingerprint);
                    }
                }
                if (_probeList.Count > 0)
                {
                    _currentProbe = _probeList[0];
                    _ordinal = 1;
                }
            }
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
                                    Template template = fingerprint.GetTemplate();
                                    template.NoID = "NoID" + nextID.ToString();
                                    dbMinutia.AddTemplate(template);
                                    nextID++;
                                    if (nextID > 100)
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            //SerializeDatabaseProto serialize = new SerializeDatabaseProto();
            //serialize.WriteToDisk(DabaseFilePath + @"finger.hive.0001.biodb", dbFingerprintMinutiaList);
        }
        public string DabaseFilePath
        {
            get { return _dabaseFilePath; }
            private set { _dabaseFilePath = value; }
        }

        public string IdentifyFinger(Template probe)
        {
            return dbMinutia.SearchPatients(probe);
        }

        private float SearchByFingerprint(Fingerprint fingerprintProbe = null)
        {
            if (fingerprintProbe == null)
                fingerprintProbe = _probe;

            Afis.ExtractFingerprint(fingerprintProbe);
            Afis.Threshold = MATCH_THRESHOLD;
            return Afis.IdentifyFinger(fingerprintProbe, _probeList);
        }
    }
}
