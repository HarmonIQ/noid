// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Web;
using System.IO;
using System.Text;
using System.Web.Configuration;
using SourceAFIS.Templates;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using NoID.FHIR.Profile;
using NoID.Utilities;
using NoID.Match.Database.Client;
using NoID.Match.Database.FingerPrint;
using NoID.Database.Wrappers;

namespace NoID.Network.Services
{
    /// <summary>
    /// Class used to search for fingerprint matches.
    /// </summary>
    public class SearchBiometrics : IHttpHandler
    {
        private Uri _sparkEndpoint = new Uri(WebConfigurationManager.AppSettings["SparkEndpointAddress"]);
        private string _databaseDirectory = WebConfigurationManager.AppSettings["DatabaseLocation"];
        private string _backupDatabaseDirectory = WebConfigurationManager.AppSettings["BackupLocation"];
        private static readonly string NoIDMongoDBAddress = WebConfigurationManager.AppSettings["NoIDMongoDBAddress"].ToString();
        private static readonly string SparkMongoDBAddress = WebConfigurationManager.AppSettings["SparkMongoDBAddress"].ToString();
        private FingerPrintMatchDatabase dbMinutia;
        private string _responseText;
        private Exception _exception;
        private Media _biometics = null;

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                Resource newResource = FHIRUtilities.StreamToFHIR(new StreamReader(context.Request.InputStream));
                _biometics = (Media)newResource;
                // TODO send to biometric match engine. If found, add patient reference to FHIR message.
                // convert FHIR fingerprint message (_biometics) to AFIS template class
                Template probe = ConvertFHIR.FHIRToTemplate(_biometics);
                dbMinutia = new FingerPrintMatchDatabase(_databaseDirectory, _backupDatabaseDirectory);
                try
                {
                    dbMinutia.LateralityCode = (FHIRUtilities.LateralitySnoMedCode)probe.NoID.LateralitySnoMedCode;
                    dbMinutia.CaptureSite = (FHIRUtilities.CaptureSiteSnoMedCode)probe.NoID.CaptureSiteSnoMedCode;
                }
                catch { }
                MinutiaResult minutiaResult = dbMinutia.SearchPatients(probe);
                if (minutiaResult != null)
                {
                    if (minutiaResult.NoID != null && minutiaResult.NoID.Length > 0)
                    {
                        // Fingerprint found in database
                        _responseText = minutiaResult.NoID;  //TODO: for now, it returns the localNoID.  should return a FHIR response.
                    }
                    else
                    {
                        _responseText = "No local database match.";
                    }
                }
                else
                {
                    _responseText = "No local database match.";
                }
                dbMinutia.Dispose();
            }
            catch(Exception ex)
            {
                _exception = ex;
                _responseText = ex.Message;
            }
            context.Response.Write(_responseText);
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