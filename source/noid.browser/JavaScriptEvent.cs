using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
