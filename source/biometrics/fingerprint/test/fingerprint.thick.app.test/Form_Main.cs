using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using DPUruNet;
using System.Reflection;
using System.ComponentModel;


namespace NoID.Biometrics
{
    public partial class Form_Main : Form
    {
        /// <summary>
        /// Holds fmds enrolled by the enrollment GUI.
        /// </summary>
        public Dictionary<int, Fmd> Fmds
        {
            get { return fmds; }
            set { fmds = value; }
        }
        private Dictionary<int, Fmd> fmds = new Dictionary<int, Fmd>();
        
        /// <summary>
        /// Reset the UI causing the user to reselect a reader.
        /// </summary>
        public bool Reset
        {
            get { return reset; }
            set { reset = value; }
        }
        private bool reset;

        public Form_Main()
        {
            using (Tracer tracer = new Tracer("Form_Main::Form_Main"))
            {

                InitializeComponent();
            }
        }

        // When set by child forms, shows s/n and enables buttons.
        public Reader CurrentReader
        {
            get { return currentReader; }
            set
            {
                currentReader = value;
                SendMessage(Action.UpdateReaderState, value);
            }
        }
        private Reader currentReader;

        #region Click Event Handlers
        private ReaderSelection _readerSelection;
        private void btnReaderSelect_Click(System.Object sender, System.EventArgs e)
        {
            using (Tracer tracer = new Tracer("Form_Main::btnReaderSelect_Click"))
            {

                if (_readerSelection == null)
                {
                    _readerSelection = new ReaderSelection();
                    _readerSelection.Sender = this;
                }

                _readerSelection.ShowDialog();

                _readerSelection.Dispose();
                _readerSelection = null;
            }
        }

        private Capture _capture;
        private void btnCapture_Click(System.Object sender, System.EventArgs e)
        {
            if (_capture == null)
            {
                _capture = new Capture();
                _capture._sender = this;
            }

            _capture.ShowDialog();

            _capture.Dispose();
            _capture = null;
        }

        private Verification _verification;
        private void btnVerify_Click(System.Object sender, System.EventArgs e)
        {
            if (_verification == null)
            {
                _verification = new Verification();
                _verification._sender = this;
            }

            _verification.ShowDialog();

            _verification.Dispose();
            _verification = null;
        }

        private Identification _identification;
        private void btnIdentify_Click(System.Object sender, System.EventArgs e)
        {
            if (_identification == null)
            {
                _identification = new Identification();
                _identification._sender = this;
            }

            _identification.ShowDialog();

            _identification.Dispose();
            _identification = null;
        }

        private Enrollment _enrollment;
        private void btnEnroll_Click(System.Object sender, System.EventArgs e)
        {
            if (_enrollment == null)
            {
                _enrollment = new Enrollment();
                _enrollment._sender = this;
            }

            _enrollment.ShowDialog();

            _enrollment.Dispose();
            _enrollment = null;
        }

        private Stream _stream;
        private void btnStreaming_Click(System.Object sender, System.EventArgs e)
        {
            if (_stream == null)
            {
                _stream = new Stream();
                _stream._sender = this;
            }

            _stream.ShowDialog();

            _stream.Dispose();
            _stream = null;
        }

        EnrollmentControl enrollmentControl;
        private void btnEnrollmentControl_Click(object sender, EventArgs e)
        {
            if (enrollmentControl == null)
            {
                enrollmentControl = new EnrollmentControl();
                enrollmentControl._sender = this;
            }

            enrollmentControl.ShowDialog();

        }

        IdentificationControl identificationControl;
        private void btnIdentificationControl_Click(object sender, EventArgs e)
        {
            if (identificationControl == null)
            {
                identificationControl = new IdentificationControl();
                identificationControl._sender = this;
            }

            identificationControl.ShowDialog();

            identificationControl.Dispose();
            identificationControl = null;
        }
        #endregion

        /// <summary>
        /// Open a device and check result for errors.
        /// </summary>
        /// <returns>Returns true if successful; false if unsuccessful</returns>
        public bool OpenReader()
        {
            using (Tracer tracer = new Tracer("Form_Main::OpenReader"))
            {
                reset = false;
                Constants.ResultCode result = Constants.ResultCode.DP_DEVICE_FAILURE;

                // Open reader
                result = currentReader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);

                if (result != Constants.ResultCode.DP_SUCCESS)
                {
                    MessageBox.Show("Error:  " + result);
                    reset = true;
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Hookup capture handler and start capture.
        /// </summary>
        /// <param name="OnCaptured">Delegate to hookup as handler of the On_Captured event</param>
        /// <returns>Returns true if successful; false if unsuccessful</returns>
        public bool StartCaptureAsync(Reader.CaptureCallback OnCaptured)
        {
            using (Tracer tracer = new Tracer("Form_Main::StartCaptureAsync"))
            {
                // Activate capture handler
                currentReader.On_Captured += new Reader.CaptureCallback(OnCaptured);

                // Call capture
                if (!CaptureFingerAsync())
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Cancel the capture and then close the reader.
        /// </summary>
        /// <param name="OnCaptured">Delegate to unhook as handler of the On_Captured event </param>
        public void CancelCaptureAndCloseReader(Reader.CaptureCallback OnCaptured)
        {
            using (Tracer tracer = new Tracer("Form_Main::CancelCaptureAndCloseReader"))
            {
                if (currentReader != null)
                {
                    currentReader.CancelCapture();

                    // Dispose of reader handle and unhook reader events.
                    currentReader.Dispose();

                    if (reset)
                    {
                        CurrentReader = null;
                    }
                }
            }
        }

        /// <summary>
        /// Check the device status before starting capture.
        /// </summary>
        /// <returns></returns>
        public void GetStatus()
        {
            using (Tracer tracer = new Tracer("Form_Main::GetStatus"))
            {
                Constants.ResultCode result = currentReader.GetStatus();

                if ((result != Constants.ResultCode.DP_SUCCESS))
                {
                    reset = true;
                    throw new Exception("" + result);
                }

                if ((currentReader.Status.Status == Constants.ReaderStatuses.DP_STATUS_BUSY))
                {
                    Thread.Sleep(50);
                }
                else if ((currentReader.Status.Status == Constants.ReaderStatuses.DP_STATUS_NEED_CALIBRATION))
                {
                    currentReader.Calibrate();
                }
                else if ((currentReader.Status.Status != Constants.ReaderStatuses.DP_STATUS_READY))
                {
                    throw new Exception("Reader Status - " + currentReader.Status.Status);
                }
            }
        }
        
        /// <summary>
        /// Check quality of the resulting capture.
        /// </summary>
        public bool CheckCaptureResult(CaptureResult captureResult)
        {
            using (Tracer tracer = new Tracer("Form_Main::CheckCaptureResult"))
            {
                if (captureResult.Data == null || captureResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    if (captureResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    {
                        reset = true;
                        throw new Exception(captureResult.ResultCode.ToString());
                    }

                    // Send message if quality shows fake finger
                    if ((captureResult.Quality != Constants.CaptureQuality.DP_QUALITY_CANCELED))
                    {
                        throw new Exception("Quality - " + captureResult.Quality);
                    }
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Function to capture a finger. Always get status first and calibrate or wait if necessary.  Always check status and capture errors.
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public bool CaptureFingerAsync()
        {
            using (Tracer tracer = new Tracer("Form_Main::CaptureFingerAsync"))
            {
                try
                {
                    GetStatus();

                    Constants.ResultCode captureResult = currentReader.CaptureAsync(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, currentReader.Capabilities.Resolutions[0]);
                    if (captureResult != Constants.ResultCode.DP_SUCCESS)
                    {
                        reset = true;
                        throw new Exception("" + captureResult);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:  " + ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Create a bitmap from raw data in row/column format.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Bitmap CreateBitmap(byte[] bytes, int width, int height)
        {
            byte[] rgbBytes = new byte[bytes.Length * 3];

            for (int i = 0; i <= bytes.Length - 1; i++)
            {
                rgbBytes[(i * 3)] = bytes[i];
                rgbBytes[(i * 3) + 1] = bytes[i];
                rgbBytes[(i * 3) + 2] = bytes[i];
            }
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            for (int i = 0; i <= bmp.Height - 1; i++)
            {
                IntPtr p = new IntPtr(data.Scan0.ToInt64() + data.Stride * i);
                System.Runtime.InteropServices.Marshal.Copy(rgbBytes, i * bmp.Width * 3, p, bmp.Width * 3);
            }

            bmp.UnlockBits(data);

            return bmp;
        }

        #region SendMessage
        private enum Action
        {
           UpdateReaderState 
        }
        private delegate void SendMessageCallback(Action state, object payload);
        private void SendMessage(Action state, object payload)
        {
            using (Tracer tracer = new Tracer("Form_Main::SendMessage"))
            {

                if (this.txtReaderSelected.InvokeRequired)
                {
                    SendMessageCallback d = new SendMessageCallback(SendMessage);
                    this.Invoke(d, new object[] { state, payload });
                }
                else
                {
                    switch (state)
                    {
                        case Action.UpdateReaderState:
                            if ((Reader)payload != null)
                            {
                                txtReaderSelected.Text = ((Reader)payload).Description.SerialNumber;
                                btnCapture.Enabled = true;
                                btnStreaming.Enabled = true;
                                btnVerify.Enabled = true;
                                btnIdentify.Enabled = true;
                                btnEnroll.Enabled = true;
                                btnEnrollmentControl.Enabled = true;
                                if (fmds.Count > 0)
                                {
                                    btnIdentificationControl.Enabled = true;
                                }
                            }
                            else
                            {
                                txtReaderSelected.Text = String.Empty;
                                btnCapture.Enabled = false;
                                btnStreaming.Enabled = false;
                                btnVerify.Enabled = false;
                                btnIdentify.Enabled = false;
                                btnEnroll.Enabled = false;
                                btnEnrollmentControl.Enabled = false;
                                btnIdentificationControl.Enabled = false;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        #endregion
    }
}