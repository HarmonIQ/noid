// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using CefSharp;
using CefSharp.WinForms;

namespace NoID.Browser
{
    public class JavaScriptEvent
    {
        public string Sender; //sending function
        public EventArgs Args; //
    }

    public class BoundObject
    {
        private readonly ChromiumWebBrowser _browser;
        string _selectedItem;

        public BoundObject(ChromiumWebBrowser browser)
        {
            _browser = browser;
        }

        public void OnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Frame.IsMain)
            {
                _browser.ExecuteScriptAsync(@"
                                          document.body.onmouseup = function()
                                          {
                                            BoundObject.onSelected(window.document.activeElement.id.toString());
                                          }
                                          ");
            }
        }

        public void onSelected(string selected)
        {
            _selectedItem = selected;
        }

        public string SelectedItem
        {
            get { return _selectedItem; }
        }
    }
}
