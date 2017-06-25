// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using Hl7.Fhir.Model;
using NoID.FHIR.Profile;
using NoID.Security;
using NoID.Utilities;
using NoID.Network.Transport;

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
                patientFHIRProfile.BirthDay = formatDateOfBirth(birthYear, birthMonth, birthDay);
                patientFHIRProfile.StreetAddress = streetAddress;
                patientFHIRProfile.StreetAddress2 = streetAddress2;
                patientFHIRProfile.City = city;
                patientFHIRProfile.State = state;
                patientFHIRProfile.PostalCode = postalCode;
                patientFHIRProfile.EmailAddress = emailAddress;
                patientFHIRProfile.PhoneCell = phoneCell;
                // Send FHIR message
                Authentication auth = SecurityUtilities.GetAuthentication(_serviceName);
                HttpsClient client = new HttpsClient();
                Patient pt = patientFHIRProfile.CreateFHIRPatientProfile();
                if (client.SendFHIRPatientProfile(_endPoint, auth, pt) == false)
                {
                    // Error occured set error description
                    ErrorDescription = client.ResponseText;
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorDescription = ex.Message;
                return false;
            }
            return true;
        }

        public string ErrorDescription
        {
            get { return _errorDescription; }
            private set { _errorDescription = value; }
        }

        private string formatDateOfBirth(string year, string month, string day)
        {
            string birthString = "";
            if (year.Length == 4)
            {
                birthString = year + "-";
            }
            else
            {
                throw new Exception("Invalid day (" + year + ") in the date of birth!");
            }
            switch (month)
            {
                case "Jan":
                    birthString += "01-";
                    break;
                case "Feb":
                    birthString += "02-";
                    break;
                case "Mar":
                    birthString += "03-";
                    break;
                case "Apr":
                    birthString += "04-";
                    break;
                case "May":
                    birthString += "05-";
                    break;
                case "June":
                    birthString += "06-";
                    break;
                case "July":
                    birthString += "07-";
                    break;
                case "Aug":
                    birthString += "08-";
                    break;
                case "Sept":
                    birthString += "09-";
                    break;
                case "Oct":
                    birthString += "10-";
                    break;
                case "Nov":
                    birthString += "11-";
                    break;
                case "Dec":
                    birthString += "12-";
                    break;
                default:
                    throw new Exception("Invalid month (" + month +") in the date of birth!");
            }
            //birthYear + "-" + birthMonth + "-" + birthDay
            if (day.Length == 1)
            {
                birthString += "0" + day;
            }
            else if (day.Length == 2)
            {
                birthString += day;
            }
            else
            {
                throw new Exception("Invalid day (" + day + ") in the date of birth!");
            }
            return birthString;
        }
    }
}
