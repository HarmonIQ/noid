// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.
// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Windows.Forms;
using CefSharp;
using NoID.Security;

namespace NoID.Browser
{
    public class Program
    {
        private static readonly string _serviceName = System.Configuration.ConfigurationManager.AppSettings["NoIDServiceName"].ToString();

        [STAThread]
        public static void Main()
        {
            if (PasswordManager.GetPassword(_serviceName + "_Status") == "Successful")
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
