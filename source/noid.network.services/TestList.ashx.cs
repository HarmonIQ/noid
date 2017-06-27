// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.IO;
using System.Web;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using NoID.Utilities;

namespace NoID.Network.Services
{
    /// <summary>
    /// Tests receiving and processing FHIR Patient resource 
    /// with media embedded in the photo array
    /// </summary>
    public class TestList : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string responseText = null;
            context.Response.ContentType = "text/plain";

            if (!(context.Request.InputStream == null))
            {
                try
                {
                    Patient newResource = (Patient)FHIRUtilities.StreamToFHIR(new StreamReader(context.Request.InputStream));

                    if (newResource.Photo.Count > 0)
                    {
                        Attachment mediaAttachment = newResource.Photo[0];
                        byte[] byteMinutias = mediaAttachment.Data;

                        Stream stream = new MemoryStream(byteMinutias);
                        Media media = (Media)FHIRUtilities.StreamToFHIR(new StreamReader(stream));

                    }
                    responseText = "Parsed List object.";
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