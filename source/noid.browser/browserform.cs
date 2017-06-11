// Copyright © 2016 NoID Developers. All rights reserved.
// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.
using System;
using System.Windows.Forms;
using NoID.Browser.Controls;
using CefSharp.WinForms;
using CefSharp;
using NoID.Message;
using NoID.Biometrics.Managers;
using DPUruNet;
using SourceAFIS.Simple;

namespace NoID.Browser
{
    public partial class BrowserForm : Form
    {
        private static AfisEngine Afis = new AfisEngine();
        private const float MATCH_THRESHOLD = 23;
        private readonly ChromiumWebBrowser browser;
        private DigitalPersona biometricDevice;
        private PatientProfile patientProfile_FHIR = new PatientProfile();
        private Person currentCapture;
        private Person previousCapture;
        private bool match = false;
        private float score = 0;

        //TODO: Abstract CaptureResult so it will work with any fingerprint scanner.
        private void OnCaptured(CaptureResult captureResult)
        {
            DisplayOutput("Captured finger image....");
            match = false;
            currentCapture = new Person();
            // Check capture quality and throw an error if bad.
            if (!biometricDevice.CheckCaptureResult(captureResult)) return;

            Fingerprint newFingerPrint = new Fingerprint();
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
            previousCapture = currentCapture;
            var matchResults = String.Format("Match: {0}, Score: {1}", match, score);
            DisplayOutput(matchResults);
        }

        public BrowserForm()
        {
            InitializeComponent();

            Afis.Threshold = MATCH_THRESHOLD;

            Text = "NoID Browser";
            WindowState = FormWindowState.Maximized;

            string pathAppDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string endPath = "";
            string approle = System.Configuration.ConfigurationManager.AppSettings["approle"].ToString();

            switch (approle)
            {
                case "enrollment-kiosk":
                    endPath = @pathAppDirectory + @"/html/enrollment-kiosk.html";
                    break;
                case "enrollment-pc":
                    endPath = @pathAppDirectory + @"/html/enrollment.html";
                    break;
                case "identity-kiosk":
                    endPath = @pathAppDirectory + @"/html/identity-kiosk.html";
                    break;
                case "identity-pc":
                    endPath = @pathAppDirectory + @"/html/identity.html";
                    break;
                case "patient-portal-kiosk":
                    endPath = @pathAppDirectory + @"/html/patient-portal-kiosk.html";
                    break;
                case "patient-portal-pc":
                    endPath = @pathAppDirectory + @"/html/patient-portal-pc.html";
                    break;
                case "healthcare-node-admin-kiosk":
                    endPath = @pathAppDirectory + @"/html/healthcare-node-admin-kiosk.html";
                    break;
                case "healthcare-node-admin-pc":
                    endPath = @pathAppDirectory + @"/html/healthcare-node-admin-pc.html";
                    break;
                case "match-hub-admin-kiosk":
                    endPath = @pathAppDirectory + @"/html/match-hub-admin-kiosk.html";
                    break;
                case "match-hub-admin-pc":
                    endPath = @pathAppDirectory + @"/html/match-hub-admin-pc.html";
                    break;
                default:
                    endPath = @pathAppDirectory + @"/html/enrollment-kiosk.html";
                    break;
            }

            browser = new ChromiumWebBrowser(endPath)
            {
                Dock = DockStyle.Fill
            };

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
            var version = String.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}, Environment: {3}", Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion, bitness);
            DisplayOutput(version);
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
