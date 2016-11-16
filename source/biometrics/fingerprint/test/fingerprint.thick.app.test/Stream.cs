using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using DPUruNet;

namespace NoID.Biometrics
{
    public partial class Stream : Form
    {
        /// <summary>
        /// Holds the main form with many functions common to all of SDK actions.
        /// </summary>
        public Form_Main _sender;
        private bool reset = false;

        public Stream()
        {
            InitializeComponent();
        }

        private void Stream_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();

            pbFingerprint.Image = null;

            if (!_sender.OpenReader())
            {
                MessageBox.Show("Cannot open this reader in this environment.");
                this.Close();
                return;
            }

            if (!_sender.CurrentReader.Capabilities.CanStream)
            {
                MessageBox.Show("This reader cannot stream in this environment.");
                return;
            }

            _sender.CurrentReader.StartStreaming();

            CaptureResult captureResult = null;

            reset = false;
            while ((!reset))
            {
                captureResult = _sender.CurrentReader.GetStreamImage(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, _sender.CurrentReader.Capabilities.Resolutions[0]);

                Application.DoEvents();

                if (captureResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    if (_sender.CurrentReader != null)
                    {
                        _sender.CurrentReader.Dispose();
                        _sender.CurrentReader = null;
                    }
                    reset = true;
                    MessageBox.Show("Error:  " + captureResult.ResultCode.ToString());
                }

                if (captureResult.Data != null)
                {
                    foreach (Fid.Fiv fiv in captureResult.Data.Views)
                    {
                        SendMessage(_sender.CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height));
                    }
                }
            }
        }

        /// <summary>
        /// Close window.
        /// </summary>
        private void btnBack_Click(System.Object sender, System.EventArgs e)
        {
            reset = true;

            this.Close();
        }

        /// <summary>
        /// Close window.
        /// </summary>
        private void Stream_Closed(object sender, EventArgs e)
        {
            for (int i = 0; i < 20; i++)
            {
                Thread.Sleep(25);
                Application.DoEvents();
            }

            reset = true;

            for (int i = 0; i < 20; i++)
            {
                Thread.Sleep(25);
                Application.DoEvents();
            }

            if (_sender.CurrentReader != null)
            {
                _sender.CurrentReader.StopStreaming();
                _sender.CurrentReader.Dispose();
            }
        }

        #region SendMessage
        private delegate void SendMessageCallback(object payload);
        private void SendMessage(object payload)
        {
            try
            {
                if (this.pbFingerprint.InvokeRequired)
                {
                    SendMessageCallback d = new SendMessageCallback(SendMessage);
                    this.Invoke(d, new object[] { payload });
                }
                else
                {
                    pbFingerprint.Image = (Bitmap)payload;
                    pbFingerprint.Refresh();
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
}