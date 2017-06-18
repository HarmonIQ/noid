// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System.Web;

namespace NoID.Network.Services
{
    /// <summary>
    /// Summary description for ReceiveFHIR
    /// </summary>
    public class ReceiveFHIR : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Total Bytes Received: " + context.Request.TotalBytes.ToString());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}