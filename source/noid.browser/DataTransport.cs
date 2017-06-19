// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using Hl7.Fhir.Model;
using NoID.Network.Client;
using NoID.Security;

namespace NoID.Browser
{
    public class DataTransport
    {

        private Exception _exception;

        public bool SendFHIRPatientProfile(Uri endpoint, Authentication auth, Patient patient = null)
        {
            try
            {
                WebSend clientWebSend = new WebSend(endpoint, auth, patient);
                string httpResponse = clientWebSend.PostHttpWebRequest();
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
                WebSend clientWebSend = new WebSend(endpoint, auth, media);
                string httpResponse = clientWebSend.PostHttpWebRequest();
            }
            catch (Exception ex)
            {
                _exception = new Exception("DataTransport.SendFHIRMediaProfile() failed to send to FHIR server: " + ex.Message);
                return false;
            }
            return true;
        }
    }
}
