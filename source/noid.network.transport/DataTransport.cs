// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Hl7.Fhir.Model;
using NoID.Network.Client;
using NoID.Security;
using NoID.FHIR.Profile;

namespace NoID.Network.Transport
{
    public class HttpsClient
    {
        //TODO: only allow https
        private Exception _exception;
        private string _responseText;

        public IList<PatientProfile> RequestPendingQueue(Uri endpoint, Authentication auth, Patient patient = null)
        {
            IList<PatientProfile> pendingProfiles = null;
            try
            {
                string pendingPatientJSON = null;
                WebSend clientWebSend = new WebSend(endpoint, auth);
                pendingPatientJSON = clientWebSend.GetPatientList("pending");
                pendingProfiles = JsonConvert.DeserializeObject<IList<PatientProfile>>(pendingPatientJSON);
            }
            catch (Exception ex)
            {
                _responseText = "DataTransport::RequestPendingQueue() failed to get pending patients: " + ex.Message;
                _exception = new Exception(_responseText);
            }
            return pendingProfiles;
        }

        public bool SendFHIRPatientProfile(Uri endpoint, Authentication auth, Patient patient = null)
        {
            try
            {
                _responseText = "";
                WebSend clientWebSend = new WebSend(endpoint, auth, patient);
                _responseText = clientWebSend.PostHttpWebRequest();
            }
            catch (Exception ex)
            {
                _exception = new Exception("DataTransport.SendFHIRPatientProfile() failed to send to FHIR server: " + ex.Message);
                return false;
            }
            return true;
        }

        public bool SendFHIRMediaProfile(Uri endpoint, Authentication auth, Media media = null)
        {
            try
            {
                _responseText = "";
                WebSend clientWebSend = new WebSend(endpoint, auth, media);
                _responseText = clientWebSend.PostHttpWebRequest();
            }
            catch (Exception ex)
            {
                _responseText = "Error in DataTransport.SendFHIRMediaProfile() failed to send to FHIR server: " + ex.Message;
                _exception = new Exception(_responseText);
                return false;
            }
            return true;
        }

        public string ResponseText
        {
            get { return _responseText; }
        }
    }
}
