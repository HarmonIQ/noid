// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Configuration;
using System.Collections.Generic;
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
        private static readonly string DevicePhysicalLocation = ConfigurationManager.AppSettings["DevicePhysicalLocation"].ToString();
        private static readonly string ClinicArea = ConfigurationManager.AppSettings["ClinicArea"].ToString();
        private static readonly string AddNewPatientUri = ConfigurationManager.AppSettings["AddNewPatientUri"].ToString();
        private static readonly string SearchBiometricsUri = ConfigurationManager.AppSettings["SearchBiometricsUri"].ToString();
        private static readonly string IdentityChallengeUri = ConfigurationManager.AppSettings["IdentityChallengeUri"].ToString();
        private static readonly string SecretQuestion1 = ConfigurationManager.AppSettings["SecretQuestion1"].ToString();
        private static readonly string SecretQuestion2 = ConfigurationManager.AppSettings["SecretQuestion2"].ToString();

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
		bool _cannotCaptureLeftFingerprint = false;
		bool _cannotCaptureRightFingerprint = false;

		public delegate void PatientEventHandler(object sender, string trigger);
        public event PatientEventHandler ResetSession = delegate { };

        public PatientBridge(string organizationName,  string serviceName) : base(organizationName, serviceName)
        {
            _patientFHIRProfile = new PatientFHIRProfile(organizationName, "Pending");
        }

        ~PatientBridge() { }

		public void Dispose()
		{
			_hasValidLeftFingerprint = false;
			_hasValidRightFingerprint = false;
			_captureSite = FHIRUtilities.CaptureSiteSnoMedCode.IndexFinger;
			_laterality = FHIRUtilities.LateralitySnoMedCode.Left;
			_exceptionMissingReason = "";
			_secretAnswer1 = "";
			_secretAnswer2 = "";
			_existingDOBMatch = "";
			_cannotCaptureLeftFingerprint = false;
			_cannotCaptureRightFingerprint = false;
			base.alertFunction = "";
			base.currentPage = "";
			base.errorDescription = "";
			base.showExceptionModal = "";
			_patientFHIRProfile.Dispose();
		}

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

        //TODO: Update function in JavaScript and C# to postUnknownDOBExistingPatient
        public bool postUnknownDOBExistingpatient(string passedLocalNoID)
		{
            bool result = false;
            try
            {
                Authentication auth;
                if (Utilities.Auth == null)
                {
                    auth = SecurityUtilities.GetAuthentication(serviceName);
                }
                else
                {
                    auth = Utilities.Auth;
                }
                localNoID = passedLocalNoID;
                HttpsClient client = new HttpsClient();
                Uri endpoint = new Uri(IdentityChallengeUri);
                string resultResponse = client.SendIdentityChallenge(endpoint, auth, passedLocalNoID, "failedchallenge", "", SecurityUtilities.GetComputerName(), ClinicArea);

                if (resultResponse.ToLower() == "yes")
                {
                    result = true;
                }
                else if (resultResponse.ToLower().Contains("error") == true)
                {
                    errorDescription = "Error in PatientBridge::postUnknownDOBExistingPatient: " + resultResponse.Substring(3);
                }
            }
            catch (Exception ex)
            {
                errorDescription = "Error in PatientBridge::postUnknownDOBExistingPatient: " + ex.Message;
            }
            return result;
        }

		public bool postConfirmExistingPatient(string passedLocalNoID, string birthYear, string birthMonth, string birthDay)
		{
            bool result = false;
			try
			{
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
                Uri endpoint = new Uri(IdentityChallengeUri);
                string isoBirthDate = formatDateOfBirth(birthYear, birthMonth, birthDay);
                string resultResponse = client.SendIdentityChallenge(endpoint, auth, localNoID, "birthdate", isoBirthDate, SecurityUtilities.GetComputerName(), ClinicArea);

                if (resultResponse.ToLower() == "yes")
                {
                    _existingDOBMatch = "match";
                    result = true;
                }
                else if (resultResponse.ToLower().Contains("error") == true)
                {
                    errorDescription = "Error in PatientBridge::postConfirmExistingPatient: " + resultResponse.Substring(3);
                }
			}
			catch (Exception ex)
			{
                errorDescription = "Error in PatientBridge::postConfirmExistingPatient: " + ex.Message;
            }
			return result;
		}

		public bool postDoNotHaveValidBiometricButtonclick(string laterality) {
			try
			{
				errorDescription = "";				

				if (laterality == "Left")
				{
					_cannotCaptureLeftFingerprint = true;
					_laterality = FHIRUtilities.StringToLaterality("Right");
					return true;
				}
				else if (laterality == "Right")
				{
					_cannotCaptureRightFingerprint = true;
					_laterality = FHIRUtilities.StringToLaterality("Unknown");
				}

				if (_cannotCaptureLeftFingerprint == true && _cannotCaptureRightFingerprint == true)
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

		public bool postDemoForNoBioMatch
			(
			string languageSelected, 
			string firstName, 
			string middleName, 
			string lastName, 
			string gender, 
			string selectedBirthYear,
			string selectedBirthMonth,
			string selectedBirthDay,
			string streetAddress,
			string streetAddress2,
			string city,
			string state,
			string postalCode,
			string emailAddress,
			string phoneCell,
			string selectedExceptionReason,
			string secretExAnswer1,
			string secretExAnswer2,
			string selectedsecretQuestion1,
			string selectedsecretQuestion2
			)
		{
			try
			{
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
        // Maybe rename to postEnrollment
        // TODO: pass multi birth flag, password, hub selected, gender Q&A to this funtion.
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
                string phoneCell,
				string multipleBirthFlag,
				string genderChangeFlag,
				string password,
				string patientHub,				
				string doesLeftBiometricExist,
				string doesRightBiometricExist,
				string missingBiometricReason,
				string secretExAnswer1,
				string secretExAnswer2,
				string selectedsecretQuestion1,
				string selectedsecretQuestion2
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
                _patientFHIRProfile.NoIDHubPassword = password;
                _patientFHIRProfile.NoIDHubName = patientHub;
                _patientFHIRProfile.MultipleBirth = multipleBirthFlag;
                _patientFHIRProfile.GenderChangeFlag = genderChangeFlag;
                _patientFHIRProfile.BiometricExceptionMissingReason = exceptionMissingReason;
                _patientFHIRProfile.SecretQuestion1 = SecretQuestion1; //TODO: Implement non-fixed questions and get value from HTML
                _patientFHIRProfile.SecretAnswer1 = secretAnswer1;
                _patientFHIRProfile.SecretQuestion2 = SecretQuestion2; //TODO: Implement non-fixed questions and get value from HTML
                _patientFHIRProfile.SecretAnswer2 = secretAnswer2;

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
                    errorDescription = HandleNullString(client.ResponseText);
                    return false;
                }
                else
                {
                    // No error, return message.
                    _reponseString = HandleNullString(client.ResponseText);
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
            _captureSite = FHIRUtilities.CaptureSiteSnoMedCode.LittleFinger;
            _laterality = FHIRUtilities.LateralitySnoMedCode.Left;
            _patientFHIRProfile = new PatientFHIRProfile(organizationName, "New");
            _patientFHIRProfile.FHIRAddress = null;
            _patientFHIRProfile.FingerPrintMinutiasList = new List<FingerPrintMinutias>();
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

		public bool cannotCaptureLeftFingerprint
		{
			get { return _cannotCaptureLeftFingerprint; }
			set { _cannotCaptureLeftFingerprint = value; }
		}

		public bool cannotCaptureRightFingerprint
		{
			get { return _cannotCaptureRightFingerprint; }
			set {_cannotCaptureRightFingerprint = value; }
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
