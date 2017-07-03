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
        private static readonly Uri _sparkEndpoint = new Uri(WebConfigurationManager.AppSettings["SparkEndpointAddress"]);
        private static readonly string DatabaseDirectory = WebConfigurationManager.AppSettings["DatabaseLocation"];
        private static readonly string BackupDatabaseDirectory = WebConfigurationManager.AppSettings["BackupLocation"];
        private static readonly string NoIDMongoDBAddress = WebConfigurationManager.AppSettings["NoIDMongoDBAddress"].ToString();
        private static readonly string SparkMongoDBAddress = WebConfigurationManager.AppSettings["SparkMongoDBAddress"].ToString();
        private static readonly string OrganizationName = WebConfigurationManager.AppSettings["OrganizationName"].ToString();
        private static readonly string DomainName = WebConfigurationManager.AppSettings["DomainName"].ToString();
        private static readonly string NodeSalt = WebConfigurationManager.AppSettings["NodeSalt"].ToString();
        
        private FingerPrintMatchDatabase dbMinutia;
        private string _responseText;
        private Patient _patient = null;
        private Exception _exception;

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                Stream httpStream = context.Request.InputStream;
                StreamReader httpStreamReader = new StreamReader(httpStream);
                string jsonFHIRMessage = FHIRUtilities.StreamToFHIRString(httpStreamReader);
                
                //reset the stream to the begining
                httpStream.Position = 0;
                httpStreamReader.DiscardBufferedData();
                Resource newResource = FHIRUtilities.StreamToFHIR(httpStreamReader);

                _patient = (Patient)newResource;
                //TODO: make sure this FHIR message has a new pending status.
                
                //TODO: make this an atomic transaction.  
                //          delete the FHIR message from Spark if there is an error in the minutia.
                Patient ptSaved = (Patient)SendPatientToSparkServer();
                //LogUtilities.LogEvent("AddNewPatient.ashx Saved FHIR in spark.");
                if (ptSaved == null)
                {
                    _responseText = "Error sending Patient FHIR message to the Spark FHIR endpoint. " + ExceptionString;
                    return;
                }

                SourceAFIS.Templates.NoID noID = new SourceAFIS.Templates.NoID();
                noID.SessionID = ptSaved.Id.ToString();
                //TODO: Add Argon2d hash here
                noID.LocalNoID = "noid://" + DomainName + "/" + StringUtilities.SHA256(DomainName + noID.SessionID + NodeSalt);
                SessionQueue seq = PatientToSessionQueue(_patient, ptSaved.Id.ToString(), noID.LocalNoID);
                //TODO: send to selected match hub and get the remote hub ID.
                // Hub ID in the same format: noid://domain/LocalID
                if (_patient.Photo.Count > 0)
                {
                    dbMinutia = new FingerPrintMatchDatabase(DatabaseDirectory, BackupDatabaseDirectory);
                    foreach (var minutia in _patient.Photo)
                    {
                        byte[] byteMinutias = minutia.Data;
                        Stream stream = new MemoryStream(byteMinutias);
                        Media media = (Media)FHIRUtilities.StreamToFHIR(new StreamReader(stream));
                        // Save minutias for matching.
                        Template fingerprintTemplate = ConvertFHIR.FHIRToTemplate(media);
                        fingerprintTemplate.NoID = noID;
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
                    dbwrapper.AddPendingPatient(seq, jsonFHIRMessage);
                    //TODO: end atomic transaction.  
                }
                _responseText = "Successful.";
                LogUtilities.LogEvent("Ending AddNewPatient.ashx");
            }
            catch (Exception ex)
            {
                _responseText = "Error in FHIRMessageRouter::FHIRMessageRouter: " + ex.Message;
                LogUtilities.LogEvent(_responseText);
            }
        }

        SessionQueue PatientToSessionQueue(Patient pt, string sparkReference, string localNoID)
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
                        }
                    }
                    if (seq._id == null)
                    {
                        seq._id = sparkReference;
                    }
                    if (seq._id.Length == 0)
                    {
                        seq._id = sparkReference;
                    }
                    seq.LocalReference = localNoID;
                    seq.SparkReference = sparkReference;
                    seq.PatientStatus = "new";
                    seq.ApprovalStatus = "pending";
                    seq.SubmitDate = DateTime.UtcNow;
                    //TODO get PatientBeginDate from FHIR message
                    //seq.PatientBeginDate = DateTime.UtcNow;
                    seq.SessionComputerName = ""; //TODO: get from browser patient object.  browser needs to add it.
                    seq.ClinicArea = ""; //TODO: get from browser patient object.  browser needs to add it.
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