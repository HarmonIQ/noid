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

namespace NoID.Match.Database.Tests
{
    public class MatchProbesTest
    {
        public event EventHandler FingerCaptured = delegate { };
        public event EventHandler GoodPairFound = delegate { };
        public event EventHandler NewBestMatchFound = delegate { };
        public event EventHandler DatabaseMatchFound = delegate { };
        public event EventHandler DatabaseMatchError = delegate { };
        public event EventHandler DoesNotMatch = delegate { };
        public event EventHandler PoorCaputure = delegate { };

        public float Score = 0;

        private static AfisEngine Afis = new AfisEngine();
        private DigitalPersona biometricDevice;
        private FingerPrintMatchDatabase dbMinutia;
        private Exception _exception;
        public ulong nextID = 1;
        
        private Person currentCapture;
        private Person previousCapture;
        public Fingerprint currenFingerPrint;
        public Fingerprint previousFingerPrint;
        public int Quality;

        private Person bestCapture;
        private string patientNoID = "";
        private bool fGoodPairFound = false;
        public float HighScore = 0;


        public MatchProbesTest(string databaseDirectory, string backupDatabaseDirectory, string lateralityCode, string captureSiteCode)
        {
            dbMinutia = new FingerPrintMatchDatabase(databaseDirectory, backupDatabaseDirectory, lateralityCode, captureSiteCode);
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

            bool newBest = false;
            bool match = false;
            Afis.Threshold = 70;
            Score = 0;
            
            Constants.CaptureQuality quality = captureResult.Quality;
            if ((int)quality != 0)
            {
                Quality = (int)quality;
                PoorCaputure(quality, new EventArgs());
                return;
            }

            currentCapture = new SourceAFIS.Simple.Person();

            Fingerprint newFingerPrint = new Fingerprint();
            foreach (Fid.Fiv fiv in captureResult.Data.Views)
            {
                newFingerPrint.AsBitmap = ImageUtilities.CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height);
            }
            currentCapture.Fingerprints.Add(newFingerPrint);
            Afis.Extract(currentCapture);
            currenFingerPrint = newFingerPrint;
            Template tmpCurrent = newFingerPrint.GetTemplate();
            if (!(bestCapture == null))
            {
                Score = Afis.Verify(currentCapture, bestCapture);
                match = (Score > Afis.Threshold);
            }
            else if ((bestCapture == null) && !(previousCapture == null))
            {
                Score = Afis.Verify(currentCapture, previousCapture);
                match = (Score > Afis.Threshold);
            }

            if (FingerCaptured != null)
            {
                FingerCaptured(newFingerPrint, new EventArgs());
            }

            if (!(match))
            {
                DoesNotMatch(currenFingerPrint, new EventArgs());
            }
            else if (match)
            {
                if (GoodPairFound != null)
                {
                    GoodPairFound(currenFingerPrint, new EventArgs());
                }
                if (fGoodPairFound == false)
                    fGoodPairFound = true;
            }

            Template tmp;
            if (Score > HighScore)
            {
                HighScore = Score;
                bestCapture = currentCapture;
                if (NewBestMatchFound != null)
                {
                    newBest = true;
                    NewBestMatchFound(bestCapture.Fingerprints[0], new EventArgs());
                }
            }
            if (!(bestCapture == null))
            {
                tmp = bestCapture.Fingerprints[0].GetTemplate();
            }
            else
            {
                tmp = tmpCurrent;
            }

            string idFound = IdentifyFinger(tmp);
            if (match && (fGoodPairFound == true) && (patientNoID.Length == 0) && (idFound != null) && (idFound.Length == 0))
            {
                tmp.NoID = "NoID" + nextID;
                dbMinutia.AddTemplate(tmp);
                patientNoID = idFound;
                nextID++;
            }
            else if (match && (fGoodPairFound == true) && (patientNoID.Length > 0) && (idFound.Length == 0))
            {
                if (DatabaseMatchError != null)
                {
                    DatabaseMatchError(newFingerPrint, new EventArgs());
                }
                patientNoID = "Error, adjust threshold. It is too low.";
            }
            else
            {
                if (match && DatabaseMatchFound != null)
                {
                    DatabaseMatchFound(newFingerPrint, new EventArgs());
                    if (newBest)
                    {
                        dbMinutia.UpdateTemplate(tmp, idFound);
                    }
                }
            }

            previousCapture = currentCapture;
            if (!(previousCapture == null))
            {
                previousFingerPrint = previousCapture.Fingerprints[0];
            }
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
