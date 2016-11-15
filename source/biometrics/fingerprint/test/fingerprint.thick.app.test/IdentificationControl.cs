using System;
using System.Windows.Forms;
using DPUruNet;

namespace NoID.Biometrics
{
    public partial class IdentificationControl : Form
    {
        public Form_Main _sender;
        private const int DPFJ_PROBABILITY_ONE = 0x7fffffff;
        private DPCtlUruNet.IdentificationControl identificationControl;

        

        public IdentificationControl()
        {
            InitializeComponent();
        }

        private void IdentificationControl_Load(object sender, EventArgs e)
        {
            if (identificationControl != null)
            {
                identificationControl.Reader = _sender.CurrentReader;
            }
            else
            {
                // See the SDK documentation for an explanation on threshold scores.
                int thresholdScore = DPFJ_PROBABILITY_ONE * 1 / 100000;

                identificationControl = new DPCtlUruNet.IdentificationControl(_sender.CurrentReader, _sender.Fmds.Values, thresholdScore, 10, Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);
                identificationControl.Location = new System.Drawing.Point(3, 3);
                identificationControl.Name = "identificationControl";
                identificationControl.Size = new System.Drawing.Size(397, 128);
                identificationControl.TabIndex = 0;
                identificationControl.OnIdentify += new DPCtlUruNet.IdentificationControl.FinishIdentification(this.identificationControl_OnIdentify);

                // Be sure to set the maximum number of matches you want returned.
                identificationControl.MaximumResult = 10;

                this.Controls.Add(identificationControl);
            }

            identificationControl.StartIdentification();
        }

        private void identificationControl_OnIdentify(DPCtlUruNet.IdentificationControl IdentificationControl, IdentifyResult IdentificationResult)
        {
            if (IdentificationResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
            {
                if (IdentificationResult.Indexes == null)
                {
                    if (IdentificationResult.ResultCode == Constants.ResultCode.DP_INVALID_PARAMETER)
                    {
                        MessageBox.Show("Warning: Fake finger was detected.");
                    }
                    else if (IdentificationResult.ResultCode == Constants.ResultCode.DP_NO_DATA)
                    {
                        MessageBox.Show("Warning: No finger was detected.");
                    }
                    else
                    {
                        if (_sender.CurrentReader != null)
                        {
                            _sender.CurrentReader.Dispose();
                            _sender.CurrentReader = null;
                        }
                    }
                }
                else
                {
                    if (_sender.CurrentReader != null)
                    {
                        _sender.CurrentReader.Dispose();
                        _sender.CurrentReader = null;
                    }

                    MessageBox.Show("Error:  " + IdentificationResult.ResultCode);
                }
            }
            else
            {
                _sender.CurrentReader = IdentificationControl.Reader;
                txtMessage.Text = txtMessage.Text + "OnIdentify:  " + (IdentificationResult.Indexes.Length.Equals(0) ? "No " : "One or more ") + "matches.  Try another finger.\r\n\r\n";
            }

            txtMessage.SelectionStart = txtMessage.TextLength;
            txtMessage.ScrollToCaret();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void IdentificationControl_Closed(object sender, EventArgs e)
        {
            identificationControl.StopIdentification();
        }
    }
}
//! @endcond