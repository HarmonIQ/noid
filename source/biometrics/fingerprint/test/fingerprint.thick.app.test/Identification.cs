using System;
using System.Windows.Forms;
using DPUruNet;
using SourceAFIS.Simple;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace NoID.Biometrics
{
    public partial class Identification : Form
    {
        Person patient = new Person();
        
        

        static AfisEngine Afis = new AfisEngine();

        /// <summary>
        /// Holds the main form with many functions common to all of SDK actions.
        /// </summary>
        public Form_Main _sender;

        private const int DPFJ_PROBABILITY_ONE = 0x7fffffff;
        //private Fmd rightIndex;
        //private Fmd rightThumb;
        //private Fmd anyFinger;
        private int count;

        public Identification()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Identification_Load(object sender, System.EventArgs e)
        {
            Afis.Threshold = 20;
            txtIdentify.Text = string.Empty;
            //firstFingerPrint = null;
            //firstPerson =  null;
           
            count = 0;

            SendMessage(Action.SendMessage, "Place your first of four fingers on the reader.");

            if (!_sender.OpenReader())
            {
                this.Close();
            }

            if (!_sender.StartCaptureAsync(this.OnCaptured))
            {
                this.Close();
            }
        }

        /// <summary>
        /// Handler for when a fingerprint is captured.
        /// </summary>
        /// <param name="captureResult">contains info and data on the fingerprint capture</param>
        private void OnCaptured(CaptureResult captureResult)
        {
            try
            {
                // Check capture quality and throw an error if bad.
                if (!_sender.CheckCaptureResult(captureResult)) return;

                SendMessage(Action.SendMessage, "A finger was captured.");
                Fingerprint newFingerPrint = new Fingerprint();
                if (count < 4)
                {
                    foreach (Fid.Fiv fiv in captureResult.Data.Views)
                    {
                        newFingerPrint.AsBitmap = _sender.CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height);
                    }
                    patient.Fingerprints.Add(newFingerPrint);
                    Afis.Extract(patient);
                    count += 1;
                    if (count != 4)
                    { 
                        SendMessage(Action.SendMessage, "Now place your next finger on the reader.");
                    }
                    else
                    {
                        SendMessage(Action.SendMessage, "Now place a search finger on the reader.");
                    }
                }
                else if (count > 0)
                {
                    Person matchFinger = new Person();

                    foreach (Fid.Fiv fiv in captureResult.Data.Views)
                    {
                        newFingerPrint.AsBitmap = _sender.CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height);
                    }
                    matchFinger.Fingerprints.Add(newFingerPrint);
                    Afis.Extract(matchFinger);
                    long memUsage = GetMemoryUsage(patient);
                    float score = Afis.Verify(matchFinger, patient);
                    bool match = (score > Afis.Threshold);
                    string matchString = "AFIS doesn't match.  Get out of here kid!";
                    if (match)
                    {
                        matchString = "AFIS matches.";
                    }

                    SendMessage(Action.SendMessage, "Identification resulted: " + matchString + " score = " + score.ToString());
                    SendMessage(Action.SendMessage, "Place another finger on the reader.");
                    count += 1;
                }
            }
            catch (Exception ex)
            {
                // Send error message, then close form
                SendMessage(Action.SendMessage, "Error:  " + ex.Message);                
            }
        }

        private long GetMemoryUsage(object o)
        {
            long size = 0;
            using (MemoryStream s = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(s, o);
                size = s.Length;
            }
            return size;
        }

        /// <summary>
        /// Close window.
        /// </summary>
        private void btnBack_Click(System.Object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Close window.
        /// </summary>
        private void Identification_Closed(object sender, System.EventArgs e)
        {
            _sender.CancelCaptureAndCloseReader(this.OnCaptured);
        }

        #region SendMessage
        private enum Action
        {
            SendMessage
        }
        private delegate void SendMessageCallback(Action action, string payload);
        private void SendMessage(Action action, string payload)
        {
            try
            {
                if (this.txtIdentify.InvokeRequired)
                {
                    SendMessageCallback d = new SendMessageCallback(SendMessage);
                    this.Invoke(d, new object[] { action, payload });
                }
                else
                {
                    switch (action)
                    {
                        case Action.SendMessage:
                            txtIdentify.Text += payload + "\r\n\r\n";
                            txtIdentify.SelectionStart = txtIdentify.TextLength;
                            txtIdentify.ScrollToCaret();
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
}