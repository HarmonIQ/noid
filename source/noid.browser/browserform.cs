// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.
// Copyright © 2010-2017 The CefSharp Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Windows.Forms;
using System.Configuration;
using System.Collections.Generic;
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

namespace NoID.Browser
{
    /// <summary>
    /// Main client browser wrapper class
    /// </summary>

    public partial class BrowserForm : Form
    {
        private readonly string MinimumAcceptedMatchScore = ConfigurationManager.AppSettings["MinimumAcceptedMatchScore"].ToString();
        private readonly uint _minimumAcceptedMatchScore;
        private readonly string SearchBiometricsUri = ConfigurationManager.AppSettings["SearchBiometricsUri"].ToString();
        private readonly string organizationName = ConfigurationManager.AppSettings["OrganizationName"].ToString();
        private readonly string NoIDServiceName = ConfigurationManager.AppSettings["NoIDServiceName"].ToString();
        private readonly string approle = ConfigurationManager.AppSettings["approle"].ToString();
        
        private static AfisEngine Afis = new AfisEngine();
        private MinutiaCaptureController _firstMinutiaCaptureController;
        private MinutiaCaptureController _minutiaCaptureController;

        //TODO: Use one object for both bridges.
        private static PatientBridge _patientBridge;
        private static ProviderBridge _providerBridge;

        private const float PROBE_MATCH_THRESHOLD = 70;
        private readonly ChromiumWebBrowser browser;

        //TODO: Abstract biometricDevice so it will work with any fingerprint scanner.
        private DigitalPersona biometricDevice;

        

        //private Uri healthcareNodeFHIRAddress;
        private string healthcareNodeWebAddress;
        private string healthcareNodeChainVerifyAddress;
		private int maxFingerprintScanAttempts = 1; //accept at least one attempt
		private int fingerprintScanAttempts = 0;
		private List<string> attemptedScannedFingers = new List<string>();
		private bool hasLeftFingerprintScan = true;
		private bool hasRightFingerprintScan = true;
		private bool currentCaptureInProcess = false;
		
		public BrowserForm()
        {
            InitializeComponent();
            this.MinimizeBox = false;
            this.MaximizeBox = false;

            healthcareNodeWebAddress = StringUtilities.RemoveTrailingBackSlash(ConfigurationManager.AppSettings["HealthcareNodeWeb"].ToString());
            healthcareNodeChainVerifyAddress = StringUtilities.RemoveTrailingBackSlash(ConfigurationManager.AppSettings["HealthcareNodeChainVerifyAddress"].ToString());

            Text = "NoID Browser";
            WindowState = FormWindowState.Maximized;
            
            string endPath = "";
            

            switch (approle)
            {
                case "patient":
                case "enrollment-pc":
                case "enrollment-kiosk":
                case "patient-pc":
                case "patient-kiosk":
                    if (uint.TryParse(MinimumAcceptedMatchScore, out _minimumAcceptedMatchScore) == false)
                    {
                        _minimumAcceptedMatchScore = 75;
                    }
                    Afis.Threshold = _minimumAcceptedMatchScore;
                    _minutiaCaptureController = new MinutiaCaptureController(_minimumAcceptedMatchScore);
                    endPath = endPath = healthcareNodeWebAddress + "/enrollment.html"; //TODO: rename to patient.html
                    browser = new ChromiumWebBrowser(endPath) { Dock = DockStyle.Fill };
                    _patientBridge = new PatientBridge(organizationName, NoIDServiceName);
                    browser.RegisterJsObject("NoIDBridge", _patientBridge);
                    _patientBridge.ResetSession += ResetSessions;
                    _patientBridge.JavaScriptAsync += ExecuteJavaScriptAsync;
                    break;
                case "provider":
                case "provider-pc":
                case "provider-kiosk":
                    endPath = healthcareNodeWebAddress + "/provider.html";
                    browser = new ChromiumWebBrowser(endPath) { Dock = DockStyle.Fill };
                    _providerBridge = new ProviderBridge(organizationName, NoIDServiceName);
                    browser.RegisterJsObject("NoIDBridge", _providerBridge);
                    _providerBridge.JavaScriptAsync += ExecuteJavaScriptAsync;
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
			browser.GetMainFrame().ExecuteJavaScriptAsync("showPleaseWait();");
			//mark schroeder20170703
			if (PatientBridge.cannotCaptureLeftFingerprint == true)
			{
				Laterality = FHIRUtilities.LateralitySnoMedCode.Right;
			}		

			if (currentCaptureInProcess == false)
			{
                if ((PatientBridge.hasValidLeftFingerprint == true && Laterality == FHIRUtilities.LateralitySnoMedCode.Left) || (PatientBridge.hasValidRightFingerprint == true && Laterality == FHIRUtilities.LateralitySnoMedCode.Right))
				{
					//mark schroeder 20170701 do not capture more left fingerprints if left is set. Same for right
					return;
				}
				else
				{
					try
					{
						//mark schroeder 20170701 use this to stop more capture attempts while processing. Added to below if statememt
						currentCaptureInProcess = true;
						maxFingerprintScanAttempts = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["maxFingerprintScanAttempts"].ToString());
						fingerprintScanAttempts++;
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
						return;
					}

					if (_patientBridge.captureSite != FHIRUtilities.CaptureSiteSnoMedCode.Unknown && _patientBridge.laterality != FHIRUtilities.LateralitySnoMedCode.Unknown)
					{
						if (fingerprintScanAttempts <= maxFingerprintScanAttempts)
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
									// Good pair found.  Query web service for a match.
									FingerPrintMinutias newFingerPrintMinutias = new FingerPrintMinutias
											(SessionID, _minutiaCaptureController.BestTemplate1, Laterality, CaptureSite);
									PatientBridge.PatientFHIRProfile.AddFingerPrint(newFingerPrintMinutias, 
                                        deviceName, 
                                        _minutiaCaptureController.BestTemplate1.OriginalDpi, 
                                        _minutiaCaptureController.BestTemplate1.OriginalHeight, 
                                        _minutiaCaptureController.BestTemplate1.OriginalWidth);

                                    newFingerPrintMinutias = new FingerPrintMinutias
										(SessionID, _minutiaCaptureController.BestTemplate2, Laterality, CaptureSite);
									PatientBridge.PatientFHIRProfile.AddFingerPrint(newFingerPrintMinutias, 
                                        deviceName, 
                                        _minutiaCaptureController.BestTemplate2.OriginalDpi, 
                                        _minutiaCaptureController.BestTemplate2.OriginalHeight, 
                                        _minutiaCaptureController.BestTemplate2.OriginalWidth);

									Media media = PatientBridge.PatientFHIRProfile.FingerPrintFHIRMedia(newFingerPrintMinutias, deviceName, tmpCurrent.OriginalDpi, tmpCurrent.OriginalHeight, tmpCurrent.OriginalWidth);

                                    // TODO: REMOVE THIS LINE!  ONLY FOR TESTING
                                    //FHIRUtilities.SaveJSONFile(media, @"C:\JSONTest");

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
                                    PatientBridge.fhirAddress = new Uri(SearchBiometricsUri);
                                    dataTransport.SendFHIRMediaProfile(PatientBridge.fhirAddress, auth, media);
									string lateralityString = FHIRUtilities.LateralityToString(Laterality);
									string captureSiteString = FHIRUtilities.CaptureSiteToString(CaptureSite);
#if NAVIGATE
									string output = lateralityString + " " + captureSiteString + " fingerprint accepted. Score = " + _minutiaCaptureController.BestScore + ", Fingerprint sent to server: Response = " + dataTransport.ResponseText;
									DisplayOutput(output);
#endif
                                    if (dataTransport.ResponseText.ToLower().Contains("error") == true || dataTransport.ResponseText.ToLower().Contains("index") == true)
                                    {
                                        string message = "Critical Identity Error Occured In Fingerprint Capture method. Please contact your adminstrator: " + dataTransport.ResponseText + " Error code = 909";
                                        MessageBox.Show(message);
                                        browser.GetMainFrame().ExecuteJavaScriptAsync("pageRefresh();");
                                        return;
                                    }
									if (dataTransport.ResponseText.ToLower().Contains(@"noid://") == true)
									{
										// Match found, inform JavaScript that this is an returning patient for Identity.
										PatientBridge.PatientFHIRProfile.LocalNoID = dataTransport.ResponseText;  //save the localNoID
										PatientBridge.PatientFHIRProfile.NoIDStatus = "Pending";										
										browser.GetMainFrame().ExecuteJavaScriptAsync("showIdentity('" + PatientBridge.PatientFHIRProfile.LocalNoID + "');");
									}
                                    else if (dataTransport.ResponseText.ToLower() == "pending")
                                    {
                                        MessageBox.Show("You are already checked in.  If you believe this is an error, please contact staff");
                                        browser.GetMainFrame().ExecuteJavaScriptAsync("pageRefresh();");
                                        return;
                                    }
									else
									{
                                        if (PatientBridge.hasValidLeftFingerprint == true)
                                        {
                                            if (MatchLeftAndRight() == true)
                                            {
                                                MessageBox.Show("Both right and left capture sites are the same.  Please start over.");
                                                browser.GetMainFrame().ExecuteJavaScriptAsync("pageRefresh();");
                                                return;
                                            }
                                        }
                                        // Match not found, inform JavaScript the capture pair is complete and the patient can move to the next step.
                                        browser.GetMainFrame().ExecuteJavaScriptAsync("showComplete('" + Laterality.ToString() + "');");
                                        if (Laterality == FHIRUtilities.LateralitySnoMedCode.Left)
										{
											Laterality = FHIRUtilities.LateralitySnoMedCode.Right;
                                            CaptureSite = FHIRUtilities.CaptureSiteSnoMedCode.IndexFinger;
                                            _firstMinutiaCaptureController = _minutiaCaptureController;
                                            _minutiaCaptureController = new MinutiaCaptureController(_minimumAcceptedMatchScore);
                                            PatientBridge.hasValidLeftFingerprint = true;
										}
										else if (Laterality == FHIRUtilities.LateralitySnoMedCode.Right)
										{
											Laterality = FHIRUtilities.LateralitySnoMedCode.Unknown;
											CaptureSite = FHIRUtilities.CaptureSiteSnoMedCode.Unknown;
											PatientBridge.hasValidRightFingerprint = true;
                                        }
										fingerprintScanAttempts = 0; //reset scan attempt count on successful scan
									}
								}
								else
								{
									// Good fingerprint pairs not found yet.  inform JavaScript to promt the patient to try again.
									browser.GetMainFrame().ExecuteJavaScriptAsync("showFail('" + Laterality.ToString() + "');");
#if NAVIGATE
									DisplayOutput("Fingerprint NOT accepted. Score = " + _minutiaCaptureController.BestScore);
#endif
                                    currentCaptureInProcess = false;
                                    return;
								}
							}
						}
						else
						{
							//int testy = CaptureSite.ToString().IndexOf("Thumb");
							if (CaptureSite.ToString().IndexOf("Thumb") == -1)
							{
								browser.GetMainFrame().ExecuteJavaScriptAsync("alert('You have exceeded the maximum allowed scan attempts for your " + Laterality.ToString() + " " + CaptureSite + " Lets try another finger.');");
							}
							fingerprintScanAttempts = 0; //reset scan attempt count on successful scan
														 //get next laterality and capture site. Order of precedence is left, then right. Index, middle, ring, little, thumb										
							switch (Laterality.ToString() + CaptureSite.ToString())
							{
								case "LeftIndexFinger":
									attemptedScannedFingers.Add(Laterality.ToString() + CaptureSite.ToString());
                                    browser.GetMainFrame().ExecuteJavaScriptAsync("setLateralitySite('selectLeftMiddle');");
                                    CaptureSite = FHIRUtilities.CaptureSiteSnoMedCode.MiddleFinger;
                                    Laterality = FHIRUtilities.LateralitySnoMedCode.Left;
                                    break;
								case "LeftMiddleFinger":
									attemptedScannedFingers.Add(Laterality.ToString() + CaptureSite.ToString());
									browser.GetMainFrame().ExecuteJavaScriptAsync("setLateralitySite('selectLeftRing');");
                                    CaptureSite = FHIRUtilities.CaptureSiteSnoMedCode.RingFinger;
                                    Laterality = FHIRUtilities.LateralitySnoMedCode.Left;
                                    break;
								case "LeftRingFinger":
									attemptedScannedFingers.Add(Laterality.ToString() + CaptureSite.ToString());
									browser.GetMainFrame().ExecuteJavaScriptAsync("setLateralitySite('selectLeftLittle');");
                                    CaptureSite = FHIRUtilities.CaptureSiteSnoMedCode.LittleFinger;
                                    Laterality = FHIRUtilities.LateralitySnoMedCode.Left;
                                    break;
								case "LeftLittleFinger":
									attemptedScannedFingers.Add(Laterality.ToString() + CaptureSite.ToString());
									browser.GetMainFrame().ExecuteJavaScriptAsync("setLateralitySite('selectLeftThumb');");
                                    CaptureSite = FHIRUtilities.CaptureSiteSnoMedCode.Thumb;
                                    Laterality = FHIRUtilities.LateralitySnoMedCode.Left;
                                    break;
								case "LeftThumb":
									attemptedScannedFingers.Add(Laterality.ToString() + CaptureSite.ToString());
									browser.GetMainFrame().ExecuteJavaScriptAsync("moveToRightHandScan();");
                                    CaptureSite = FHIRUtilities.CaptureSiteSnoMedCode.IndexFinger;
                                    Laterality = FHIRUtilities.LateralitySnoMedCode.Right;
                                    hasLeftFingerprintScan = false;
									break;
								case "RightIndexFinger":
									hasLeftFingerprintScan = _patientBridge.hasValidLeftFingerprint;
									attemptedScannedFingers.Add(Laterality.ToString() + CaptureSite.ToString());
									browser.GetMainFrame().ExecuteJavaScriptAsync("setLateralitySite('selectRightMiddle');");
                                    CaptureSite = FHIRUtilities.CaptureSiteSnoMedCode.MiddleFinger;
                                    Laterality = FHIRUtilities.LateralitySnoMedCode.Right;
                                    break;
								case "RightMiddleFinger":
									attemptedScannedFingers.Add(Laterality.ToString() + CaptureSite.ToString());
									browser.GetMainFrame().ExecuteJavaScriptAsync("setLateralitySite('selectRightRing');");
                                    CaptureSite = FHIRUtilities.CaptureSiteSnoMedCode.RingFinger;
                                    Laterality = FHIRUtilities.LateralitySnoMedCode.Right;
                                    break;
								case "RightRingFinger":
									attemptedScannedFingers.Add(Laterality.ToString() + CaptureSite.ToString());
									browser.GetMainFrame().ExecuteJavaScriptAsync("setLateralitySite('selectRightLittle');");
                                    CaptureSite = FHIRUtilities.CaptureSiteSnoMedCode.LittleFinger;
                                    Laterality = FHIRUtilities.LateralitySnoMedCode.Right;
                                    break;
								case "RightLittleFinger":
									attemptedScannedFingers.Add(Laterality.ToString() + CaptureSite.ToString());
									browser.GetMainFrame().ExecuteJavaScriptAsync("setLateralitySite('selectRightThumb');");
                                    CaptureSite = FHIRUtilities.CaptureSiteSnoMedCode.Thumb;
                                    Laterality = FHIRUtilities.LateralitySnoMedCode.Right;
                                    break;
								case "RightThumb":
									attemptedScannedFingers.Add(Laterality.ToString() + CaptureSite.ToString());
									hasRightFingerprintScan = false;
                                    _minutiaCaptureController = null;
                                    if (hasLeftFingerprintScan == false)
                                    {
                                        _firstMinutiaCaptureController = null;
                                    }
                                    CaptureSite = FHIRUtilities.CaptureSiteSnoMedCode.Unknown;
                                    Laterality = FHIRUtilities.LateralitySnoMedCode.Unknown;
                                    browser.GetMainFrame().ExecuteJavaScriptAsync("clickNoRightHandFingerPrint();");
                                    break;
								default:
									break;
							}
							//define walk through fingers and ability to override
						}
					}
					else
					{
						if (hasLeftFingerprintScan == true && hasRightFingerprintScan == true)
						{
							browser.GetMainFrame().ExecuteJavaScriptAsync("alert('You have successfully completed this step. Please proceed to the next page by clicking the NEXT button below');");
						}
						else
						{
							browser.GetMainFrame().ExecuteJavaScriptAsync("alert('Must be on the correct page to accept a fingerprint scan. Please follow the instructions on the screen.');");
						}
#if NAVIGATE
						DisplayOutput("Must be on the correct page to accept a fingerprint scan.");
#endif
					}
					currentCaptureInProcess = false;
				}
			}
			else
			{
				browser.GetMainFrame().ExecuteJavaScriptAsync("alert('Current Scan In Process. Please wait and follow the on screen instructions.');");
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

        bool MatchLeftAndRight()
        {
            bool result = false;
            if (_firstMinutiaCaptureController != null && _minutiaCaptureController != null)
            {
                if (_firstMinutiaCaptureController.BestTemplate1 != null && _minutiaCaptureController.BestTemplate1 != null && _firstMinutiaCaptureController.BestTemplate2 != null && _minutiaCaptureController.BestTemplate2 != null)
                {
                    result = _firstMinutiaCaptureController.MatchBest(_minutiaCaptureController.BestTemplate1);
                    if (result == false)
                    {
                        result = _firstMinutiaCaptureController.MatchBest(_minutiaCaptureController.BestTemplate2);
                    }
                }
            }
            return result;
        }

        void ExecuteJavaScriptAsync(object sender, string javaScriptToExecute)
        {
            browser.GetMainFrame().ExecuteJavaScriptAsync(javaScriptToExecute);
        }

        void ResetSessions(object sender, string trigger)
        {
            switch (approle)
            {
                case "patient":
                case "enrollment-pc":
                case "enrollment-kiosk":
                case "patient-pc":
                case "patient-kiosk":
					//_patientBridge = new PatientBridge(organizationName, NoIDServiceName);
					_patientBridge.Dispose();
					fingerprintScanAttempts = 0;
                    attemptedScannedFingers = new List<string>();
                    hasLeftFingerprintScan = true;
                    hasRightFingerprintScan = true;
                    currentCaptureInProcess = false;
                    _firstMinutiaCaptureController = null;
                    _minutiaCaptureController = new MinutiaCaptureController(_minimumAcceptedMatchScore);
                    //browser.RegisterJsObject("NoIDBridge", _patientBridge);
                    break;
                case "provider":
                case "provider-pc":
                case "provider-kiosk":
                    _providerBridge = new ProviderBridge(organizationName, NoIDServiceName);
                    //browser.RegisterJsObject("NoIDBridge", _providerBridge);
                    break;
            }
        }


        PatientBridge PatientBridge
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
