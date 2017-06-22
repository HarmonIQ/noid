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

namespace NoID.Browser
{
    public partial class BrowserForm : Form
    {
        private static AfisEngine Afis = new AfisEngine();
        private const float PROBE_MATCH_THRESHOLD = 70;
        private readonly ChromiumWebBrowser browser;
        //TODO: Abstract biometricDevice so it will work with any fingerprint scanner.
        private DigitalPersona biometricDevice;
        private PatientFHIRProfile noidFHIRProfile;
        private string organizationName = System.Configuration.ConfigurationManager.AppSettings["OrganizationName"].ToString();
        private SourceAFIS.Simple.Person currentCapture;
        private SourceAFIS.Simple.Person previousCapture;
        private Fingerprint newFingerPrint;
        private Fingerprint previousFingerPrint;
        private bool match = false;
        private float score = 0;
        private Uri healthcareNodeFHIRAddress;
        private string healthcareNodeWebAddress;
        private string healthcareNodeChainVerifyAddress;
        private readonly string NoIDServiceName = System.Configuration.ConfigurationManager.AppSettings["NoIDServiceName"].ToString();
        private string NoIDServicePassword;

        //TODO: Abstract CaptureResult so it will work with any fingerprint scanner.
        private void OnCaptured(CaptureResult captureResult)
        {
#if NAVIGATE
            DisplayOutput("Captured finger image....");
#endif
            NoIDServicePassword = NoID.Security.PasswordManager.GetPassword(NoIDServiceName);
            match = false;
            currentCapture = new SourceAFIS.Simple.Person();
            // Check capture quality and throw an error if bad.
            if (!biometricDevice.CheckCaptureResult(captureResult)) return;

            newFingerPrint = new Fingerprint();
            foreach (Fid.Fiv fiv in captureResult.Data.Views)
            {
                newFingerPrint.AsBitmap = ImageUtilities.CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height);
            }
            currentCapture.Fingerprints.Add(newFingerPrint);
            Afis.Extract(currentCapture);
            if (!(previousCapture is null))
            {
                score = Afis.Verify(currentCapture, previousCapture);
                match = (score > Afis.Threshold);
            }
            var matchResults = String.Format("Match: {0}, Score: {1}", match, score);
#if NAVIGATE
            DisplayOutput(matchResults);
#endif
            if (score >= PROBE_MATCH_THRESHOLD)
            {
                FHIRUtilities.LateralitySnoMedCode laterality = FHIRUtilities.LateralitySnoMedCode.Left;
                FHIRUtilities.CaptureSiteSnoMedCode captureSiteSnoMedCode = FHIRUtilities.CaptureSiteSnoMedCode.IndexFinger;

                noidFHIRProfile.PatientCertificateID = Guid.NewGuid().ToString();

                Afis.ExtractFingerprint(newFingerPrint);
                Template tmpNew = newFingerPrint.GetTemplate();
                Afis.ExtractFingerprint(previousFingerPrint);
                Template tmpPrevious = newFingerPrint.GetTemplate();
                FingerPrintMinutias fingerprintMinutia;
                if (tmpNew.Minutiae.Length >= tmpPrevious.Minutiae.Length)
                {
                     fingerprintMinutia = new FingerPrintMinutias(noidFHIRProfile.PatientCertificateID, tmpNew, laterality, captureSiteSnoMedCode);
                }
                else
                {
                    fingerprintMinutia = new FingerPrintMinutias(noidFHIRProfile.PatientCertificateID, tmpPrevious, laterality, captureSiteSnoMedCode);
                }
                
                Media media = noidFHIRProfile.FingerPrintFHIRMedia(fingerprintMinutia);
                HttpsClient dataTransport = new HttpsClient();
                Authentication auth = SecurityUtilities.GetAuthentication(NoIDServiceName);
                dataTransport.SendFHIRMediaProfile(healthcareNodeFHIRAddress, auth, media);
            }
            previousCapture = currentCapture;
            previousFingerPrint = newFingerPrint;
        }

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
                case "enrollment-kiosk":
                    endPath = healthcareNodeWebAddress + "/enrollment-kiosk.html";
                    break;
                case "enrollment-pc":
                    endPath = endPath = healthcareNodeWebAddress + "/enrollment.html";
                    break;
                case "identity-kiosk":
                    endPath = endPath = healthcareNodeWebAddress + "/identity-kiosk.html";
                    break;
                case "identity-pc":
                    endPath = healthcareNodeWebAddress + "/identity.html";
                    break;
                case "patient-portal-kiosk":
                    endPath = healthcareNodeWebAddress + "/patient-portal-kiosk.html";
                    break;
                case "patient-portal-pc":
                    endPath = healthcareNodeWebAddress + "patient-portal-pc.html";
                    break;
                case "healthcare-node-admin-kiosk":
                    endPath = healthcareNodeWebAddress + "/healthcare-node-admin-kiosk.html";
                    break;
                case "healthcare-node-admin-pc":
                    endPath = healthcareNodeWebAddress + "/healthcare-node-admin-pc.html";
                    break;
                case "match-hub-admin-kiosk":
                    endPath = healthcareNodeWebAddress + "/match-hub-admin-kiosk.html";
                    break;
                case "match-hub-admin-pc":
                    endPath = healthcareNodeWebAddress + "/match-hub-admin-pc.html";
                    break;
                default:
                    endPath = healthcareNodeWebAddress + "/enrollment.html";
                    break;
            }

            browser = new ChromiumWebBrowser(endPath) { Dock = DockStyle.Fill };
            // Handles JavaScripts Events
            NoIDBridge bridge = new NoIDBridge(organizationName, healthcareNodeFHIRAddress, NoIDServiceName);
            browser.RegisterJsObject("NoIDBridge", bridge);

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
    }
}
