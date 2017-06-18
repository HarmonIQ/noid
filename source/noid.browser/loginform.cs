// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Windows.Forms;
using NoID.Security;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using System.Text;

namespace NoID.Browser
{
    public partial class LoginForm : Form
    {
        
        public LoginForm()
        {
            InitializeComponent();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            PasswordManager.SavePassword(maskedTextBoxPassword.Text, textBoxUserName.Text);
            labelStatus.Text = "Status: Password Set";
        }

        private void buttonTestAuthentication_Click(object sender, EventArgs e)
        {
            string username = textBoxUserName.Text;
            string pwd = PasswordManager.GetPassword(textBoxUserName.Text);
            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + pwd));
            Uri endpoint = new Uri(Utilities.RemoveTrailingBackSlash(System.Configuration.ConfigurationManager.AppSettings["HealthcareNodeFHIRAddress"].ToString()));
            FhirClient client = new FhirClient(endpoint);
            client.LastRequest.Headers.Add("Authorization", "Basic " + svcCredentials);
            Patient newPatient = Utilities.CreateTestFHIRPatientProfile();

            try
            {
                client.Create(newPatient);
                labelStatus.Text = "Status: Authentication Successful";
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Status: Authentication Failed " + ex.Message;
            }
        }
    }
}
