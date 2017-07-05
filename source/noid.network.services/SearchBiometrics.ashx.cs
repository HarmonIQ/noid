// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Web;
using System.IO;
using System.Web.Configuration;
using SourceAFIS.Templates;
using Hl7.Fhir.Model;
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
        private readonly string MinimumAcceptedMatchScore = WebConfigurationManager.AppSettings["MinimumAcceptedMatchScore"];
        private uint _minimumAcceptedMatchScore;
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
                if (uint.TryParse(MinimumAcceptedMatchScore, out _minimumAcceptedMatchScore) == false)
                {
                    _minimumAcceptedMatchScore = 30;
                }
                Resource newResource = FHIRUtilities.StreamToFHIR(new StreamReader(context.Request.InputStream));
                _biometics = (Media)newResource;
                // TODO send to biometric match engine. If found, add patient reference to FHIR message.
                // convert FHIR fingerprint message (_biometics) to AFIS template class
                Template probe = ConvertFHIR.FHIRToTemplate(_biometics);
                dbMinutia = new FingerPrintMatchDatabase(_databaseDirectory, _backupDatabaseDirectory, _minimumAcceptedMatchScore);
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
                        // check if patient is already pending.
                        MongoDBWrapper dbwrapper = new MongoDBWrapper(NoIDMongoDBAddress, SparkMongoDBAddress);
                        string currentStatus = dbwrapper.GetCurrentStatus(minutiaResult.NoID);
                        if (currentStatus.ToLower() != "pending")
                        {
                            _responseText = minutiaResult.NoID;  //TODO: for now, it returns the localNoID.  should return a FHIR response.
                        }
                        else
                        {
                            _responseText = "pending";
                        }
                        LogUtilities.LogEvent(_responseText);
                    }
                    else
                    {
                        _responseText = "No local database match.";
                        LogUtilities.LogEvent(_responseText);
                    }
                }
                else
                {
                    _responseText = "No local database match.";
                    LogUtilities.LogEvent(_responseText);
                }
                dbMinutia.Dispose();
                LogUtilities.LogEvent("After dbMinutia.Dispose();");
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