using System;
using System.Drawing;
using System.Windows.Forms;
using DPUruNet;
using SourceAFIS;
using System.Xml.Linq;

namespace NoID.Biometrics
{
    public partial class Capture : Form
    {
        /// <summary>
        /// Holds the main form with many functions common to all of SDK actions.
        /// </summary>
        public Form_Main _sender;
        private static XName xmlName;
        private static XElement xml = new XElement(xmlName);
        private static FingerprintTemplate template = new FingerprintTemplate(xml);
        private FingerprintMatcher matcher = new FingerprintMatcher(template);
        public Capture()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Capture_Load(object sender, EventArgs e)
        {
            // Reset variables
            pbFingerprint.Image = null;

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
        public void OnCaptured(CaptureResult captureResult)
        {
            try
            {
                // Check capture quality and throw an error if bad.
                if (!_sender.CheckCaptureResult(captureResult)) return;

                // Create bitmap
                foreach (Fid.Fiv fiv in captureResult.Data.Views)
                {
                    SendMessage(Action.SendBitmap, _sender.CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height));
                }
            }
            catch (Exception ex)
            {
                // Send error message, then close form
                SendMessage(Action.SendMessage, "Error:  " + ex.Message);                
            }
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
        private void Capture_Closed(object sender, EventArgs e)
        {
            _sender.CancelCaptureAndCloseReader(this.OnCaptured);
        }

        #region SendMessage
        private enum Action
        {
            SendBitmap,
            SendMessage
        }
        private delegate void SendMessageCallback(Action action, object payload);
        private void SendMessage(Action action, object payload)
        {
            try
            {
                if (this.pbFingerprint.InvokeRequired)
                {
                    SendMessageCallback d = new SendMessageCallback(SendMessage);
                    this.Invoke(d, new object[] { action, payload });
                }
                else
                {
                    switch (action)
                    {
                        case Action.SendMessage:
                            MessageBox.Show((string)payload);
                            break;
                        case Action.SendBitmap:
                            pbFingerprint.Image = (Bitmap)payload;
                            pbFingerprint.Refresh();
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