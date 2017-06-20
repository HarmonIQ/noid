// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using NoID.FHIR.Profile;

namespace NoID.Browser
{
    public class NoIDBridge
    {
        string _organizationName;
        Uri _endPoint;
        string _serviceName;
        string _errorDescription;

        public NoIDBridge(string organizationName, Uri endPoint, string serviceName)
        {
            _organizationName = organizationName;
            _endPoint = endPoint;
            _serviceName = serviceName;
        }

        // JavaScript function is NoIDBridge.postDemographics( <params> )
        public bool postDemographics
            (
                string language,
                string firstName,
                string middleName,
                string lastName,
                string gender,
                string birthYear,
                string birthMonth,
                string birthDay,
                string streetAddress,
                string streetAddress2,
                string city,
                string state,
                string postalCode,
                string emailAddress,
                string phoneCell
            )
        {
            try
            {
                PatientFHIRProfile patientFHIRProfile = new PatientFHIRProfile(_organizationName, _endPoint);
                patientFHIRProfile.Language = language;
                patientFHIRProfile.LastName = lastName;
                patientFHIRProfile.FirstName = firstName;
                patientFHIRProfile.MiddleName = middleName;
                Hl7.Fhir.Model.AdministrativeGender genderType = Hl7.Fhir.Model.AdministrativeGender.Unknown;
                if (gender == "f")
                {
                    genderType = Hl7.Fhir.Model.AdministrativeGender.Female;
                }
                else if (gender == "m")
                {
                    genderType = Hl7.Fhir.Model.AdministrativeGender.Male;
                }
                patientFHIRProfile.Gender = genderType;
                patientFHIRProfile.BirthDay = birthYear + "-" + birthMonth + "-" + birthDay;
                patientFHIRProfile.StreetAddress = streetAddress;
                patientFHIRProfile.StreetAddress2 = streetAddress2;
                patientFHIRProfile.City = city;
                patientFHIRProfile.State = state;
                patientFHIRProfile.PostalCode = postalCode;
                patientFHIRProfile.EmailAddress = emailAddress;
                patientFHIRProfile.PhoneCell = phoneCell;
                /*
                Authentication auth = new Authentication(TestUserName, TestPassword);
                Uri endpoint = new Uri(TestEndPoint);
                WebSend ws = new WebSend(endpoint, auth, payload);
                */
            }
            catch (Exception ex)
            {
                ErrorDescription = ex.Message;
            }
            return true;
        }

        public string ErrorDescription
        {
            get { return _errorDescription; }
            private set { _errorDescription = value; }
        }
    }
}
