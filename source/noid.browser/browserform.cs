// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.
// Copyright © 2010-2017 The CefSharp Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;
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
    /// Main client browser wrapper class
    /// </summary>

    public partial class BrowserForm : Form
    {
        private static AfisEngine Afis = new AfisEngine();
        private static MinutiaCaptureController _minutiaCaptureController = new MinutiaCaptureController();
        private List<FingerPrintMinutias> _fingerprintMinutias = new List<FingerPrintMinutias>();
        private PatientFHIRProfile noidFHIRProfile;

        //TODO: Use one object for both bridges.
        private PatientBridge _patientBridge;
        private ProviderBridge _providerBridge;

        private const float PROBE_MATCH_THRESHOLD = 70;
        private readonly ChromiumWebBrowser browser;

        //TODO: Abstract biometricDevice so it will work with any fingerprint scanner.
        private DigitalPersona biometricDevice;
        
        private string organizationName = System.Configuration.ConfigurationManager.AppSettings["OrganizationName"].ToString();
        private readonly string NoIDServiceName = System.Configuration.ConfigurationManager.AppSettings["NoIDServiceName"].ToString();
        private Uri healthcareNodeFHIRAddress;
        private string healthcareNodeWebAddress;
        private string healthcareNodeChainVerifyAddress;
        private bool fLoadWebService = false;
        private bool fBiometricsComplete = false;

        public BrowserForm()
        {
            InitializeComponent();

            healthcareNodeFHIRAddress = new Uri(StringUtilities.RemoveTrailingBackSlash(System.Configuration.ConfigurationManager.AppSettings["HealthcareNodeFHIRAddress"].ToString()));
            healthcareNodeWebAddress = StringUtilities.RemoveTrailingBackSlash(System.Configuration.ConfigurationManager.AppSettings["HealthcareNodeWeb"].ToString());
            healthcareNodeChainVerifyAddress = StringUtilities.RemoveTrailingBackSlash(System.Configuration.ConfigurationManager.AppSettings["HealthcareNodeChainVerifyAddress"].ToString());

            noidFHIRProfile = new PatientFHIRProfile(organizationName, healthcareNodeFHIRAddress);
            Afis.Threshold = PROBE_MATCH_THRESHOLD;

            Text = "NoID Browser";
            WindowState = FormWindowState.Maximized;
            
            string endPath = "";
            string approle = System.Configuration.ConfigurationManager.AppSettings["approle"].ToString();

            switch (approle)
            {
                case "patient":
                case "enrollment-pc":
                case "enrollment-kiosk":
                case "patient-pc":
                case "patient-kiosk":
                    endPath = endPath = healthcareNodeWebAddress + "/enrollment.html"; //TODO: rename to patient.html
                    browser = new ChromiumWebBrowser(endPath) { Dock = DockStyle.Fill };
                    _patientBridge = new PatientBridge(organizationName, healthcareNodeFHIRAddress, NoIDServiceName);
                    browser.RegisterJsObject("NoIDBridge", _patientBridge);
                    break;
                case "provider":
                case "provider-pc":
                case "provider-kiosk":
                    endPath = healthcareNodeWebAddress + "/provider.html";
                    browser = new ChromiumWebBrowser(endPath) { Dock = DockStyle.Fill };
                    _providerBridge = new ProviderBridge(organizationName, healthcareNodeFHIRAddress, NoIDServiceName);
                    browser.RegisterJsObject("NoIDBridge", _providerBridge);
                    break;
                case "healthcare-node-admin-kiosk":
                case "healthcare-node-admin-pc":
                    endPath = healthcareNodeWebAddress + "/healthcare.admin.html";
                    browser = new ChromiumWebBrowser(endPath) { Dock = DockStyle.Fill };
                    break;
                case "match-hub-admin-kiosk":
                case "match-hub-admin-pc":
                    endPath = healthcareNodeWebAddress + "/hub.admin.html";
                    browser = new ChromiumWebBrowser(endPath) { Dock = DockStyle.Fill };
                    break;
                default:
                    endPath = healthcareNodeWebAddress + "/error.html"; //TODO: We need an HTML error page.
                    browser = new ChromiumWebBrowser(endPath) { Dock = DockStyle.Fill };
                    break;
            }

            biometricDevice = new DigitalPersona();
            if (!biometricDevice.StartCaptureAsync(this.OnCaptured))
            {
                this.Close();
            }
#if NAVIGATE
            toolStripContainer.ContentPanel.Controls.Add(browser);
#else
            this.Controls.Add(browser);
#endif
#if NAVIGATE
            browser.StatusMessage += OnBrowserStatusMessage;
            browser.TitleChanged += OnBrowserTitleChanged;

            browser.ConsoleMessage += OnBrowserConsoleMessage;
            browser.LoadingStateChanged += OnLoadingStateChanged;
            browser.AddressChanged += OnBrowserAddressChanged;

            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            //var version = String.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}, Environment: {3}", Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion, bitness);
            string initialDisplayText = String.Format(approle.ToString());
            DisplayOutput(initialDisplayText);
#endif
        }
        
        //TODO: Abstract CaptureResult so it will work with any fingerprint scanner.
        private void OnCaptured(CaptureResult captureResult)
        {
            if (fBiometricsComplete == true)
            {
#if NAVIGATE
                DisplayOutput("Biometrics already captured and sent to web server. Ignoring this scan.");
#endif
                return;
            }
            if (fLoadWebService == true)
            {
#if NAVIGATE
                DisplayOutput("Capture occured while accessing the FHIR web services. Ignoring this scan.");
#endif
                return;
            }
            if (_patientBridge.captureSite != FHIRUtilities.CaptureSiteSnoMedCode.Unknown && _patientBridge.laterality != FHIRUtilities.LateralitySnoMedCode.Unknown)
            {
#if NAVIGATE
                DisplayOutput("Captured finger image....");
#endif
            
                // Check capture quality and throw an error if poor or incomplete capture.
                if (!biometricDevice.CheckCaptureResult(captureResult)) return;

                Constants.CaptureQuality quality = captureResult.Quality;
                if ((int)quality != 0)
                {
                    //call javascript to inform UI that the capture quality was too low to accept.
#if NAVIGATE
                    DisplayOutput("Fingerprint quality was too low to accept. Quality = " + quality.ToString());

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
                        string NoIDServicePassword = PasswordManager.GetPassword(NoIDServiceName);
                    
                        FingerPrintMinutias fingerprintMinutia = 
                            new FingerPrintMinutias(SessionID, tmpCurrent, Laterality, CaptureSite); //need to pass session id instead of patient cert id.

                        Media media = noidFHIRProfile.FingerPrintFHIRMedia(fingerprintMinutia, deviceName, tmpCurrent.OriginalDpi, tmpCurrent.OriginalHeight, tmpCurrent.OriginalWidth);
                        HttpsClient dataTransport = new HttpsClient();
                        Authentication auth = SecurityUtilities.GetAuthentication(NoIDServiceName);
                        fLoadWebService = true;
                        dataTransport.SendFHIRMediaProfile(healthcareNodeFHIRAddress, auth, media);
                        fLoadWebService = false;
#if NAVIGATE
                        string lateralityString = FHIRUtilities.LateralityToString(Laterality);
                        string captureSiteString = FHIRUtilities.CaptureSiteToString(CaptureSite);
                        string output = lateralityString + " " + captureSiteString + " fingerprint accepted. Score = " + _minutiaCaptureController.BestScore + ", Fingerprint sent to server: Response = " + dataTransport.ResponseText;
                        DisplayOutput(output);
#endif
                        // If match found, inform JavaScript that this is an returning patient for identity.
                        browser.GetMainFrame().ExecuteJavaScriptAsync("showComplete('" + Laterality.ToString() + "');");
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
                        browser.GetMainFrame().ExecuteJavaScriptAsync("showFail('" + Laterality.ToString() + "');");
#if NAVIGATE
                        DisplayOutput("Fingerprint NOT accepted. Score = " + _minutiaCaptureController.BestScore);
#endif
                        return;
                    }
                }
            }
            else
            {
#if NAVIGATE
                DisplayOutput("Must be on the correct page to accept a fingerprint scan.");
#endif
            }
        }
        
#if NAVIGATE
        private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => statusLabel.Text = args.Value);
        }

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => Text = args.Title);
        }

        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            DisplayOutput(string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message));
        }

        private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            SetCanGoBack(args.CanGoBack);
            SetCanGoForward(args.CanGoForward);

            this.InvokeOnUiThreadIfRequired(() => SetIsLoading(!args.CanReload));
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => urlTextBox.Text = args.Address);
        }
        
        private void SetCanGoBack(bool canGoBack)
        {
            this.InvokeOnUiThreadIfRequired(() => backButton.Enabled = canGoBack);
        }

        private void SetCanGoForward(bool canGoForward)
        {
            this.InvokeOnUiThreadIfRequired(() => forwardButton.Enabled = canGoForward);
        }
        
        private void SetIsLoading(bool isLoading)
        {
            goButton.Text = isLoading ?
                "Stop" :
                "Go";
            goButton.Image = isLoading ?
                properties.resources.nav_plain_red :
                properties.resources.nav_plain_green;

            HandleToolStripLayout();
        }
        
        public void DisplayOutput(string output)
        {
            this.InvokeOnUiThreadIfRequired(() => outputLabel.Text = output);
        }

        private void HandleToolStripLayout(object sender, LayoutEventArgs e)
        {
            HandleToolStripLayout();
        }
        
        private void HandleToolStripLayout()
        {
            var width = toolStrip1.Width;
            foreach (ToolStripItem item in toolStrip1.Items)
            {
                if (item != urlTextBox)
                {
                    width -= item.Width - item.Margin.Horizontal;
                }
            }
            urlTextBox.Width = Math.Max(0, width - urlTextBox.Margin.Horizontal - 18);
        }
        
        private void ExitMenuItemClick(object sender, EventArgs e)
        {
            browser.Dispose();
            Cef.Shutdown();
            Close();
        }

        private void GoButtonClick(object sender, EventArgs e)
        {
            LoadUrl(urlTextBox.Text);
        }

        private void BackButtonClick(object sender, EventArgs e)
        {
            browser.Back();
        }

        private void ForwardButtonClick(object sender, EventArgs e)
        {
            browser.Forward();
        }
        
        private void UrlTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            LoadUrl(urlTextBox.Text);
        }

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                browser.Load(url);
            }
        }
#endif

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
