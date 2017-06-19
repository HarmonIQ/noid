// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Web;
using System.IO;
using Hl7.Fhir.Model;
using NoID.FHIR.Profile;
using Hl7.Fhir.Rest;
using System.Web.Configuration;

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
        private string _responseText;
        private Base _baseFHIR = null;
        private Patient _patient = null;
        private Media _media = null;
        private Uri _sparkEndpoint = new Uri(WebConfigurationManager.AppSettings["SparkEndpointAddress"]);

        public FHIRMessageRouter(HttpContext context)
        {
            try
            {
                Resource newResource = FHIRMessageConverter.StreamToFHIR(new StreamReader(context.Request.InputStream));

                switch (newResource.TypeName.ToLower())
                {
                    case "patient":
                        _patient = (Patient)newResource;
                        if (!(SendPatientToSparkServer()))
                            return;

                        _responseText = "I received a patient and saved the message in Spark";
                        break;
                    case "media":
                        //TODO save FHIR message in Spark
                        //TODO send to biometric match engine.
                        _media = (Media)newResource;
                        _responseText = "I got a fingerprint.";
                        break;
                    default:
                        _responseText = FHIRType + " not supported.";
                        break;
                }
            }
            catch (Exception ex)
            {
                _responseText = "Error processing FHIR message: " + ex.Message;
            }
        }

        public bool SendPatientToSparkServer()
        {
            FhirClient client = new FhirClient(_sparkEndpoint);

            if (!(Patient == null))
            {
                try
                {
                    client.Create(Patient);
                }
                catch (Exception ex)
                {
                    _responseText = ex.Message;
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

        public Media Media
        {
            get { return _media; }
            private set { _media = value; }
        }

        public Base BaseFHIR
        {
            get { return _baseFHIR; }
            private set { _baseFHIR = value; }
        }

        public string FHIRType
        {
            get
            {
                string fhirType;
                if (!(_baseFHIR == null))
                {
                    fhirType = _baseFHIR.TypeName;
                }
                else
                {
                    fhirType = "Nothing";
                }
                return fhirType;
            }
        }

        public string ResponseText
        {
            get { return _responseText; }
        }
    }
}