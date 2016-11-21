﻿// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Windows.Forms;
using CefSharp;

namespace NoID.Browser
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            //For Windows 7 and above, best to include relevant app.manifest entries as well
            Cef.EnableHighDPISupport();

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(new CefSettings(), performDependencyCheck: true, browserProcessHandler: null);

            var browser = new BrowserForm();
            Application.Run(browser);
        }
    }
}
