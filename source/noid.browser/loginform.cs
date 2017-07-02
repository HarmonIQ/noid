﻿// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Windows.Forms;
using Hl7.Fhir.Model;
using NoID.Security;
using NoID.Network.Client;
using NoID.Utilities;

namespace NoID.Browser
{
    public partial class LoginForm : Form
    {
        
        public LoginForm()
        {
            InitializeComponent();
            textBoxUserName.Text = System.Configuration.ConfigurationManager.AppSettings["NoIDServiceName"].ToString();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            PasswordManager.SavePassword(maskedTextBoxPassword.Text, textBoxUserName.Text);
            string pwd = PasswordManager.GetPassword(textBoxUserName.Text);
            if (pwd == maskedTextBoxPassword.Text)
            {
                labelStatus.Text = "Status: Password Set";
            }
            else
            {
                labelStatus.Text = "NoID Password Manager not working.  Password could not be saved in the Windows keystore.";
            }
        }

        private void buttonTestAuthentication_Click(object sender, EventArgs e)
        {
            string username = textBoxUserName.Text;
            string pwd = PasswordManager.GetPassword(textBoxUserName.Text);
            if (pwd.Length == 0)
            {
                labelStatus.Text = "Password in keystore is blank.";
                return;
            }

            Authentication auth = new Authentication(username, pwd);
            Uri endpoint = new Uri(StringUtilities.RemoveTrailingBackSlash(System.Configuration.ConfigurationManager.AppSettings["AddNewPatientUri"].ToString()));

            Patient newPatient = FHIRUtilities.CreateTestFHIRPatientProfile();
            WebSend ws = new WebSend(endpoint, auth, newPatient);

            try
            {
                ws.PostHttpWebRequest();
                PasswordManager.SavePassword("Successful", username + "_Status");
                labelStatus.Text = "Status: Authentication Successful";
            }
            catch (Exception ex)
            {
                PasswordManager.SavePassword("Failed", username + "_Status");
                labelStatus.Text = "Status: Authentication Failed " + ex.Message;
            }
        }
    }
}
