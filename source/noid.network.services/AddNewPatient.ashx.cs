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
    /// Web service to add a new patient.
    /// </summary>
    public class AddNewPatient : IHttpHandler
    {
        private Uri _sparkEndpoint = new Uri(WebConfigurationManager.AppSettings["SparkEndpointAddress"]);
        private string _databaseDirectory = WebConfigurationManager.AppSettings["DatabaseLocation"];
        private string _backupDatabaseDirectory = WebConfigurationManager.AppSettings["BackupLocation"];
        private static readonly string NoIDMongoDBAddress = WebConfigurationManager.AppSettings["NoIDMongoDBAddress"].ToString();
        private static readonly string SparkMongoDBAddress = WebConfigurationManager.AppSettings["SparkMongoDBAddress"].ToString();
        private FingerPrintMatchDatabase dbMinutia;
        private string _responseText;
        private Patient _patient = null;
        private Exception _exception;

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                Resource newResource = FHIRUtilities.StreamToFHIR(new StreamReader(context.Request.InputStream));
                _patient = (Patient)newResource;
                //TODO: make sure this FHIR message has a new pending status.
                
                //TODO: make this an atomic transaction.  
                //          delete the FHIR message from Spark if there is an error in the minutia.
                Patient ptSaved = (Patient)SendPatientToSparkServer();
                if (ptSaved == null)
                {
                    _responseText = "Error sending Patient FHIR message to the Spark FHIR endpoint. " + ExceptionString;
                    return;
                }
                SessionQueue seq = PatientToSessionQueue(_patient, ptSaved.Id.ToString());
                if (_patient.Photo.Count > 0)
                {
                    dbMinutia = new FingerPrintMatchDatabase(_databaseDirectory, _backupDatabaseDirectory);
                    foreach (var minutia in _patient.Photo)
                    {
                        byte[] byteMinutias = minutia.Data;
                        Stream stream = new MemoryStream(byteMinutias);
                        Media media = (Media)FHIRUtilities.StreamToFHIR(new StreamReader(stream));
                        // Save minutias for matching.
                        Template fingerprintTemplate = ConvertFHIR.FHIRToTemplate(media);
                        fingerprintTemplate.NoID.LocalNoID = seq.LocalReference;
                        try
                        {
                            dbMinutia.LateralityCode = (FHIRUtilities.LateralitySnoMedCode)fingerprintTemplate.NoID.LateralitySnoMedCode;
                            dbMinutia.CaptureSite = (FHIRUtilities.CaptureSiteSnoMedCode)fingerprintTemplate.NoID.CaptureSiteSnoMedCode;
                        }
                        catch { }
                        if (dbMinutia.AddTemplate(fingerprintTemplate) == false)
                        {
                            _responseText = "Error adding a fingerprint to the match database.";
                        }
                    }
                    dbMinutia.Dispose();

                    MongoDBWrapper dbwrapper = new MongoDBWrapper(NoIDMongoDBAddress, SparkMongoDBAddress);
                    dbwrapper.AddPendingPatient(seq);
                    //TODO: end atomic transaction.  
                }
                _responseText = "Successful.";
            }
            catch (Exception ex)
            {
                _responseText = "Error in FHIRMessageRouter::FHIRMessageRouter: " + ex.Message;
            }
        }

        SessionQueue PatientToSessionQueue(Patient pt, string SparkReference)
        {
            SessionQueue seq = null;
            try
            {
                if (pt != null)
                {
                    seq = new SessionQueue();
                    if (pt.Identifier.Count > 0)
                    {
                        foreach (Identifier id in _patient.Identifier)
                        {
                            if (id.System.ToString().ToLower().Contains("session") == true)
                            {
                                seq._id = id.Value.ToString();
                            }
                            else if (id.System.ToString().ToLower().Contains("remote") == true)
                            {
                                seq.RemoteHubReference = id.Value.ToString();
                            }
                            else if (id.System.ToString().ToLower().Contains("local") == true)
                            {
                                seq.LocalReference = id.Value.ToString();
                            }
                        }
                        //TODO get last update datetime
                        seq.SparkReference = SparkReference;
                        seq.PatientStatus = "new";
                        seq.ApprovalStatus = "pending";
                        seq.SessionComputerName = ""; //TODO: get from browser patient object.  browser needs to add it.
                        seq.ClinicArea = ""; //TODO: get from browser patient object.  browser needs to add it.
                        if (seq.LocalReference == "")
                        {
                            seq.LocalReference = StringUtilities.SHA256(seq._id); // LocalReference is always a hash of the first patient SessionID
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
            return seq;
        }

        Resource SendPatientToSparkServer()
        {
            FhirClient client = new FhirClient(_sparkEndpoint);

            Resource response = null;
            if (!(Patient == null))
            {
                try
                {
                    response = client.Create(Patient);
                }
                catch (Exception ex)
                {
                    _responseText = "Error in FHIRMessageRouter::SendPatientToSparkServer. " + ex.Message;
                    _exception = ex;
                }
            }
            return response;
        }

        Patient Patient
        {
            get { return _patient; }
            set { _patient = value; }
        }

        public string ExceptionString
        {
            get
            {
                if (!(_exception == null))
                {
                    return _exception.Message;
                }
                else
                {
                    return "";
                }
            }
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