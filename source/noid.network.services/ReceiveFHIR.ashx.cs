// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Web;

namespace NoID.Network.Services
{
    /// <summary>
    /// Used to receive, process (route), and send a reponse to the NoID client
    /// </summary>
    public class ReceiveFHIR : IHttpHandler
    {
        private FHIRMessageRouter messageRouter;

        public void ProcessRequest(HttpContext context)
        {
            string responseText = null;
            //context.Response.ContentType = "application/json+fhir";
            context.Response.ContentType = "text/plain";
            if (!(context.Request.InputStream == null))
            {
                try
                {
                    messageRouter = new FHIRMessageRouter(context);
                    responseText = messageRouter.ResponseText;
                }
                catch (Exception ex)
                {
                    //TODO: return FHIR or JSON formated error here instead of plain text.  manage errors with a class.
                    responseText = "NoID ReceiveFHIR Server Error 35000. " + ex.Message;
                }
            }
            else
            {
                //TODO: return FHIR or JSON formated error here instead of plain text.  manage errors with a class.
                responseText = "NoID ReceiveFHIR Server Error 35001.  FHIR message not found in HTTP body.";
            }
            context.Response.Write(responseText);
            context.Response.End();
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