// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using NoID.Network.Security;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using NoID.FHIR.Profile;

namespace NoID.Network.Client.Test
{
    class Program
    {
        private static readonly string TestUserName = System.Configuration.ConfigurationManager.AppSettings["TestUserName"].ToString();
        private static readonly string TestPassword = System.Configuration.ConfigurationManager.AppSettings["TestPassword"].ToString();
        private static readonly string TestEndPoint = System.Configuration.ConfigurationManager.AppSettings["TestEndPoint"].ToString();

        static void Main(string[] args)
        {
            //SendJSON();
            SendProtoBuffer();
            Console.ReadLine();
        }

        private static void SendJSON()
        {
            Patient payload = CreateTestFHIRPatientProfile();
            Authentication auth = new Authentication(TestUserName, TestPassword);
            Uri endpoint = new Uri(TestEndPoint);
            WebSend ws = new WebSend(endpoint, auth, payload);
            Console.WriteLine(ws.PostHttpWebRequest());
        }

        private static void SendProtoBuffer()
        {
            PatientFHIRProfile payload = CreatePatientFHIRProfile();
            Authentication auth = new Authentication(TestUserName, TestPassword);
            Uri endpoint = new Uri(TestEndPoint);
            WebSend ws = new WebSend(endpoint, auth, payload);
            Console.WriteLine(ws.PostHttpWebRequest());
        }

        private static PatientFHIRProfile CreatePatientFHIRProfile()
        {
            PatientFHIRProfile pt = new PatientFHIRProfile("Test Org", new Uri(TestEndPoint));
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

        private static Patient CreateTestFHIRPatientProfile()
        {
            Patient pt = new Patient();
            Identifier idSession;
            Identifier idPatientCertificate;

            idSession = new Identifier();
            idSession.System = "http://www.mynoid.com/fhir/SessionID";
            idSession.Value = "S12348";
            pt.Identifier.Add(idSession);

            idPatientCertificate = new Identifier();
            idPatientCertificate.System = "http://www.mynoid.com/fhir/PatientCertificateID";
            idPatientCertificate.Value = "PT67891";
            pt.Identifier.Add(idPatientCertificate);

            ResourceReference managingOrganization = new ResourceReference("238.3.322.21.2", "Test NoID");
            pt.ManagingOrganization = managingOrganization;

            pt.Language = "English";
            pt.BirthDate = "20060703";
            pt.Gender = AdministrativeGender.Female;
            pt.MultipleBirth = new FhirString("No");
            // Add patient name
            HumanName ptName = new HumanName();
            ptName.Given = new string[] { "Mary", "J" };
            ptName.Family = "Bling";
            pt.Name = new List<HumanName> { ptName };
            // Add patient address
            Address address = new Address();
            address.Line = new string[] { "300 Exit St", "Unit 5" };
            address.City = "New Orleans";
            address.State = "LA";
            address.Country = "USA";
            address.PostalCode = "70112-1202";
            pt.Address.Add(address);

            return pt;
        }
    }
}
