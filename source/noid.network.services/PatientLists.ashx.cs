﻿// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Web;
using System.Web.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using NoID.FHIR.Profile;
using NoID.Utilities;
using NoID.Database.Wrappers;

namespace NoID.Network.Services
{
    /// <summary>
    /// Web service that returns a list of pending patients profiles
    /// </summary>
    public class PatientLists : IHttpHandler
    {
        private readonly Uri sparkEndpointAddress = new Uri(StringUtilities.RemoveTrailingBackSlash(WebConfigurationManager.AppSettings["SparkEndpointAddress"].ToString()));
        private static readonly string NoIDMongoDBAddress = WebConfigurationManager.AppSettings["NoIDMongoDBAddress"].ToString();
        private static readonly string SparkMongoDBAddress = WebConfigurationManager.AppSettings["SparkMongoDBAddress"].ToString();
        private readonly string OrganizationName = WebConfigurationManager.AppSettings["OrganizationName"].ToString();
        private readonly string DomainName = WebConfigurationManager.AppSettings["DomainName"].ToString();
        private List<Exception> _exceptions = new List<Exception>();

        public void ProcessRequest(HttpContext context)
        {
            string jsonResponse = null;
            try
            {
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
            catch (Exception ex)
            {
                context.Response.Write("PatientLists::ProcessRequest Error: " + ex.Message);
            }
            
        }

        //TODO: Add
        private IList<PatientProfile> GetPendingPatients()
        {
            List<PatientProfile> listPending = new List<PatientProfile>();
            try
            {
                MongoDBWrapper dbwrapper = new MongoDBWrapper(NoIDMongoDBAddress, SparkMongoDBAddress);
                List<SessionQueue> pendingSessionList = dbwrapper.GetPendingPatients();
                FhirClient client = new FhirClient(sparkEndpointAddress);

                foreach (var pending in pendingSessionList)
                {
                    string sparkAddress = sparkEndpointAddress.ToString() + "/Patient/" + pending.SparkReference;
                    Patient pendingPatient = (Patient)client.Get(sparkAddress);
                    PatientProfile patientProfile = new PatientProfile(pendingPatient, true);
                    patientProfile.SessionID = pending.SparkReference;
                    patientProfile.LocalNoID = pending.LocalReference;
                    listPending.Add(patientProfile);
                }

                /*
                string gtDateFormat = "gt" + FHIRUtilities.DateToFHIRString(DateTime.UtcNow.AddDays(-2));
                client.PreferredFormat = ResourceFormat.Json;
                Uri uriTwoDays = new Uri(sparkEndpointAddress.ToString() + "/Patient?_lastUpdated=" + gtDateFormat);
                Bundle patientBundle = (Bundle)client.Get(uriTwoDays);
                foreach (Bundle.EntryComponent entry in patientBundle.Entry)
                {
                    string ptURL = entry.FullUrl.ToString().Replace("http://localhost:49911/fhir", sparkEndpointAddress.ToString());
                    Patient pt = (Patient)client.Get(ptURL);
                    if (pt.Meta.Extension.Count > 0)
                    {
                        Extension ext = pt.Meta.Extension[0];
                        if (ext.Value.ToString().ToLower().Contains("pending") == true)
                        {
                            PatientProfile patientProfile = new PatientProfile(pt, false);
                            listPending.Add(patientProfile);
                        }
                    }
                }
                */
            }
            catch(Exception ex)
            {
                throw ex;
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