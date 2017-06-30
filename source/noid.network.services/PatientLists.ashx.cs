// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Web;
using System.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using NoID.FHIR.Profile;
using NoID.Utilities;

namespace NoID.Network.Services
{
    /// <summary>
    /// Web service that returns a list of pending patients profiles
    /// </summary>
    public class PatientLists : IHttpHandler
    {
        private readonly Uri sparkEndpointAddress = new Uri(StringUtilities.RemoveTrailingBackSlash(ConfigurationManager.AppSettings["SparkEndpointAddress"].ToString()));
        private readonly string organizationName = ConfigurationManager.AppSettings["OrganizationName"].ToString();
        private readonly string domainName = ConfigurationManager.AppSettings["DomainName"].ToString();
        private List<Exception> _exceptions = new List<Exception>();

        public void ProcessRequest(HttpContext context)
        {
            string jsonResponse = null;
            context.Response.ContentType = "application/json"; // streaming JSON text
            if (context.Request.QueryString.Count > 0)
            {
                string queryStringOne = context.Request.QueryString[0].ToLower();
                if (queryStringOne == "pending")
                {
                    IList<PatientProfile> _pendingPatients = GetPendingPatients();
                    jsonResponse = JsonConvert.SerializeObject(_pendingPatients); // Pending patient list
                }
                else if (queryStringOne == "approved")
                {
                    jsonResponse = JsonConvert.SerializeObject("Error. Approved patient list not implemented yet.");
                }
                else if (queryStringOne == "denied")
                {
                    jsonResponse = JsonConvert.SerializeObject("Error. Denied patient list not implemented yet.");
                }
                else if (queryStringOne == "hold")
                {
                    jsonResponse = JsonConvert.SerializeObject("Error. Hold patient list not implemented yet.");
                }
                else
                {
                    jsonResponse = JsonConvert.SerializeObject("Error. " + queryStringOne + " not implemented yet.");
                }
            }
            else
            {
                jsonResponse = JsonConvert.SerializeObject("Error. No query string.");
            }
            context.Response.Write(jsonResponse);
        }

        //TODO: Add
        private IList<PatientProfile> GetPendingPatients()
        {
            List<PatientProfile> listPending = new List<PatientProfile>();
            try
            {
                FhirClient client = new FhirClient(sparkEndpointAddress);
                string gtDateFormat = "gt" + FHIRUtilities.DateToFHIRString(DateTime.UtcNow.AddDays(-2));
                client.PreferredFormat = ResourceFormat.Json;
                Uri uriTwoDays = new Uri(sparkEndpointAddress.ToString() + "/Patient?_lastUpdated=" + gtDateFormat);
                Bundle patientBundle = (Bundle)client.Get(uriTwoDays);
                foreach (Bundle.EntryComponent entry in patientBundle.Entry)
                {
                    string ptURL = entry.FullUrl.ToString().Replace("http://localhost:49911/fhir", sparkEndpointAddress.ToString());
                    Patient pt = (Patient)client.Get(ptURL);
                    PatientProfile patientProfile = new PatientProfile(pt, false);
                    listPending.Add(patientProfile);
                }
            }
            catch(Exception ex)
            {
                _exceptions.Add(ex);
            }
            return listPending;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    static class TestPatientList
    {
        /// <summary>Creates a test patient list for the ProviderBridge class
        /// <seealso cref="ProviderBridge"/>
        /// </summary>

        public static IList<PatientProfile> GetTestPatients(string organizationName)
        {
            string addressPending = StringUtilities.RemoveTrailingBackSlash(System.Configuration.ConfigurationManager.AppSettings["SparkEndpointAddress"].ToString());
            Uri endPoint = new Uri(addressPending);

            List<PatientProfile> newList = new List<PatientProfile>();
            Patient testPatient = FHIRUtilities.CreateTestFHIRPatientProfile
                (
                organizationName, Guid.NewGuid().ToString(), "", "English", "1961-04-22", "F", "No",
                "Donna", "Marie", "Kasick", "4712 W 3rd St.", "Apt 35", "New York", "NY", "10000-2221",
                "212-555-3000", "212-555-7400", "212-555-9555", "donnakasick@yandex.com"
                );

            PatientProfile newProfile = new PatientProfile(organizationName, endPoint, testPatient, "New", DateTime.UtcNow.AddMinutes(-3));


            newList.Add(newProfile);

            testPatient = FHIRUtilities.CreateTestFHIRPatientProfile
                (
                organizationName, Guid.NewGuid().ToString(), "", "English", "1992-10-30", "F", "Yes",
                "Christine", "J", "Pinkentinfuter", "2088 N 23nd St.", "Unit 51", "New York", "NY", "10012-0159",
                "", "318-777-8888", "318-222-4111", "Christine@Pinkentinfuter.com"
                );

            newProfile = new PatientProfile(organizationName, endPoint, testPatient, "Return", DateTime.UtcNow.AddMinutes(-5));

            newList.Add(newProfile);

            testPatient = FHIRUtilities.CreateTestFHIRPatientProfile
                (
                organizationName, Guid.NewGuid().ToString(), "", "English", "1954-02-19", "M", "No",
                "Mitchel", "James", "Hendrichs", "2442 Bleaker St.", "Apt 722", "New York", "NY", "10503-04855",
                "212-111-1234", "", "", "jhendrichs@gtestmail.com"
                );

            newProfile = new PatientProfile(organizationName, endPoint, testPatient, "Return", DateTime.UtcNow.AddMinutes(-15));

            newList.Add(newProfile);

            testPatient = FHIRUtilities.CreateTestFHIRPatientProfile
                (
                organizationName, Guid.NewGuid().ToString(), "", "English", "2001-03-21", "M", "No",
                "Brian", "H", "O'Donald", "212 Cremont", "", "New Jersey", "NJ", "14235",
                "", "212-555-1234", "", "BrianODonald2001@yahoo.com"
                );

            newProfile = new PatientProfile(organizationName, endPoint, testPatient, "New", DateTime.UtcNow.AddMinutes(-32));

            newList.Add(newProfile);

            testPatient = FHIRUtilities.CreateTestFHIRPatientProfile
                (
                organizationName, Guid.NewGuid().ToString(), "", "English", "1999-09-02", "F", "No",
                "Jonie", "M", "Smith", "", "", "", "", "",
                "", "", "", ""
                );

            newProfile = new PatientProfile(organizationName, endPoint, testPatient, "Return", DateTime.UtcNow.AddMinutes(-41));

            newList.Add(newProfile);

            return newList;
        }
    }
}