﻿// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Web;
using System.Net.Http;
using System.IO;
using System.ComponentModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NoID.FHIR.Profile;


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

        public FHIRMessageRouter(HttpContext context)
        {
            try
            {
                StreamReader bodyStream = new StreamReader(context.Request.InputStream);
                bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                string jsonString = bodyStream.ReadToEnd();
                FhirJsonParser fhirJsonParser = new FhirJsonParser();
                Resource rs = fhirJsonParser.Parse<Resource>(jsonString);

                switch (rs.TypeName.ToLower())
                {
                    case "patient":
                        _patient = (Patient)rs;
                        _responseText = "I got a patient.";
                        break;
                    case "media":
                        _media = (Media)rs;
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

        public PatientFHIRProfile PatientFHIRProfile
        {
            get { return _patientFHIRProfile; }
            private set { _patientFHIRProfile = value; }
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