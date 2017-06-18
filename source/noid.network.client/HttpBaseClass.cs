using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace NoID.Network.Client
{
    public class HttpBaseClass
    {

        private string UserName;
        private string UserPwd;
        private string ProxyServer;
        private int ProxyPort;
        private string Request;

        public HttpBaseClass(string HttpUserName,
          string HttpUserPwd, string HttpProxyServer,
          int HttpProxyPort, string HttpRequest)
        {
            UserName = HttpUserName;
            UserPwd = HttpUserPwd;
            ProxyServer = HttpProxyServer;
            ProxyPort = HttpProxyPort;
            Request = HttpRequest;
        }

        /// <summary>
        /// This method creates secure/non secure web
        /// request based on the parameters passed.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="collHeader">This parameter of type
        ///    NameValueCollection may contain any extra header
        ///    elements to be included in this request      </param>
        /// <param name="RequestMethod">Value can POST OR GET</param>
        /// <param name="NwCred">In case of secure request this would be true</param>
        /// <returns></returns>
        public virtual HttpWebRequest CreateWebRequest(string uri, NameValueCollection collHeader, string RequestMethod, bool NwCred)
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(uri);
            webrequest.KeepAlive = false;
            webrequest.Method = RequestMethod;

            int iCount = collHeader.Count;
            string key;
            string keyvalue;

            for (int i = 0; i < iCount; i++)
            {
                key = collHeader.Keys[i];
                keyvalue = collHeader[i];
                webrequest.Headers.Add(key, keyvalue);
            }

            webrequest.ContentType = "text/html";
            //"application/x-www-form-urlencoded";

            if (ProxyServer.Length > 0)
            {
                webrequest.Proxy = new
                 WebProxy(ProxyServer, ProxyPort);
            }
            webrequest.AllowAutoRedirect = false;

            if (NwCred)
            {
                CredentialCache wrCache = new CredentialCache();
                wrCache.Add(new Uri(uri), "Basic", new NetworkCredential(UserName, UserPwd));
                webrequest.Credentials = wrCache;
            }
            //Remove collection elements
            collHeader.Clear();
            return webrequest;
        }

        /// <summary>
        /// This method retreives redirected URL from
        /// response header and also passes back
        /// any cookie (if there is any)
        /// </summary>
        /// <param name="webresponse"></param>
        /// <param name="Cookie"></param>
        /// <returns></returns>
        public virtual string GetRedirectURL(HttpWebResponse
             webresponse, ref string Cookie)
        {
            string uri = "";

            WebHeaderCollection headers = webresponse.Headers;

            if ((webresponse.StatusCode == HttpStatusCode.Found) ||
              (webresponse.StatusCode == HttpStatusCode.Redirect) ||
              (webresponse.StatusCode == HttpStatusCode.Moved) ||
              (webresponse.StatusCode == HttpStatusCode.MovedPermanently))
            {
                // Get redirected uri
                uri = headers["Location"];
                uri = uri.Trim();
            }

            //Check for any cookies
            if (headers["Set-Cookie"] != null)
            {
                Cookie = headers["Set-Cookie"];
            }
            //                string StartURI = "http:/";
            //                if (uri.Length > 0 && uri.StartsWith(StartURI)==false)
            //                {
            //                      uri = StartURI + uri;
            //                }
            return uri;
        }//End of GetRedirectURL method


        public virtual string GetFinalResponse(string ReUri,
         string Cookie, string RequestMethod, bool NwCred)
        {
            NameValueCollection collHeader =
                  new NameValueCollection();

            if (Cookie.Length > 0)
            {
                collHeader.Add("Cookie", Cookie);
            }

            HttpWebRequest webrequest =
              CreateWebRequest(ReUri, collHeader,
              RequestMethod, NwCred);

            BuildReqStream(ref webrequest);

            HttpWebResponse webresponse;

            webresponse = (HttpWebResponse)webrequest.GetResponse();

            Encoding enc = System.Text.Encoding.GetEncoding(1252);
            StreamReader loResponseStream = new
              StreamReader(webresponse.GetResponseStream(), enc);

            string Response = loResponseStream.ReadToEnd();

            loResponseStream.Close();
            webresponse.Close();

            return Response;
        }

        private void BuildReqStream(ref HttpWebRequest webrequest)
        //This method build the request stream for WebRequest
        {
            byte[] bytes = Encoding.ASCII.GetBytes(Request);
            webrequest.ContentLength = bytes.Length;

            Stream oStreamOut = webrequest.GetRequestStream();
            oStreamOut.Write(bytes, 0, bytes.Length);
            oStreamOut.Close();
        }
    }
}
