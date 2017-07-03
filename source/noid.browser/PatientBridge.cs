// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using Hl7.Fhir.Model;
using NoID.FHIR.Profile;
using NoID.Security;
using NoID.Utilities;
using NoID.Network.Transport;
using System.Configuration;

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
        private static readonly string DevicePhysicalLocation = ConfigurationManager.AppSettings["DevicePhysicalLocation"].ToString();
        private static readonly string ClinicArea = ConfigurationManager.AppSettings["ClinicArea"].ToString();
        private static readonly string AddNewPatientUri = ConfigurationManager.AppSettings["AddNewPatientUri"].ToString();
        private static readonly string SearchBiometricsUri = ConfigurationManager.AppSettings["SearchBiometricsUri"].ToString();

        FHIRUtilities.CaptureSiteSnoMedCode _captureSite = FHIRUtilities.CaptureSiteSnoMedCode.IndexFinger;
        FHIRUtilities.LateralitySnoMedCode _laterality = FHIRUtilities.LateralitySnoMedCode.Left;

        PatientFHIRProfile _patientFHIRProfile;
        string _reponseString;
		    bool _hasValidLeftFingerprint = false;
		    bool _hasValidRightFingerprint = false;
		    string _exceptionMissingReason = "";
		    string _secretAnswer1 = "";
		    string _secretAnswer2 = "";
		    string _existingDOBMatch = "";


		    public delegate void PatientEventHandler(object sender, string trigger);
        public event PatientEventHandler ResetSession = delegate { };


        public PatientBridge(string organizationName,  string serviceName) : base(organizationName, serviceName)
        {
            _patientFHIRProfile = new PatientFHIRProfile(organizationName, "Pending");
        }

        ~PatientBridge() { }

        private void TriggerResetSession(string trigger)
        {
            if (ResetSession != null)
                ResetSession(this, trigger);
        }

        public bool postMissingBiometricInfo(string exceptionMissingReason,	string secretAnswer1, string secretAnswer2)
		{
			try
			{
				_exceptionMissingReason = exceptionMissingReason;
				_secretAnswer1 = secretAnswer1;
				_secretAnswer2 = secretAnswer2;
			}
			catch (Exception ex)
			{
				errorDescription = ex.Message;
				return false;
			}
			return true;
		}
		public bool postUnknownDOBExistingpatient(string passedLocalNoID)
		{
			try
			{
				//need to define id to pass back. Calling existing id match does not seem to have id available
				//testing. remove below message
				// need to add patient to approval queue from this step, but flag the type with an *
				//need to return 
				//errorDescription = "Need to wire this up. Currently passing NoID : " + HandleNullString(sessionID);
				errorDescription = "";
			}
			catch (Exception ex)
			{
				errorDescription = ex.Message;
				return false;
			}
			return true;
		}
		public bool postConfirmExistingPatient(string passedLocalNoID, string birthYear, string birthMonth, string birthDay)
		{
			try
			{
				//need to define id to pass back. Calling existing id match does not seem to have id available
				//testing. remove below message
				// if dob == dob then set _existingDOBMatch to "match" and send patient to queue as returning patient
				//need to return successful add to patient queue message so I can begin close. For now hardcodeing to 10 seconds
				//_existingDOBMatch ??
				_existingDOBMatch = "match";
				errorDescription = "";
			}
			catch (Exception ex)
			{
				errorDescription = ex.Message;
				return false;
			}
			return true;
		}
		public bool postDoNotHaveValidBiometricButtonclick(string laterality) {
			try
			{
				if (laterality == "Left")
				{
					_hasValidLeftFingerprint = false;					
				}				
				if (laterality == "Right")
				{
					_hasValidRightFingerprint = false;
				}
				if (_hasValidLeftFingerprint == false && _hasValidRightFingerprint == false)
				{
					showExceptionModal = "yes";
				}
				else
				{
					errorDescription = "";
				}
			}
			catch (Exception ex)
			{
				errorDescription = ex.Message;
				return false;
			}
			return true;
		}

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

        public PatientFHIRProfile PatientFHIRProfile
        {
            get { return _patientFHIRProfile; }
            set { _patientFHIRProfile = value; }
        }

        public Uri fhirAddress
        {
            get { return _patientFHIRProfile.FHIRAddress; }
            set { _patientFHIRProfile.FHIRAddress = value; }
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
                _patientFHIRProfile.ClinicArea = ClinicArea;
                _patientFHIRProfile.DevicePhysicalLocation = DevicePhysicalLocation;
                _patientFHIRProfile.NoIDType = "New";
                _patientFHIRProfile.NoIDStatus = "Pending";
                _patientFHIRProfile.CheckinDateTime = FHIRUtilities.DateTimeToFHIRString(DateTime.UtcNow);
                // Send FHIR message
                Authentication auth;
                if (Utilities.Auth == null)
                {
                    auth = SecurityUtilities.GetAuthentication(serviceName);
                }
                else
                {
                    auth = Utilities.Auth;
                }
                HttpsClient client = new HttpsClient();
                Patient pt = _patientFHIRProfile.CreateFHIRPatientProfile();

                // TODO: REMOVE THIS LINE!  ONLY FOR TESTING
                //FHIRUtilities.SaveJSONFile(pt, @"C:\JSONTest");
                fhirAddress = new Uri(AddNewPatientUri);
                if (client.SendFHIRPatientProfile(fhirAddress, auth, pt) == false)
                {
                    // Error occured set error description
                    errorDescription = client.ResponseText;
                    return false;
                }
                else
                {
                    // No error, return message.
                    _reponseString = client.ResponseText;
                }
            }
            catch (Exception ex)
            {
                errorDescription = ex.Message;
                return false;
            }
            return true;
        }

        //postChangePage
        //  C# -> Javascript function is NoIDBridge.postChangePage( <params> )
        public bool postChangePage(string newPageName)
        {
            bool result = false;
            try
            {
                /*
                onclick="setCurrentPage('SelectLanguage');"
                onclick="setCurrentPage('SelectNewOrReturn');"
                onclick="setCurrentPage('ConsentAgreement');"
                onclick="setCurrentPage('SelectLeftFinger');"
                onclick="setCurrentPage('SelectRightFinger');"
                onclick="setCurrentPage('EnterDemographics');"
                onclick="setCurrentPage('EnterContactInfo');"
                onclick="setCurrentPage('SelectHub');"
                onclick="setCurrentPage('EnterPassword');"
                onclick="setCurrentPage('ReviewEnrollment');"
                onclick="setCurrentPage('EnrollmentComplete');"
                */
                currentPage = newPageName;
                result = true;
            }
            catch (Exception ex)
            {
                errorDescription = ex.Message;
            }
            return result;
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
            _captureSite = FHIRUtilities.CaptureSiteSnoMedCode.Unknown;
            _laterality = FHIRUtilities.LateralitySnoMedCode.Unknown;
            _patientFHIRProfile = new PatientFHIRProfile(organizationName, "New");
            _patientFHIRProfile.FHIRAddress = null;
            TriggerResetSession("PatientBridge::ResetVariables");
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
		string HandleNullString(string convert)
		{
			if (convert == null)
			{
				return "";
			}
			return convert;
		}
		public string sessionID
        {
            get { return _patientFHIRProfile.SessionID; }
        }

        public string localNoID
        {
            get { return _patientFHIRProfile.LocalNoID; }
            set { _patientFHIRProfile.LocalNoID = value; }
        }

        public string remoteNoID
        {
            get { return _patientFHIRProfile.RemoteNoID; }
            set { _patientFHIRProfile.RemoteNoID = value; }
        }

		public bool hasValidLeftFingerprint
		{
			get { return _hasValidLeftFingerprint; }
			set { _hasValidLeftFingerprint = value; }
		}

		public bool hasValidRightFingerprint
		{
			get { return _hasValidRightFingerprint; }
			set { _hasValidRightFingerprint = value; }
		}

		public string exceptionMissingReason
		{
			get { return _exceptionMissingReason; }
			set { _exceptionMissingReason = value; }
		}

		public string secretAnswer1
		{
			get { return _secretAnswer1; }
			set { _secretAnswer1 = value; }
		}
		public string secretAnswer2
		{
			get { return _secretAnswer2; }
			set { _secretAnswer2 = value; }
		}
		public string existingDOBMatch
		{
			get { return _existingDOBMatch; }
			set { _existingDOBMatch = value; }
		}
	}
}
