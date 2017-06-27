// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.
// Copyright © 2010-2017 The CefSharp Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using CefSharp;
using CefSharp.WinForms;
using DPUruNet;
using SourceAFIS.Simple;
using SourceAFIS.Templates;
using Hl7.Fhir.Model;
using NoID.Utilities;
using NoID.Security;
using NoID.FHIR.Profile;
using NoID.Biometrics.Managers;
using NoID.Network.Transport;
using NoID.Match.Database.Client;
using System.Collections.Generic;

namespace NoID.Browser
{
    /// <summary>
    /// Controls the browser client when in patient mode.
    /// </summary>

    class PatientController
    {
        private readonly ChromiumWebBrowser _browser;
        private static AfisEngine Afis = new AfisEngine();
        private static MinutiaCaptureController _minutiaCaptureController = new MinutiaCaptureController();
        private List<FingerPrintMinutias> _fingerprintMinutias = new List<FingerPrintMinutias>();
        private PatientFHIRProfile noidFHIRProfile;

        private PatientBridge _patientBridge;

        private const float PROBE_MATCH_THRESHOLD = 70;

        //TODO: Abstract biometricDevice so it will work with any fingerprint scanner.
        private DigitalPersona biometricDevice;

        private string organizationName = System.Configuration.ConfigurationManager.AppSettings["OrganizationName"].ToString();
        private readonly string NoIDServiceName = System.Configuration.ConfigurationManager.AppSettings["NoIDServiceName"].ToString();
        private Uri _endPoint;
        private bool fLoadWebService = false;
        private bool fBiometricsComplete = false;

        public PatientController(ChromiumWebBrowser browser)
        {
            _browser = browser;
            Afis.Threshold = PROBE_MATCH_THRESHOLD;
            _endPoint = new Uri(StringUtilities.RemoveTrailingBackSlash(System.Configuration.ConfigurationManager.AppSettings["HealthcareNodeFHIRAddress"].ToString()));
            noidFHIRProfile = new PatientFHIRProfile(organizationName, _endPoint);
            _patientBridge = new PatientBridge(organizationName, _endPoint, NoIDServiceName);
            try
            {
                biometricDevice = new DigitalPersona();
                if (!biometricDevice.StartCaptureAsync(this.OnCaptured))
                {
                    throw new Exception("PatientController failed to ");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }

        //TODO: Abstract CaptureResult so it will work with any fingerprint scanner.
        private void OnCaptured(CaptureResult captureResult)
        {
            if (fBiometricsComplete == true)
            {
#if DEBUG_OBJECTS
                //DisplayOutput("Biometrics already captured and sent to web server. Ignoring this scan.");
#endif
                return;
            }
            if (fLoadWebService == true)
            {
#if DEBUG_OBJECTS
                //DisplayOutput("Capture occured while accessing the FHIR web services. Ignoring this scan.");
#endif
                return;
            }
            if (_patientBridge.captureSite != FHIRUtilities.CaptureSiteSnoMedCode.Unknown && _patientBridge.laterality != FHIRUtilities.LateralitySnoMedCode.Unknown)
            {
#if DEBUG_OBJECTS
                //DisplayOutput("Captured finger image....");
#endif
                // Check capture quality and throw an error if poor or incomplete capture.
                if (!biometricDevice.CheckCaptureResult(captureResult)) return;

                Constants.CaptureQuality quality = captureResult.Quality;
                if ((int)quality != 0)
                {
                    //call javascript to inform UI that the capture quality was too low to accept.
#if DEBUG_OBJECTS
                    //DisplayOutput("Fingerprint quality was too low to accept. Quality = " + quality.ToString());
#endif
                    return;
                }
                Type captureResultType = captureResult.GetType();
                string deviceClassName = captureResultType.ToString();
                string deviceName = "";
                if (deviceClassName == "DPUruNet.CaptureResult")
                {
                    deviceName = "DigitalPersona U.Are.U 4500";
                }

                SourceAFIS.Simple.Person currentCapture = new SourceAFIS.Simple.Person();

                Fingerprint newFingerPrint = new Fingerprint();
                foreach (Fid.Fiv fiv in captureResult.Data.Views)
                {
                    newFingerPrint.AsBitmap = ImageUtilities.CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height);
                }
                currentCapture.Fingerprints.Add(newFingerPrint);
                Afis.Extract(currentCapture);
                Template tmpCurrent = newFingerPrint.GetTemplate();

                if (_minutiaCaptureController.MatchFound == false)
                {
                    if (_minutiaCaptureController.AddMinutiaTemplateProbe(tmpCurrent) == true)
                    {
                        // Good pair found.
                        // Query web service for a match.
                        FingerPrintMinutias fingerprintMinutia =
                            new FingerPrintMinutias(SessionID, tmpCurrent, Laterality, CaptureSite); //need to pass session id instead of patient cert id.

                        Media media = noidFHIRProfile.FingerPrintFHIRMedia(fingerprintMinutia, deviceName, tmpCurrent.OriginalDpi, tmpCurrent.OriginalHeight, tmpCurrent.OriginalWidth);
                        HttpsClient dataTransport = new HttpsClient();

                        Authentication auth;
                        if (Utilities.Auth == null)
                        {
                            auth = SecurityUtilities.GetAuthentication(NoIDServiceName);
                        }
                        else
                        {
                            auth = Utilities.Auth;
                        }
                        fLoadWebService = true;
                        dataTransport.SendFHIRMediaProfile(_endPoint, auth, media);
                        fLoadWebService = false;
#if DEBUG_OBJECTS
                        string lateralityString = FHIRUtilities.LateralityToString(Laterality);
                        string captureSiteString = FHIRUtilities.CaptureSiteToString(CaptureSite);
                        string output = lateralityString + " " + captureSiteString + " fingerprint accepted. Score = " + _minutiaCaptureController.BestScore + ", Fingerprint sent to server: Response = " + dataTransport.ResponseText;
                        //DisplayOutput(output);
#endif
                        // If match found, inform JavaScript that this is an returning patient for identity.
                        _browser.GetMainFrame().ExecuteJavaScriptAsync("showComplete('" + Laterality.ToString() + "');");
                        if (Laterality == FHIRUtilities.LateralitySnoMedCode.Left || Laterality == FHIRUtilities.LateralitySnoMedCode.Right)
                        {
                            FingerPrintMinutias newFingerPrintMinutias = new FingerPrintMinutias
                                (
                                    SessionID, _minutiaCaptureController.BestTemplate1, Laterality, CaptureSite
                                );
                            _fingerprintMinutias.Add(newFingerPrintMinutias);

                            newFingerPrintMinutias = new FingerPrintMinutias
                                (
                                    SessionID, _minutiaCaptureController.BestTemplate2, Laterality, CaptureSite
                                );
                            _fingerprintMinutias.Add(newFingerPrintMinutias);
                            if (Laterality == FHIRUtilities.LateralitySnoMedCode.Right)
                            {
                                fBiometricsComplete = true;
                            }
                            // Reset
                            CaptureSite = FHIRUtilities.CaptureSiteSnoMedCode.Unknown;
                            Laterality = FHIRUtilities.LateralitySnoMedCode.Unknown;
                            _minutiaCaptureController = new MinutiaCaptureController();
                        }
                        // If not match found, inform JavaScript that this is an new patient enrollment.  reset _minutiaCaptureController for right side.
                    }
                    else
                    {
                        // Good fingerprint pairs not found yet.  inform JavaScript to promt the patient to try again.
                        _browser.GetMainFrame().ExecuteJavaScriptAsync("showFail('" + Laterality.ToString() + "');");
#if DEBUG_OBJECTS
                        //DisplayOutput("Fingerprint NOT accepted. Score = " + _minutiaCaptureController.BestScore);
#endif
                        return;
                    }
                }
            }
            else
            {
#if DEBUG_OBJECTS
                //DisplayOutput("Must be on the correct page to accept a fingerprint scan.");
#endif
            }
        }

        public PatientBridge PatientBridge
        {
            get
            {
                return _patientBridge;
            }
        }

        FHIRUtilities.LateralitySnoMedCode Laterality
        {
            get
            {
                if (_patientBridge != null)
                {
                    return _patientBridge.laterality;
                }
                else
                {
                    throw new Exception("Tried to access Laterality when _patientBridge is null");
                }
            }
            set
            {
                if (_patientBridge != null)
                {

                    _patientBridge.laterality = value;
                }
                else
                {
                    throw new Exception("Tried to access Laterality when _patientBridge is null");
                }
            }
        }

        FHIRUtilities.CaptureSiteSnoMedCode CaptureSite
        {
            get
            {
                if (_patientBridge != null)
                {
                    return _patientBridge.captureSite;
                }
                else
                {
                    throw new Exception("Tried to access CaptureSite when _patientBridge is null");
                }
            }
            set
            {
                if (_patientBridge != null)
                {

                    _patientBridge.captureSite = value;
                }
                else
                {
                    throw new Exception("Tried to access CaptureSite when _patientBridge is null");
                }
            }
        }

        string SessionID
        {
            get
            {
                if (_patientBridge != null)
                {
                    return _patientBridge.sessionID;
                }
                else
                {
                    throw new Exception("Tried to access SessionID when _patientBridge is null");
                }
            }
        }

        string LocalNoID
        {
            get
            {
                if (_patientBridge != null)
                {
                    return _patientBridge.localNoID;
                }
                else
                {
                    throw new Exception("Tried to access LocalNoID when _patientBridge is null");
                }
            }
            set
            {
                if (_patientBridge != null)
                {

                    _patientBridge.localNoID = value;
                }
                else
                {
                    throw new Exception("Tried to access LocalNoID when _patientBridge is null");
                }
            }
        }

        string CurrentPage
        {
            get
            {
                if (_patientBridge != null)
                {
                    return _patientBridge.currentPage;
                }
                else
                {
                    throw new Exception("Tried to access CurrentPage when _patientBridge is null");
                }
            }
        }

        string RemoteNoID
        {
            get
            {
                if (_patientBridge != null)
                {
                    return _patientBridge.remoteNoID;
                }
                else
                {
                    throw new Exception("Tried to access RemoteNoID when _patientBridge is null");
                }
            }
            set
            {
                if (_patientBridge != null)
                {

                    _patientBridge.remoteNoID = value;
                }
                else
                {
                    throw new Exception("Tried to access RemoteNoID when _patientBridge is null");
                }
            }
        }
    }
}
