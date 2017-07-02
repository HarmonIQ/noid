// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Configuration;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using NoID.Security;
using NoID.Utilities;
using NoID.Network.Transport;
using NoID.FHIR.Profile;
using NoID.Database.Wrappers;

namespace NoID.Network.Client.Test
{
    class Program
    {
        private static readonly string PatentCheckinUri = ConfigurationManager.AppSettings["PatentCheckinUri"].ToString();
        private static readonly string PendingPatientsUri = ConfigurationManager.AppSettings["PendingPatientsUri"].ToString();
        private static readonly string SearchBiometricsUri = ConfigurationManager.AppSettings["SearchBiometricsUri"].ToString();
        private static readonly string NoIDServiceName = ConfigurationManager.AppSettings["NoIDServiceName"].ToString();
        private static readonly string NoIDMongoDBAddress = ConfigurationManager.AppSettings["NoIDMongoDBAddress"].ToString();
        private static readonly string SparkMongoDBAddress = ConfigurationManager.AppSettings["SparkMongoDBAddress"].ToString();
        
        static void Main(string[] args)
        {
            string commandLine = "";
            Console.WriteLine("Enter C for checkin patient, P for pending patient queue, M for Mongo tests, F for fingerprint identity and Q to quit");
            while (commandLine != "q")
            {
                if (commandLine == "c")
                {
                    // call PatentCheckinUri
                    Console.WriteLine("Sending test patient FHIR message.");
                    Patient testPt = TestPatient();
                    SendJSON(testPt);
                    Console.WriteLine("Sending FHIR message from file.");
                    Patient readPt = ReadPatient(@"C:\JSONTest\sample-new-patient.json");
                    SendJSON(readPt);
                }
                else if (commandLine == "p") //send profiles
                {
                    // call PendingPatientsUri
                    IList<PatientProfile> patientProfiles = GetCheckinList();
                    Console.WriteLine("Patient profiles received.");
                }
                else if (commandLine == "m") // MongoDB tests
                {
                    MongoDBWrapper dbwrapper = new MongoDBWrapper(NoIDMongoDBAddress, SparkMongoDBAddress);
                    SessionQueue seq = new SessionQueue();
                    seq._id = Guid.NewGuid().ToString();
                    seq.ClinicArea = "Test Clinic";
                    seq.LocalReference = "123456";
                    seq.SparkReference = "spark5";
                    seq.ApprovalStatus = "pending";
                    seq.PatientStatus = "new";
                    seq.RemoteHubReference = "rem440403";
                    seq.SessionComputerName = "Prototype Computer 1";
                    seq.SubmitDate = DateTime.UtcNow.AddMinutes(-15);
                    seq.PatientBeginDate = DateTime.UtcNow.AddMinutes(-19);
                    Console.WriteLine(seq.Serialize());
                    dbwrapper.AddPendingPatient(seq);
                    List<SessionQueue> PendingPatients  = dbwrapper.GetPendingPatients();
                    dbwrapper.UpdateSessionQueueRecord(seq._id, "approved", "TestUser");
                }
                else if (commandLine == "f") // test fingerprint identity web service
                {
                    Media readMedia = ReadMedia(@"C:\JSONTest\sample-media-fhir-message.json");
                    SendJSON(readMedia);
                }
                string previousCommand = commandLine;
                commandLine = Console.ReadLine();
                if (commandLine.Length > 0)
                {
                    commandLine = commandLine.ToLower().Substring(0, 1);
                }
                else
                {
                    commandLine = previousCommand;
                }
            }
        }

        private static Resource ReadJSONFile(string filePath)
        {
            return FHIRUtilities.FileToResource(filePath);
        }

        private static Patient ReadPatient(string filePath)
        {
            return (Patient)ReadJSONFile(filePath);
        }

        private static Media ReadMedia(string filePath)
        {
            return (Media)ReadJSONFile(filePath);
        }

        private static Patient TestPatient()
        {
            Patient testPatient = FHIRUtilities.CreateTestFHIRPatientProfile
                (
                "Test NoID Org", Guid.NewGuid().ToString(), "", "English", "1961-04-22", "F", "No",
                "Donna", "Marie", "Kasick", "4712 W 3rd St.", "Apt 35", "New York", "NY", "10000-2221",
                "212-555-3000", "212-555-7400", "212-555-9555", "donnakasick@yandex.com"
                );
            return testPatient;
        }

        private static void SendJSON(Patient payload)
        {
            Authentication auth = SecurityUtilities.GetAuthentication(NoIDServiceName);
            Uri endpoint = new Uri(PatentCheckinUri);
            HttpsClient client = new HttpsClient();
            client.SendFHIRPatientProfile(endpoint, auth, payload);
            Console.WriteLine(client.ResponseText);
        }

        private static void SendJSON(Media payload)
        {
            Authentication auth = SecurityUtilities.GetAuthentication(NoIDServiceName);
            Uri endpoint = new Uri(SearchBiometricsUri);
            HttpsClient client = new HttpsClient();
            client.SendFHIRMediaProfile(endpoint, auth, payload);
            Console.WriteLine(client.ResponseText);
        }

        private static IList<PatientProfile> GetCheckinList()
        {
            IList<PatientProfile> PatientProfiles = null;

            Authentication auth = SecurityUtilities.GetAuthentication(NoIDServiceName);
            Uri endpoint = new Uri(PendingPatientsUri);
            HttpsClient client = new HttpsClient();
            PatientProfiles = client.RequestPendingQueue(endpoint, auth);
            Console.WriteLine(client.ResponseText);
            return PatientProfiles;
        }

        /*
        // Example using Google protobuf 
        private static void SendProtoBuffer()
        {
            PatientFHIRProfile payload = CreatePatientFHIRProfile();
            Authentication auth = SecurityUtilities.GetAuthentication(NoIDServiceName);
            Uri endpoint = new Uri(FHIREndPoint);
            WebSend ws = new WebSend(endpoint, auth, payload);
            Console.WriteLine(ws.PostHttpWebRequest());
        }
        
        private static PatientFHIRProfile CreatePatientFHIRProfile()
        {
            PatientFHIRProfile pt = new PatientFHIRProfile("Test Org", new Uri(FHIREndPoint));
            pt.LastName = "Williams";
            pt.FirstName = "Brianna";
            pt.MiddleName = "E.";
            pt.BirthDate = "20030514";
            pt.Gender = AdministrativeGender.Female.ToString().Substring(0,1).ToUpper();
            pt.EmailAddress = "Test@gtest.com";
            pt.StreetAddress = "321 Easy St";
            pt.StreetAddress2 = "Unit 4A";
            pt.City = "New Orleans";
            pt.State = "LA";
            pt.PostalCode = "70112-2110";
            pt.PhoneCell = "15045551212";
            pt.PhoneHome = "12125551212";
            return pt;
        }
        */
    }
}