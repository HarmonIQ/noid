using System.Windows.Forms;
using DPUruNet;

namespace NoID.Biometrics
{
    public partial class Capabilities : Form
    {
        private ReaderSelection _sender;

        public ReaderSelection Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }

        public Capabilities()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(System.Object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Open a device in cooperative mode.  Display capabilities of the reader.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Capabilities_Load(object sender, System.EventArgs e)
        {
            // Clear capabilities display.
            lstCaps.BeginUpdate();
            lstCaps.Items.Clear();
            lstCaps.EndUpdate();

            Constants.ResultCode result = Constants.ResultCode.DP_DEVICE_FAILURE;

            result = _sender.CurrentReader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);

            if (result != Constants.ResultCode.DP_SUCCESS)
            {
                MessageBox.Show("Error:  " + result);
                if (_sender.CurrentReader != null)
                {
                    _sender.CurrentReader.Dispose();
                    _sender.CurrentReader = null;
                }
                return;
            }

            // Update display.

            txtName.Text = _sender.CurrentReader.Description.Name;
            txtReaderSelected.Text = _sender.CurrentReader.Description.SerialNumber;

            lstCaps.BeginUpdate();

            lstCaps.Items.Add("Can Capture:  " + _sender.CurrentReader.Capabilities.CanCapture.ToString());
            lstCaps.Items.Add("Can Stream:  " + _sender.CurrentReader.Capabilities.CanStream.ToString());
            lstCaps.Items.Add("Extract Features:  " + _sender.CurrentReader.Capabilities.ExtractFeatures.ToString());
            lstCaps.Items.Add("Can Match:  " + _sender.CurrentReader.Capabilities.CanMatch.ToString());
            lstCaps.Items.Add("Can Identify:  " + _sender.CurrentReader.Capabilities.CanIdentify.ToString());
            lstCaps.Items.Add("Has Fingerprint Storage:  " + _sender.CurrentReader.Capabilities.HasFingerprintStorage.ToString());
            lstCaps.Items.Add("Has Power Management:  " + _sender.CurrentReader.Capabilities.HasPowerManagement.ToString());
            lstCaps.Items.Add("PIV Compliant:  " + _sender.CurrentReader.Capabilities.PIVCompliant.ToString());
            lstCaps.Items.Add("Indicator Type:  " + _sender.CurrentReader.Capabilities.IndicatorType.ToString());

            foreach (int resolution in _sender.CurrentReader.Capabilities.Resolutions)
            {
                if (!(resolution == 0))
                {
                    lstCaps.Items.Add("Resolution:  " + resolution.ToString());
                }
            }

            lstCaps.EndUpdate();

            _sender.CurrentReader.Dispose();
        }
    }
}