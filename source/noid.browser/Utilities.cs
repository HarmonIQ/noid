// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace NoID.Browser
{
    public static class Utilities
    {
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static string RemoveTrailingBackSlash(string s)
        {
            string newString = s;
            string lastChar = Reverse(s).Substring(0, 1);
            if (lastChar == "/")
            {
                newString = newString.Substring(0, newString.Length - 1);
            }
            return newString;
        }

        public static Patient CreateTestFHIRPatientProfile()
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

            ResourceReference managingOrganization = new ResourceReference(NoID.FHIR.Profile.Utilities.NoID_OID, "Test NoID");
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
