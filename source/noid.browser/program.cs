// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.
// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Windows.Forms;
using Hl7.Fhir.Model;
using CefSharp;
using NoID.Security;
using NoID.Utilities;
using NoID.Network.Client;

namespace NoID.Browser
{
    public class Program
    {
        private static readonly string _serviceName = System.Configuration.ConfigurationManager.AppSettings["NoIDServiceName"].ToString();

        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Utilities.Auth = new Authentication(_serviceName, args[0].ToString());
                Uri endpoint = new Uri(StringUtilities.RemoveTrailingBackSlash(System.Configuration.ConfigurationManager.AppSettings["AddNewPatientUri"].ToString()));

                Patient newPatient = FHIRUtilities.CreateTestFHIRPatientProfile();
                WebSend ws = new WebSend(endpoint, Utilities.Auth, newPatient);
                try
                {
                    ws.PostHttpWebRequest();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Status: Authentication Failed " + ex.Message + " Closing NoID.");
                    return;
                }
            }

            if (Utilities.Auth != null || PasswordManager.GetPassword(_serviceName + "_Status") == "Successful")
            {
                //For Windows 7 and above, best to include relevant app.manifest entries as well
                Cef.EnableHighDPISupport();

                //Perform dependency check to make sure all relevant resources are in our output directory.
                Cef.Initialize(new CefSettings(), performDependencyCheck: true, browserProcessHandler: null);

                var browser = new BrowserForm();
                Application.Run(browser);
            }
            else
            {
                var login = new LoginForm();
                Application.Run(login);
            }
        }
    }
}