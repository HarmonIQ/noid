// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.IO;
using DPUruNet;
using SourceAFIS.Simple;
using SourceAFIS.General;
using SourceAFIS.Templates;
using NoID.Biometrics.Managers;
using NoID.Match.Database.FingerPrint;
using System.Configuration;

namespace NoID.Match.Database.Tests
{
    public class MatchProbesTest
    {
        public event EventHandler FingerCaptured = delegate { };
        public event EventHandler GoodPairFound = delegate { };
        public event EventHandler DatabaseMatchFound = delegate { };
        public event EventHandler DatabaseMatchError = delegate { };

        private static AfisEngine Afis = new AfisEngine();
        private DigitalPersona biometricDevice;
        private FingerPrintMatchDatabase dbMinutia;
        private Exception _exception;
        public ulong nextID = 1;
        private string _dabaseFilePath = ConfigurationManager.AppSettings["DatabaseLocation"].ToString();
        private string _lateralityCode = ConfigurationManager.AppSettings["Laterality"].ToString();
        private string  _captureSiteCode = ConfigurationManager.AppSettings["CaptureSite"].ToString();
        private Person currentCapture;
        private Person previousCapture;
        private Person bestCapture;
        private string patientNoID = "";
        private bool fGoodPairFound = false;

        public MatchProbesTest()
        {
            dbMinutia = new FingerPrintMatchDatabase(_dabaseFilePath, _lateralityCode, _captureSiteCode);
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
            // Check capture quality and throw an error if bad.
            if (!biometricDevice.CheckCaptureResult(captureResult)) return;
            
            bool match = false;
            float score = 0;
            Afis.Threshold = 70;

            currentCapture = new SourceAFIS.Simple.Person();
            
            Fingerprint newFingerPrint = new Fingerprint();
            foreach (Fid.Fiv fiv in captureResult.Data.Views)
            {
                newFingerPrint.AsBitmap = ImageUtilities.CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height);
            }
            currentCapture.Fingerprints.Add(newFingerPrint);
            Afis.Extract(currentCapture);
            Template tmpCurrent = newFingerPrint.GetTemplate();
            if (!(bestCapture == null))
            {
                score = Afis.Verify(currentCapture, bestCapture);
                match = (score > Afis.Threshold);
            }
            else if ((bestCapture == null) && !(previousCapture == null))
            {
                score = Afis.Verify(currentCapture, previousCapture);
                match = (score > Afis.Threshold);
            }

            if (FingerCaptured != null)
            {
                FingerCaptured(tmpCurrent, new EventArgs());
            }
                
            if (match)
            {
                Template tmp;
                if (!(bestCapture == null))
                {
                    tmp = bestCapture.Fingerprints[0].GetTemplate();
                    if (tmpCurrent.Minutiae.Length >= tmp.Minutiae.Length)
                    {
                        bestCapture = currentCapture;
                    }
                }
                else if (!(previousCapture == null))
                {
                    tmp = previousCapture.Fingerprints[0].GetTemplate();
                    if (tmp.Minutiae.Length > tmpCurrent.Minutiae.Length)
                    {
                        bestCapture = previousCapture;
                    }
                    else
                    {
                        bestCapture = currentCapture;
                    }
                }

                tmp = bestCapture.Fingerprints[0].GetTemplate();
                

                string idFound = IdentifyFinger(tmp);
                if ((fGoodPairFound == false) && (patientNoID.Length == 0) && (idFound.Length == 0))
                {
                    tmp.NoID = "NoID" + nextID;
                    dbMinutia.AddTemplate(tmp);
                    patientNoID = idFound;

                    nextID++;
                }
                else if ((fGoodPairFound == true) && (patientNoID.Length > 0) && (idFound.Length == 0))
                {
                    if (DatabaseMatchError != null)
                    {
                        DatabaseMatchError(tmpCurrent, new EventArgs());
                    }
                    patientNoID = "Error, adjust threshold. It is too low.";
                }
                else
                {
                    if (DatabaseMatchFound != null)
                    {
                        DatabaseMatchFound(tmpCurrent, new EventArgs());
                    }
                }

                if (fGoodPairFound == false)
                {
                    fGoodPairFound = true;
                    if (GoodPairFound != null)
                    {
                        GoodPairFound(tmp, new EventArgs());
                    }
                }
            }
            previousCapture = currentCapture;
        }

        public void LoadTestFingerPrintImages(string imageDirectory, bool breakAtOneHundred = false)
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
                                    if (breakAtOneHundred && nextID > 10)
                                        break;
                                }
                            }
                        }
                    }
                }
            }
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

        public int Count
        {
            get { return dbMinutia.CandidateCount; }
        }
    }
}
