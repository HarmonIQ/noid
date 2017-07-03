// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Net;
using System.IO;
using System.Web;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Utility;
using Hl7.Fhir.Serialization;
using NoID.Security;

namespace NoID.Network.Client
{
    /// <summary>Sends NoID Message from the client to a NoID webservice
    /// </summary>
    
    public class WebSend
    {
        private Uri _enpoint = null;
        private Authentication _auth = null;
        private Resource _payloadJSON = null;
        //private PatientFHIRProfile _payloadProtoBuff;

        public delegate void EventHandler(object sender, EventArgs args);

        public event EventHandler BeforeHttpRequest = delegate { };
        public event EventHandler AfterHttpResponse = delegate { };
        
        public WebSend(Uri endpoint, Authentication auth, Resource payload)
        {
            System.Diagnostics.Debug.WriteLine("WebSend Init: {0}: {1}", endpoint.ToString(), payload.ToString());
            _enpoint = endpoint;
            _auth = auth;
            _payloadJSON = payload;
        }
        public WebSend(Uri endpoint, Authentication auth)
        {
            System.Diagnostics.Debug.WriteLine("WebSend Init: {0}", endpoint.ToString());
            _enpoint = endpoint;
            _auth = auth;
        }

        public string PostHttpWebRequest()
        {
            string html = string.Empty;
            try
            {
                byte[] output = null;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_enpoint);
                request.Method = "POST";
                //TODO: test compression
                //request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Headers.Add("Authorization", "Basic " + _auth.BasicAuthentication);
                if (!(_payloadJSON is null))
                {
                    SetBodyAndContentType(request, _payloadJSON, ResourceFormat.Json, true, out output);
                }
                 
                request.GetRequestStream().Write(output, 0, output.Length);
                // call BeforeHttpRequest event
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
                // call AfterHttpResponse event
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return html;
        }

        public string SavePendingStatus(string sessionID, string action, string computerName, string userName)
        {
            string jsonResponse = null;
            try
            {
                string UriWithQueryString = _enpoint.ToString() 
                    + "?sessionid=" + HttpUtility.UrlEncode(sessionID)
                    + "&action=" + HttpUtility.UrlEncode(action)
                    + "&computername=" + HttpUtility.UrlEncode(computerName) 
                    + "&username=" + HttpUtility.UrlEncode(userName);
                Uri uriQueryString = new Uri(UriWithQueryString);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uriQueryString);
                request.Method = "GET";
                //TODO: test compression
                //request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Headers.Add("Authorization", "Basic " + _auth.BasicAuthentication);
                // call BeforeHttpRequest event
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    jsonResponse = reader.ReadToEnd();
                }
                // call AfterHttpResponse event
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return jsonResponse;
        }

        public string GetPatientList(string listType)
        {
            string jsonResponse = null;
            try
            {
                string UriWithQueryString = _enpoint.ToString() + "?type=" + listType; //types = pending, approved, denied, or holding
                Uri uriQueryString = new Uri(UriWithQueryString);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uriQueryString);
                request.Method = "GET";
                //TODO: test compression
                //request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Headers.Add("Authorization", "Basic " + _auth.BasicAuthentication);
                // call BeforeHttpRequest event
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    jsonResponse = reader.ReadToEnd();
                }
                // call AfterHttpResponse event
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return jsonResponse;
        }

        private static void SetBodyAndContentType(HttpWebRequest request, Resource payload, ResourceFormat format, bool CompressRequestBody, out byte[] body)
        {
            if (payload == null) throw Error.ArgumentNull(nameof(payload));

            if (payload is Binary)
            {
                var bin = (Binary)payload;
                body = bin.Content;
                request.ContentType = bin.ContentType;
            }
            else
            {
                body = format == ResourceFormat.Xml ?
                    FhirSerializer.SerializeToXmlBytes(payload, summary: Hl7.Fhir.Rest.SummaryType.False) :
                    FhirSerializer.SerializeToJsonBytes(payload, summary: Hl7.Fhir.Rest.SummaryType.False);

                request.ContentType = Hl7.Fhir.Rest.ContentType.BuildContentType(format, forBundle: false);
            }
        }

        /*
        private static void SetBodyAndContentType(HttpWebRequest request, byte[] payload, out byte[] body)
        {
            if (payload == null) throw Error.ArgumentNull(nameof(payload));

            var bin = (byte[])payload;
            body = payload;
            request.ContentType = "Binary";
        }
        */
    }
}
