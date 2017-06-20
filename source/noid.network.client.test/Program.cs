﻿// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using NoID.FHIR.Profile;
using NoID.Security;
using NoID.Utilities;
using NoID.Network.Transport;

namespace NoID.Network.Client.Test
{
    class Program
    {
        private static readonly string FHIREndPoint = System.Configuration.ConfigurationManager.AppSettings["FHIREndPoint"].ToString();
        private static readonly string NoIDServiceName = System.Configuration.ConfigurationManager.AppSettings["NoIDServiceName"].ToString();
        
        static void Main(string[] args)
        {
            SendJSON();
            SendProtoBuffer();
            Console.ReadLine();
        }

        private static void SendJSON()
        {
            Patient payload = FHIRUtilities.CreateTestFHIRPatientProfile();
            Authentication auth = SecurityUtilities.GetAuthentication(NoIDServiceName);
            Uri endpoint = new Uri(FHIREndPoint);
            HttpsClient client = new HttpsClient();
            client.SendFHIRPatientProfile(endpoint, auth, payload);
            Console.WriteLine(client.ResponseText);
        }

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
            pt.BirthDay = "20030514";
            pt.Gender = AdministrativeGender.Female;
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
    }
}
