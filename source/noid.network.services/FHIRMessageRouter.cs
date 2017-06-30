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


namespace NoID.Network.Services
{
    //Workflow
    //1) Save message in Spark FHIR for persisting.
    //2) If biometic probe message (FHIR Media type), route to biometic match engine router
    //3) If patient message (FHIR patient type), add the patient message and expire any previous records. 
    //      all NoID patient resources must have a session id, local healthcare node id and hub id

    public class FHIRMessageRouter
    {
        private PatientFHIRProfile _patientFHIRProfile;
        private FingerPrintMatchDatabase dbMinutia;
        private string _responseText;
        private Patient _patient = null;
        private Media _biometics = null;
        private Uri _sparkEndpoint = new Uri(WebConfigurationManager.AppSettings["SparkEndpointAddress"]);
        private string _databaseDirectory = WebConfigurationManager.AppSettings["DatabaseLocation"];
        private string _backupDatabaseDirectory = WebConfigurationManager.AppSettings["BackupLocation"];
        private Exception _exception;

        public FHIRMessageRouter(HttpContext context)
        {
            try
            {
                Resource newResource = FHIRUtilities.StreamToFHIR(new StreamReader(context.Request.InputStream));
                switch (newResource.TypeName.ToLower())
                {
                    case "patient":
                        //if new patient. TODO: check meta for NoID status

                        _patient = (Patient)newResource;
                        string sessionID = "";
                        if (_patient.Identifier.Count > 0)
                        {
                            foreach(Identifier id in _patient.Identifier)
                            {
                                if (id.System.ToString().ToLower().Contains("session") == true)
                                {
                                    sessionID = id.Value.ToString();
                                }
                            }
                        }

                        Patient ptSaved = (Patient)SendPatientToSparkServer();
                        if (ptSaved == null)
                        {
                            _responseText = "Error sending Patient FHIR message to the Spark FHIR endpoint. " + ExceptionString;
                            return;
                        }

                        string LocalNoID = ptSaved.Id.ToString();
                        //TODO: make this an atomic transaction.  
                        //          delete the FHIR message from Spark if there is an error in the minutia.

                        //TODO check for existing patient and expire old messages for the patient.
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
                                fingerprintTemplate.NoID.LocalNoID = LocalNoID;
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
                            _responseText = "Successful.";
                        }
                        break;
                    case "media":
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

                        if (!(SendBiometicsToSparkServer()))
                        {
                            _responseText = "Error sending Biometric Media FHIR message to the Spark FHIR endpoint. " + ExceptionString;
                        }
                        break;
                    default:
                        _responseText = newResource.TypeName.ToLower() + " not supported.";
                        break;
                }
            }
            catch (Exception ex)
            {
                _responseText = "Error in FHIRMessageRouter::FHIRMessageRouter: " + ex.Message;
            }
        }

        public Resource SendPatientToSparkServer()
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

        public bool SendBiometicsToSparkServer()
        {
            FhirClient client = new FhirClient(_sparkEndpoint);

            if (!(Patient == null))
            {
                try
                {
                    Resource response = client.Create(Biometics);
                    _responseText = FHIRUtilities.FHIRToString(response);
                }
                catch (Exception ex)
                {
                    _responseText = ex.Message;
                    _exception = ex;
                    return false;
                }
            }
            return true;
        }

        public PatientFHIRProfile PatientFHIRProfile
        {
            get { return _patientFHIRProfile; }
            private set { _patientFHIRProfile = value; }
        }

        public Uri SparkEndpoint
        {
            get { return _sparkEndpoint; }
            private set { _sparkEndpoint = value; }
        }

        public Patient Patient
        {
            get { return _patient; }
            private set { _patient = value; }
        }

        public Media Biometics
        {
            get { return _biometics; }
            private set { _biometics = value; }
        }

        public string ResponseText
        {
            get { return _responseText; }
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
    }
}