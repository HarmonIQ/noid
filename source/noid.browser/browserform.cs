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
using NoID.Utilities;

namespace NoID.Browser
{
    /// <summary>
    /// Main client browser wrapper class
    /// </summary>

    public partial class BrowserForm : Form
    {
        private readonly ChromiumWebBrowser browser;
        private PatientController _patientController;
        private ProviderBridge _providerBridge;
        
        private string organizationName = System.Configuration.ConfigurationManager.AppSettings["OrganizationName"].ToString();
        private readonly string NoIDServiceName = System.Configuration.ConfigurationManager.AppSettings["NoIDServiceName"].ToString();
        private string healthcareNodeWebAddress = StringUtilities.RemoveTrailingBackSlash(System.Configuration.ConfigurationManager.AppSettings["HealthcareNodeWeb"].ToString());
        private Uri healthcareNodeFHIRAddress = new Uri(StringUtilities.RemoveTrailingBackSlash(System.Configuration.ConfigurationManager.AppSettings["HealthcareNodeFHIRAddress"].ToString()));

        public BrowserForm()
        {
            InitializeComponent();
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
                    endPath = healthcareNodeWebAddress + "/enrollment.html"; //TODO: rename to patient.html
                    browser = new ChromiumWebBrowser(endPath) { Dock = DockStyle.Fill };
                    _patientController = new PatientController(browser);
                    browser.RegisterJsObject("NoIDBridge", _patientController.PatientBridge);
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
#if DEBUG_OBJECTS
            toolStripContainer.ContentPanel.Controls.Add(browser);
#else
            this.Controls.Add(browser);
#endif
#if DEBUG_OBJECTS
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

#if DEBUG_OBJECTS
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
