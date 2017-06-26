// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using NoID.Utilities;
using NoID.FHIR.Profile;

namespace NoID.Browser
{
    static class TestPatientList
    {
        /// <summary>Creates a test patient list for the ProviderBridge class
        /// <seealso cref="ProviderBridge"/>
        /// </summary>

        public static IList<PatientProfile> GetTestPatients(string organizationName)
        {
            Uri endPoint = new Uri(StringUtilities.RemoveTrailingBackSlash(System.Configuration.ConfigurationManager.AppSettings["HealthcareNodeFHIRAddress"].ToString()));
            List<PatientProfile> newList = new List<PatientProfile>();

            Patient testPatient = FHIRUtilities.CreateTestFHIRPatientProfile
                (
                organizationName, Guid.NewGuid().ToString(), "", "English", "1961-04-22", "F", "No",
                "Donna", "Marie", "Kasick", "4712 W 3rd St.", "Apt 35", "New York", "NY", "10000-2221",
                "212-555-3000", "212-555-7400", "212-555-9555", "donnakasick@yandex.com"
                );

            PatientProfile newProfile = new PatientProfile(organizationName, endPoint, testPatient);


            newList.Add(newProfile);

            testPatient = FHIRUtilities.CreateTestFHIRPatientProfile
                (
                organizationName, Guid.NewGuid().ToString(), "", "English", "1992-10-30", "F", "Yes",
                "Christine", "J", "Pinkentinfuter", "2088 N 23nd St.", "Unit 51", "New York", "NY", "10012-0159",
                "", "", "", ""
                );

            newProfile = new PatientProfile(organizationName, endPoint, testPatient);

            newList.Add(newProfile);

            testPatient = FHIRUtilities.CreateTestFHIRPatientProfile
                (
                organizationName, Guid.NewGuid().ToString(), "", "English", "1954-02-19", "M", "No",
                "Mitchel", "James", "Hendrichs", "2442 Bleaker St.", "Apt 722", "New York", "NY", "10503-04855",
                "212-111-1234", "", "", ""
                );

            newProfile = new PatientProfile(organizationName, endPoint, testPatient);

            newList.Add(newProfile);

            testPatient = FHIRUtilities.CreateTestFHIRPatientProfile
                (
                organizationName, Guid.NewGuid().ToString(), "", "English", "2001-03-21", "M", "No",
                "Brian", "H", "O'Donald", "212 Cremont", "", "New Jersey", "NJ", "14235",
                "", "212-555-1234", "", "BrianODonald2001@yahoo.com"
                );

            newProfile = new PatientProfile(organizationName, endPoint, testPatient);

            newList.Add(newProfile);

            testPatient = FHIRUtilities.CreateTestFHIRPatientProfile
                (
                organizationName, Guid.NewGuid().ToString(), "", "English", "1999-09-02", "F", "No",
                "Jonie", "M", "Smith", "", "", "", "", "",
                "", "", "", ""
                );

            newProfile = new PatientProfile(organizationName, endPoint, testPatient);

            newList.Add(newProfile);

            return newList;
        }
    }
}
