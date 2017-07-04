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
using NoID.Biometrics.Managers;
using NoID.Match.Database.Client;
using NoID.Match.Database.FingerPrint;
using NoID.Utilities;

namespace NoID.Match.Database.Tests
{
    /// <summary>
    /// Match Probes Test
    /// </summary>

    public class MatchProbesTest
    {
        private static readonly uint MinimumAcceptedMatchScore = 70;

        public event EventHandler FingerCaptured = delegate { };
        public event EventHandler GoodPairFound = delegate { };
        public event EventHandler NewBestMatchFound = delegate { };
        public event EventHandler DatabaseMatchFound = delegate { };
        public event EventHandler DatabaseMatchError = delegate { };
        public event EventHandler DoesNotMatch = delegate { };
        public event EventHandler PoorCaputure = delegate { };

        public float Score = 0;
        public string ScannerStatus = "";

        private static AfisEngine Afis = new AfisEngine();
        private MinutiaCaptureController _minutiaCaptureController = new MinutiaCaptureController(MinimumAcceptedMatchScore);
        private DigitalPersona biometricDevice;
        private Exception _exception;
        private FingerPrintMatchDatabase dbMinutia;
        private List<Fingerprint> _capturedFingerprints = new List<Fingerprint>();
        public Fingerprint bestFingerprint1;
        public Fingerprint bestFingerprint2;
        public string NoID = "";// = Guid.NewGuid().ToString();
        public ulong nextID = 1;
        private Person currentCapture;
        public int Quality;
        public float HighScore = 0;
        private readonly uint _matchThreshold = 30;

        public MatchProbesTest(string databaseDirectory, string backupDatabaseDirectory, string lateralityCode, string captureSiteCode)
        {
            dbMinutia = new FingerPrintMatchDatabase(databaseDirectory, backupDatabaseDirectory, _matchThreshold);
            try
            {
                dbMinutia.LateralityCode = (FHIRUtilities.LateralitySnoMedCode)Int32.Parse(lateralityCode);
                dbMinutia.CaptureSite = (FHIRUtilities.CaptureSiteSnoMedCode)Int32.Parse(captureSiteCode);
            }
            catch { }

            if (!(SetupScanner()))
            {
                if ((_exception == null))
                {
                    ScannerStatus = "Failed: Unknown error.";
                    throw new Exception("Unknown scanner setup error.");
                }
                else
                {
                    ScannerStatus = "Failed: " + _exception.Message;
                    throw _exception;
                }
            }
            ScannerStatus = "OK";
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
            // Check capture quality and throw an error if poor or incomplete capture.
            if (!biometricDevice.CheckCaptureResult(captureResult)) return;


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
            Template tmpCurrent = newFingerPrint.GetTemplate();

            if (FingerCaptured != null)
            {
                FingerCaptured(newFingerPrint, new EventArgs());
            }
            _capturedFingerprints.Add(newFingerPrint);

            if (_minutiaCaptureController.MatchFound == false)
            { 
                if (_minutiaCaptureController.AddMinutiaTemplateProbe(tmpCurrent) == true)
                {
                    // Good pair found.
                    bestFingerprint1 = newFingerPrint;
                    bestFingerprint2 = _capturedFingerprints[_minutiaCaptureController.OtherBestFingerprintItem];
                    if (NewBestMatchFound != null)
                    {
                        NewBestMatchFound(bestFingerprint2, new EventArgs());
                    }
                }
                else
                {
                    // Good fingerprint pairs not found yet.  Try again.
                    return;
                }
            }
            // Lookup minitia pair in database.
            // If found, show NoID
            // If not found, save minutia pair in database with new NoID.
            // probe is set, search database for match.
            Template Template1 = bestFingerprint1.GetTemplate();
            Template Template2 = bestFingerprint2.GetTemplate();

            MinutiaResult idFound = IdentifyFinger(tmpCurrent);
            if ((idFound != null && idFound.NoID.Length > 0))
            {
                // Fingerprint found in database
                Score = idFound.Score;
                if (DatabaseMatchFound != null)
                {
                    DatabaseMatchFound(newFingerPrint, new EventArgs());
                }
                if (NoID.Length == 0)
                {
                    NoID = idFound.NoID;
                }
                else if (NoID.Length > 0 && NoID != idFound.NoID)
                {
                    //critical error! create false assert here.
                    NoID = idFound.NoID;
                    if (DatabaseMatchError != null)
                    {
                        DatabaseMatchError(newFingerPrint, new EventArgs());
                    }
                }
            }
            else
            {
                // Not found in database
                Score = 0;
                if (DoesNotMatch != null)
                    DoesNotMatch(newFingerPrint, new EventArgs());

                if (NoID.Length == 0)
                {
                    Template1.NoID.LocalNoID = Guid.NewGuid().ToString();
                    Template2.NoID = Template1.NoID;
                    dbMinutia.AddTemplate(Template1);
                    dbMinutia.AddTemplate(Template2);
                    // trigger event NewPatientAdded
                }
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
                                    template.NoID.LocalNoID = "Test" + nextID.ToString();
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

        public MinutiaResult IdentifyFinger(Template probe)
        {
            return dbMinutia.SearchPatients(probe);
        }

        public int Count
        {
            get { return dbMinutia.CandidateCount; }
        }
    }
}
