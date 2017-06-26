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
    /// <summary>Binds JavaScript in the CEF browser with this .NET framework class.
    /// <para> Used to handle the Patient UI and briges calls between CEF and .NET.</para>
    /// <para> Uses JavaScript naming because CEF renames them anyways.</para>
    /// <para> Use CEFBridge as a base class and contains</para>
    /// <para> organizationName, endPoint, serviceName, errorDescription, alertFunction</para>
    /// <seealso cref="CEFBridge"/>
    /// </summary>

    class PatientBridge : CEFBridge
    {
        

        //default capture site and laterality.
        FHIRUtilities.CaptureSiteSnoMedCode _captureSite = FHIRUtilities.CaptureSiteSnoMedCode.IndexFinger;
        FHIRUtilities.LateralitySnoMedCode _laterality = FHIRUtilities.LateralitySnoMedCode.Left;
        SourceAFIS.Templates.NoID _noID;
        PatientFHIRProfile _patientFHIRProfile;

        public PatientBridge(string organizationName, Uri endPoint, string serviceName) : base(organizationName, endPoint, serviceName)
        {
            _noID = new SourceAFIS.Templates.NoID();
            _noID.SessionID = StringUtilities.SHA256(Guid.NewGuid().ToString());
            _patientFHIRProfile = new PatientFHIRProfile(organizationName, endPoint);
        }

        ~PatientBridge() { }

        // C# -> Javascript function is NoIDBridge.postLateralityCaptureSite ( <params> )
        public bool postLateralityCaptureSite(string laterality, string captureSite)
        {
            try
            {
                _captureSite = FHIRUtilities.StringToCaptureSite(captureSite);
                _laterality = FHIRUtilities.StringToLaterality(laterality);
                errorDescription = "";
            }
            catch (Exception ex)
            {
                errorDescription = ex.Message;
                return false;
            }
            return true;
        }

        //  C# -> Javascript function is NoIDBridge.postDemographics( <params> )
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

                _patientFHIRProfile.Language = language;
                _patientFHIRProfile.LastName = lastName;
                _patientFHIRProfile.FirstName = firstName;
                _patientFHIRProfile.MiddleName = middleName;
                
                if (gender.ToLower() == "f")
                {
                    _patientFHIRProfile.Gender = "F";
                }
                else if (gender.ToLower() == "m")
                {
                    _patientFHIRProfile.Gender = "M";
                }
                
                _patientFHIRProfile.BirthDate = formatDateOfBirth(birthYear, birthMonth, birthDay);
                _patientFHIRProfile.StreetAddress = streetAddress;
                _patientFHIRProfile.StreetAddress2 = streetAddress2;
                _patientFHIRProfile.City = city;
                _patientFHIRProfile.State = state;
                _patientFHIRProfile.PostalCode = postalCode;
                _patientFHIRProfile.EmailAddress = emailAddress;
                _patientFHIRProfile.PhoneCell = phoneCell;
                // Send FHIR message
                Authentication auth = SecurityUtilities.GetAuthentication(serviceName);
                HttpsClient client = new HttpsClient();
                Patient pt = _patientFHIRProfile.CreateFHIRPatientProfile();
                if (client.SendFHIRPatientProfile(endPoint, auth, pt) == false)
                {
                    // Error occured set error description
                    errorDescription = client.ResponseText;
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorDescription = ex.Message;
                return false;
            }
            return true;
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
                    throw new Exception("Invalid month (" + month + ") in the date of birth!");
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

        public FHIRUtilities.CaptureSiteSnoMedCode captureSite
        {
            get { return _captureSite; }
            set { _captureSite = value; }
        }

        public FHIRUtilities.LateralitySnoMedCode laterality
        {
            get { return _laterality; }
            set { _laterality = value; }
        }

        private void ResetVariables()
        {
            alertFunction = "";
            _captureSite = FHIRUtilities.CaptureSiteSnoMedCode.IndexFinger;
            _laterality = FHIRUtilities.LateralitySnoMedCode.Right;
            _noID = new SourceAFIS.Templates.NoID();
            _noID.SessionID = StringUtilities.SHA256(Guid.NewGuid().ToString());
        }

        public bool postResetForNewPatient()
        {
            try
            {
                ResetVariables();
            }
            catch (Exception ex)
            {
                errorDescription = ex.Message;
                return false;
            }
            return true;
        }

        public string sessionID
        {
            get { return _noID.SessionID; }
        }

        public string localNoID
        {
            get { return _noID.LocalNoID; }
            set { _noID.LocalNoID = value; }
        }

        public string remoteNoID
        {
            get { return _noID.RemoteNoID; }
            set { _noID.RemoteNoID = value; }
        }
    }
}
