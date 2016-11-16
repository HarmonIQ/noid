using System;
using System.Windows.Forms;
using DPUruNet;
using SourceAFIS.Simple;
using System.Drawing.Imaging;

namespace NoID.Biometrics
{
    public partial class Verification : Form
    {
        /// <summary>
        /// Holds the main form with many functions common to all of SDK actions.
        /// </summary>
        public Form_Main _sender;

        Fingerprint fp1 = new Fingerprint();
        Person person1 = new Person();
        Fingerprint fp2 = new Fingerprint();
        Person person2 = new Person();
        
        static AfisEngine Afis = new AfisEngine();

        private const int PROBABILITY_ONE = 0x7fffffff;
        //private Fmd firstFinger;
        //private Fmd secondFinger;
        private int count;
  
        public Verification()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Verification_Load(object sender, System.EventArgs e)
        {
            Afis.Threshold = 15;
            txtVerify.Text = string.Empty;
            //firstFinger = null;
            //secondFinger = null;
            count = 0;

            SendMessage(Action.SendMessage, "Place a finger on the reader.");

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

                //DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromFid(captureResult.Data, Constants.Formats.Fmd.ANSI);
                //if (resultConversion.ResultCode != Constants.ResultCode.DP_SUCCESS)
                //{
                //    _sender.Reset = true;
                //    throw new Exception(resultConversion.ResultCode.ToString());
                //}

                if (count == 0)
                {
                    //firstFinger = resultConversion.Data;
                    // Create bitmap
                    foreach (Fid.Fiv fiv in captureResult.Data.Views)
                    {
                        fp1.AsBitmap = _sender.CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height);
                    }
                    person1.Fingerprints.Add(fp1);
                    Afis.Extract(person1);
                    //string serializedFinger = Fmd.SerializeXml(firstFinger);
                    count += 1;
                    SendMessage(Action.SendMessage, "Now place the same or a different finger on the reader.");
                }
                else if (count == 1)
                {
                    //secondFinger = resultConversion.Data;
                    foreach (Fid.Fiv fiv in captureResult.Data.Views)
                    {
                        fp2.AsBitmap = _sender.CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height);
                    }
                    person2.Fingerprints.Add(fp2);
                   
                    Afis.Extract(person2);
                    float score = Afis.Verify(person1, person2);
                    bool match = (score > 23);
                    string matchString = "AFIS doesn't match.  Get out of here kid!";
                    if (match)
                    {
                        matchString = "AFIS matches.";
                    }
                    //CompareResult compareResult = Comparison.Compare(firstFinger, 0, secondFinger, 0);
                    //string serializedFinger = Fmd.SerializeXml(secondFinger);
                    //if (compareResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    //{
                    //    _sender.Reset = true;
                    //    throw new Exception(compareResult.ResultCode.ToString());
                    //}
                    SendMessage(Action.SendMessage, "SourceAFIS = " + matchString + " " + score.ToString());
                    //SendMessage(Action.SendMessage, "Comparison resulted in a dissimilarity score of " + compareResult.Score.ToString() + (compareResult.Score < (PROBABILITY_ONE / 100000) ? " (fingerprints matched)" : " (fingerprints did not match)"));
                    SendMessage(Action.SendMessage, "Place a finger on the reader.");
                    count = 0;
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
        private void Verification_Closed(object sender, System.EventArgs e)
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
                if (this.txtVerify.InvokeRequired)
                {
                    SendMessageCallback d = new SendMessageCallback(SendMessage);
                    this.Invoke(d, new object[] { action, payload });
                }
                else
                {
                    switch (action)
                    {
                        case Action.SendMessage:
                            txtVerify.Text += payload + "\r\n\r\n";
                            txtVerify.SelectionStart = txtVerify.TextLength;
                            txtVerify.ScrollToCaret();
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