// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using SourceAFIS.Templates;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using NoID.Utilities;
using NoID.Match.Database.Client;

namespace NoID.FHIR.Profile
{

    /// <summary cref="PatientFHIRProfileSerialize">  
    /// Lightweight NoID Patient FHIR profile
    /// </summary>  

    public class PatientProfile
    {
        private SourceAFIS.Templates.NoID _noID;
        private readonly string _organizationName;
        private Uri _fhirAddress;
        private static List<FingerPrintMinutias> _fingerPrintMinutiasList = new List<FingerPrintMinutias>();
        private Exception _exception;

        private string _language = "";
        private string _firstName = "";
        private string _lastName = "";
        private string _middleName = "";
        private string _gender = ""; // F, M or O
        private string _genderChangeFlag = ""; // true/false
        private string _birthDate = "";
        private string _streetAddress = "";
        private string _streetAddress2 = "";
        private string _city = "";
        private string _state = "";
        private string _country = "";
        private string _postalCode = "";
        private string _phoneHome = "";
        private string _phoneCell = "";
        private string _phoneWork = "";
        private string _emailAddress = "";
        private string _multipleBirth = ""; //Yes or No
        private string _noidStatus = ""; //pending, approved, denied
        private string _noidType = ""; //new, return, error, or critical
        private string _checkinDateTime = "";
        private string _noidHubName = "";
        private string _noidHubPassword = "";
        private string _biometricExceptionMissingReason = "";
        private string _secretQuestion1 = "";
        private string _secretAnswer1 = "";
        private string _secretQuestion2 = "";
        private string _secretAnswer2 = "";
        private string _localNoID = "";
        private string _sessionID = "";
        private string _remoteNoID = "";
        private string _biometricsCaptured = "";
        private string _clinicArea = "";
        private string _devicePhysicalLocation = "";
        private string _deviceStartTime = "";
        private string _domainName = "mynoid.com";
        
        #region Constructors

        [JsonConstructor]
        public PatientProfile()
        {
            NewSession();
        }

        public PatientProfile(string organizationName, string noidStatus)
        {
            _organizationName = organizationName;
            _noidStatus = noidStatus;
            NewSession();
        }

        public PatientProfile(string organizationName, Uri fhirAddress, Patient loadPatient, string noidStatus, DateTime checkinDateTime)
        {
            _organizationName = organizationName;
            _fhirAddress = fhirAddress;
            _noidStatus = noidStatus;
            _checkinDateTime = FHIRUtilities.DateTimeToFHIRString(checkinDateTime);
            NewSession();

            if (loadPatient != null)
            {
                _noID = new SourceAFIS.Templates.NoID();

                SetMetaData(loadPatient); //Sets all the data from the meta area of FHIR message
                SetIdentifierData(loadPatient); //Sets all the data from the identifier area of FHIR message

                // Gets the demographics from the patient FHIR resource class
                _lastName   = loadPatient.Name[0].Family.ToString();
                List<string> givenNames = loadPatient.Name[0].Given.ToList();
                _firstName = givenNames[0].ToString();
                if (givenNames.Count > 1)
                {
                    _middleName = givenNames[1].ToString();
                }
                _gender = loadPatient.Gender.ToString().Substring(0, 1).ToUpper();
                _birthDate = loadPatient.BirthDate.ToString();

                // Gets the address information from the patient FHIR resource class
                if (loadPatient.Address.Count > 0)
                {
                    List<string> addressLines = loadPatient.Address[0].Line.ToList();
                    _streetAddress = addressLines[0].ToString();
                    if (addressLines.Count > 1)
                    {
                        _streetAddress2 = addressLines[1].ToString();
                    }
                   
                    _city = loadPatient.Address[0].City.ToString();
                    _state = loadPatient.Address[0].State.ToString();
                    _postalCode = loadPatient.Address[0].PostalCode.ToString();
                    _country = loadPatient.Address[0].Country.ToString();
                }
                // Gets the contact information from the patient FHIR resource class
                if (loadPatient.Contact.Count > 0)
                {
                    foreach(var contact in loadPatient.Contact)
                    {
                        foreach (var telecom in contact.Telecom)
                        {
                            if (telecom.Use.ToString().ToLower() == "home")
                            {
                                if (telecom.System.ToString().ToLower() == "email")
                                {
                                    EmailAddress = telecom.Value.ToString();
                                }
                                else if (telecom.System.ToString().ToLower() == "phone")
                                {
                                    PhoneHome = telecom.Value.ToString();
                                }
                            }
                            else if (telecom.Use.ToString().ToLower() == "work")
                            {
                                PhoneWork = telecom.Value.ToString();
                            }
                            else if (telecom.Use.ToString().ToLower() == "mobile")
                            {
                                PhoneCell = telecom.Value.ToString();
                            }
                        }
                    }
                }

                if (loadPatient.Photo.Count > 0)
                {
                    foreach(var minutia in loadPatient.Photo)
                    {
                        Attachment mediaAttachment = loadPatient.Photo[0];
                        byte[] byteMinutias = mediaAttachment.Data;

                        Stream stream = new MemoryStream(byteMinutias);
                        Media media = (Media)FHIRUtilities.StreamToFHIR(new StreamReader(stream));

                        // Get captureSite and laterality from media
                        string captureSiteDescription = media.Extension[1].Value.Extension[1].Value.ToString();
                        string lateralityCode = media.Extension[1].Value.Extension[2].Value.ToString();
                        string DeviceName = media.Extension[2].Value.Extension[1].Value.ToString();
                        string OriginalDpi = media.Extension[2].Value.Extension[2].Value.ToString();
                        string OriginalHeight = media.Extension[2].Value.Extension[3].Value.ToString();
                        string OriginalWidth = media.Extension[2].Value.Extension[4].Value.ToString();

                        Template addMinutia = ConvertFHIR.FHIRToTemplate(media);
                        FingerPrintMinutias newFingerPrintMinutias = new FingerPrintMinutias(
                            SessionID, addMinutia, FHIRUtilities.SnoMedCodeToLaterality(lateralityCode), FHIRUtilities.StringToCaptureSite(captureSiteDescription));

                        AddFingerPrint(newFingerPrintMinutias, DeviceName, Int32.Parse(OriginalDpi), Int32.Parse(OriginalHeight), Int32.Parse(OriginalWidth));
                    }
                    _biometricsCaptured = GetBiometricsCaptured(); 
                }
            }
            else
            {
                throw new Exception("Error in PatientProfile constructor.  loadPatient is null.");
            }
        }

        public PatientProfile(Patient loadPatient, bool loadBiometrics = false)
        {
            NewSession();
            if (loadPatient != null)
            {
                _noID = new SourceAFIS.Templates.NoID();
                SetMetaData(loadPatient); //Sets all the data from the meta area of FHIR message
                SetIdentifierData(loadPatient); //Sets all the data from the identifier area of FHIR message

                // Gets the demographics from the patient FHIR resource class
                _lastName = loadPatient.Name[0].Family.ToString();
                List<string> givenNames = loadPatient.Name[0].Given.ToList();
                _firstName = givenNames[0].ToString();
                if (givenNames.Count > 1)
                {
                    _middleName = givenNames[1].ToString();
                }
                _gender = loadPatient.Gender.ToString().Substring(0, 1).ToUpper();
                _birthDate = loadPatient.BirthDate.ToString();

                // Gets the address information from the patient FHIR resource class
                if (loadPatient.Address.Count > 0)
                {
                    List<string> addressLines = loadPatient.Address[0].Line.ToList();
                    _streetAddress = addressLines[0].ToString();
                    if (addressLines.Count > 1)
                    {
                        _streetAddress2 = addressLines[1].ToString();
                    }

                    _city = loadPatient.Address[0].City.ToString();
                    _state = loadPatient.Address[0].State.ToString();
                    _postalCode = loadPatient.Address[0].PostalCode.ToString();
                    _country = loadPatient.Address[0].Country.ToString();
                }
                // Gets the contact information from the patient FHIR resource class
                if (loadPatient.Contact.Count > 0)
                {
                    foreach (var contact in loadPatient.Contact)
                    {
                        foreach (var telecom in contact.Telecom)
                        {
                            if (telecom.Use.ToString().ToLower() == "home")
                            {
                                if (telecom.System.ToString().ToLower() == "email")
                                {
                                    EmailAddress = telecom.Value.ToString();
                                }
                                else if (telecom.System.ToString().ToLower() == "phone")
                                {
                                    PhoneHome = telecom.Value.ToString();
                                }
                            }
                            else if (telecom.Use.ToString().ToLower() == "work")
                            {
                                PhoneWork = telecom.Value.ToString();
                            }
                            else if (telecom.Use.ToString().ToLower() == "mobile")
                            {
                                PhoneCell = telecom.Value.ToString();
                            }
                        }
                    }
                }

                if (loadBiometrics && loadPatient.Photo.Count > 0)
                {
                    foreach (var minutia in loadPatient.Photo)
                    {
                        Attachment mediaAttachment = loadPatient.Photo[0];
                        byte[] byteMinutias = mediaAttachment.Data;

                        Stream stream = new MemoryStream(byteMinutias);
                        Media media = (Media)FHIRUtilities.StreamToFHIR(new StreamReader(stream));

                        // Get captureSite and laterality from media
                        string captureSiteDescription = media.Extension[1].Value.Extension[1].Value.ToString();
                        string lateralityCode = media.Extension[1].Value.Extension[2].Value.ToString();
                        string DeviceName = media.Extension[2].Value.Extension[0].Value.ToString();
                        string OriginalDpi = media.Extension[2].Value.Extension[1].Value.ToString();
                        string OriginalHeight = media.Extension[2].Value.Extension[2].Value.ToString();
                        string OriginalWidth = media.Extension[2].Value.Extension[3].Value.ToString();

                        Template addMinutia = ConvertFHIR.FHIRToTemplate(media);
                        FingerPrintMinutias newFingerPrintMinutias = new FingerPrintMinutias(
                            SessionID, addMinutia, FHIRUtilities.SnoMedCodeToLaterality(lateralityCode), FHIRUtilities.StringToCaptureSite(captureSiteDescription));

                        AddFingerPrint(newFingerPrintMinutias, DeviceName, Int32.Parse(OriginalDpi), Int32.Parse(OriginalHeight), Int32.Parse(OriginalWidth));
                    }
                }
            }
            else
            {
                throw new Exception("Error in PatientProfile constructor.  loadPatient is null.");
            }
        }

        ~PatientProfile() { }

		#endregion

		public void Dispose()
		{
			_language = "";
			_firstName = "";
			_lastName = "";
			_middleName = "";
			_gender = ""; // F, M or O
            _genderChangeFlag = "";
            _birthDate = "";
			_streetAddress = "";
			_streetAddress2 = "";
			_city = "";
			_state = "";
			_country = "";
			_postalCode = "";
			_phoneHome = "";
			_phoneCell = "";
			_phoneWork = "";
			_emailAddress = "";
			_multipleBirth = ""; //Yes or No
			_noidStatus = ""; //pending, approved, denied
			_noidType = ""; //new, return, error, or critical
			_checkinDateTime = "";
			_noidHubName = "";
			_noidHubPassword = "";
            _biometricExceptionMissingReason = "";
            _secretQuestion1 = "";
            _secretAnswer1 = "";
            _secretQuestion2 = "";
            _secretAnswer2 = "";
			_localNoID = "";
			_sessionID = "";
			_remoteNoID = "";
			_biometricsCaptured = "";
			_clinicArea = "";
			_devicePhysicalLocation = "";
			_deviceStartTime = "";
		}

		void SetMetaData(Patient loadPatient)
        {
            if (loadPatient.Meta != null)
            {
                try
                {
                    foreach (Extension metaExtension in loadPatient.Meta.Extension)
                    {
                        if (metaExtension.Url.ToLower().Contains("status") == true)
                        {
                            foreach (Extension statusExtension in metaExtension.Extension)
                            {
                                if (statusExtension.Url.ToLower().Contains("status") == true)
                                {
                                    NoIDStatus = statusExtension.Value.ToString();
                                }
                                else if (statusExtension.Url.ToLower().Contains("type") == true)
                                {
                                    NoIDType = statusExtension.Value.ToString();
                                }
                                else if (statusExtension.Url.ToLower().Contains("checkin") == true)
                                {
                                    CheckinDateTime = statusExtension.Value.ToString();
                                }
                            }
                        }
                        else if (metaExtension.Url.ToLower().Contains("location") == true)
                        {
                            foreach (Extension locationExtension in metaExtension.Extension)
                            {
                                if (locationExtension.Url.ToLower().Contains("clinic") == true)
                                {
                                    ClinicArea = locationExtension.Value.ToString();
                                }
                                else if (locationExtension.Url.ToLower().Contains("physical") == true)
                                {
                                    DevicePhysicalLocation = locationExtension.Value.ToString();
                                }
                            }   
                        }
                        else if (metaExtension.Url.ToLower().Contains("capture") == true)
                        {
                            foreach (Extension locationExtension in metaExtension.Extension)
                            {
                                if (locationExtension.Url.ToLower().Contains("site") == true)
                                {
                                    BiometricsCaptured = locationExtension.Value.ToString();
                                }
                                else if (locationExtension.Url.ToLower().Contains("scanner") == true)
                                {
                                    //DeviceName = locationExtension.Value.ToString();
                                }
                                else if (locationExtension.Url.ToLower().Contains("dpi") == true)
                                {
                                    //OriginalDPI
                                }
                                else if (locationExtension.Url.ToLower().Contains("height") == true)
                                {
                                    //OriginalHeight
                                }
                                else if (locationExtension.Url.ToLower().Contains("width") == true)
                                {
                                    //OriginalWidth
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }

        void SetIdentifierData(Patient loadPatient)
        {
            // Gets the identifiers from the patient FHIR resource class
            if (loadPatient.Identifier != null)
            {
                foreach (Identifier id in loadPatient.Identifier)
                {
                    if (id.System.ToLower().Contains("session") == true)
                    {
                        SessionID = id.Value.ToString();
                    }
                    else if (id.System.ToLower().Contains("local") == true)
                    {
                        LocalNoID = id.Value.ToString();
                    }
                    else if (id.System.ToLower().Contains("remote") == true)
                    {
                        RemoteNoID = id.Value.ToString();
                    }
                }
            }
        }

        public void NewSession()
        {
            _noID = new SourceAFIS.Templates.NoID();
            _noID.SessionID = StringUtilities.SHA256(Guid.NewGuid().ToString());
            //TODO: Add domain name to session string.
            _sessionID = "noid://" + DomainName + "/" + _noID.SessionID;
        }

        [JsonIgnore]
        public Uri FHIRAddress
        {
            get { return _fhirAddress; }
            set { _fhirAddress = value; }
        }

        [JsonProperty("OrganizationName")]
        public string OrganizationName
        {
            get { return _organizationName; }
        }

        [JsonProperty("DomainName")]
        public string DomainName
        {
            get { return _domainName; }
        }

        [JsonProperty("LocalNoID")]
        public string LocalNoID
        {
            get { return _localNoID; }
            set { _localNoID = value; }
        }

        [JsonProperty("SessionID")]
        public string SessionID
        {
            get { return _sessionID; }
            set { _sessionID = value; }
        }

        [JsonProperty("RemoteNoID")]
        public string RemoteNoID
        {
            get {  return _remoteNoID; }
            set { _remoteNoID = value; } 
        }

        [JsonProperty("Language")]
        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        [JsonProperty("FirstName")]
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        [JsonProperty("LastName")]
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        [JsonProperty("MiddleName")]
        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; }
        }

        [JsonProperty("Gender")]
        public string Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

        [JsonProperty("GenderChangeFlag")]
        public string GenderChangeFlag
        {
            get { return _genderChangeFlag; }
            set { _genderChangeFlag = value; }
        }

        [JsonProperty("BirthDate")]
        public string BirthDate
        {
            get { return _birthDate; }
            set { _birthDate = value; }
        }

        [JsonProperty("StreetAddress")]
        public string StreetAddress
        {
            get { return _streetAddress; }
            set { _streetAddress = value; }
        }

        [JsonProperty("StreetAddress2")]
        public string StreetAddress2
        {
            get { return _streetAddress2; }
            set { _streetAddress2 = value; }
        }

        [JsonProperty("City")]
        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        [JsonProperty("State")]
        public string State
        {
            get { return _state; }
            set { _state = value; }
        }

        [JsonProperty("PostalCode")]
        public string PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }

        [JsonProperty("Country")]
        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        [JsonProperty("PhoneHome")]
        public string PhoneHome
        {
            get { return _phoneHome; }
            set { _phoneHome = value; }
        }

        [JsonProperty("PhoneCell")]
        public string PhoneCell
        {
            get { return _phoneCell; }
            set { _phoneCell = value; }
        }

        [JsonProperty("PhoneWork")]
        public string PhoneWork
        {
            get { return _phoneWork; }
            set { _phoneWork = value; }
        }

        [JsonProperty("EmailAddress")]
        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        [JsonProperty("MultipleBirth")]
        public string MultipleBirth
        {
            get { return _multipleBirth; }
            set { _multipleBirth = value; }
        }

        [JsonProperty("NoIDHubName")]
        public string NoIDHubName
        {
            get { return _noidHubName; }
            set { _noidHubName = value; }
        }

        [JsonProperty("NoIDHubPassword")]
        public string NoIDHubPassword
        {
            get { return _noidHubPassword; }
            set { _noidHubPassword = value; }
        }

        [JsonProperty("BiometricExceptionMissingReason")]
        public string BiometricExceptionMissingReason
        {
            get { return _biometricExceptionMissingReason; }
            set { _biometricExceptionMissingReason = value; }
        }

        [JsonProperty("SecretQuestion1")]
        public string SecretQuestion1
        {
            get { return _secretQuestion1; }
            set { _secretQuestion1 = value; }
        }

        [JsonProperty("SecretAnswer1")]
        public string SecretAnswer1
        {
            get { return _secretAnswer1; }
            set { _secretAnswer1 = value; }
        }

        [JsonProperty("SecretQuestion2")]
        public string SecretQuestion2
        {
            get { return _secretQuestion2; }
            set { _secretQuestion2 = value; }
        }

        [JsonProperty("SecretAnswer2")]
        public string SecretAnswer2
        {
            get { return _secretAnswer2; }
            set { _secretAnswer2 = value; }
        }

        /// <summary>
        ///   The clinic or area the kiosk/computer is used to create the FHIR profile. 
        /// </summary>
        [JsonProperty("ClinicArea")]
        public string ClinicArea
        {
            get { return _clinicArea; }
            set { _clinicArea = value; }
        }

        /// <summary>
        ///   physical location of the kiosk or computer is used to create the FHIR profile. 
        /// </summary>
        [JsonProperty("DevicePhysicalLocation")]
        public string DevicePhysicalLocation
        {
            get { return _devicePhysicalLocation; }
            set { _devicePhysicalLocation = value; }
        }

        /// <summary>
        ///   Status is pending, approved, denied, flagged.
        /// </summary>
        [JsonProperty("NoIDStatus")]
        public string NoIDStatus
        {
            get { return _noidStatus; }
            set { _noidStatus = value; }
        }

        /// <summary>
        ///   Type is new, return, hold
        /// </summary>
        [JsonProperty("NoIDType")]
        public string NoIDType
        {
            get { return _noidType; }
            set { _noidType = value; }
        }

        [JsonProperty("CheckinDateTime")]
        public string CheckinDateTime
        {
            get { return _checkinDateTime; }
            set { _checkinDateTime = value; }
        }

        [JsonProperty("DeviceStartTime")]
        public string DeviceStartTime
        {
            get { return _deviceStartTime; }
            set { _deviceStartTime = value; }
        }
        
        public string GetBiometricsCaptured()
        {
            string biometrics = "";

            if (_fingerPrintMinutiasList.Count > 0)
            {
                foreach (var finger in _fingerPrintMinutiasList)
                {
                    string fingerDesc = FixFingerDescriptions(FHIRUtilities.LateralityToString(finger.LateralitySnoMedCode) + " " + FHIRUtilities.CaptureSiteToString(finger.CaptureSiteSnoMedCode));
                    if (biometrics.Contains(fingerDesc) == false)
                    {
                        if (biometrics.Length > 0)
                        {
                            biometrics = biometrics + " and " + fingerDesc;
                        }
                        else
                        {
                            biometrics = fingerDesc;
                        }
                    }
                }
            }
            return biometrics;
        }

        string FixFingerDescriptions(string inputFinger)
        {
            return FHIRUtilities.FixEnumFingerDescriptions(inputFinger);
        }

        [JsonProperty("BiometricsCaptured")]
        public string BiometricsCaptured
        {
            get { return _biometricsCaptured; }
            set { _biometricsCaptured = value; }
        }

        [JsonIgnore]
        public SourceAFIS.Templates.NoID NoID
        {
            get { return _noID; }
            set
            {
                _noID = value;
                if (_noID != null)
                {
                    _localNoID = _noID.LocalNoID;
                    _sessionID = _noID.SessionID;
                    _remoteNoID = _noID.RemoteNoID;
                    _checkinDateTime = FHIRUtilities.DateTimeToFHIRString(_noID.SessionStartDateTime);
                }
            }
        }

        [JsonIgnore]
        public Exception BaseException
        {
            get { return _exception; }
            set { _exception = value; }
        }

        public bool AddFingerPrint(FingerPrintMinutias patientFingerprintMinutia, string deviceName, int originalDpi, int originalHeight, int originalWidth)
        {
            bool result = false;
            try
            {
                patientFingerprintMinutia.DeviceName = deviceName;
                patientFingerprintMinutia.OriginalDpi = originalDpi;
                patientFingerprintMinutia.OriginalHeight = originalHeight;
                patientFingerprintMinutia.OriginalWidth = originalWidth;
                FHIRUtilities.LateralitySnoMedCode lateralitySnoMedCode = patientFingerprintMinutia.LateralitySnoMedCode;
                FHIRUtilities.CaptureSiteSnoMedCode captureSiteSnoMedCode = patientFingerprintMinutia.CaptureSiteSnoMedCode;
                if (lateralitySnoMedCode == FHIRUtilities.LateralitySnoMedCode.Left || lateralitySnoMedCode == FHIRUtilities.LateralitySnoMedCode.Right)
                {
                    switch (captureSiteSnoMedCode)
                    {
                        case FHIRUtilities.CaptureSiteSnoMedCode.IndexFinger:
                        case FHIRUtilities.CaptureSiteSnoMedCode.MiddleFinger:
                        case FHIRUtilities.CaptureSiteSnoMedCode.RingFinger:
                        case FHIRUtilities.CaptureSiteSnoMedCode.LittleFinger:
                        case FHIRUtilities.CaptureSiteSnoMedCode.Thumb:
                            {
                                result = true;
                                _fingerPrintMinutiasList.Add(patientFingerprintMinutia);
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
            return result;
        }

        [JsonIgnore]
        public List<FingerPrintMinutias> FingerPrintMinutiasList
        {
            get { return _fingerPrintMinutiasList; }
            set { _fingerPrintMinutiasList = value; }
        }
    }

    public class PatientFHIRProfile : PatientProfile
    {
        public PatientFHIRProfile(string organizationName, string noidStatus) : base(organizationName, noidStatus)
        {
        }

        public PatientFHIRProfile(string organizationName, Uri endPoint, Patient loadPatient, string noidStatus, DateTime checkinDateTime) : base(organizationName, endPoint, loadPatient, noidStatus, checkinDateTime)
        {
        }

        ~PatientFHIRProfile() { }

        private string FingerTipSnoMedCTCode(FHIRUtilities.CaptureSiteSnoMedCode fingerTip)
        {
            return fingerTip.ToString();
        }

        private string SnoMedCTLaterality(FHIRUtilities.LateralitySnoMedCode laterality)
        {
            return laterality.ToString();
        }

        public Exception Exception
        {
            get { return BaseException; }
        }
        /*
        public string DomainName
        {
            get
            {
                try
                {
                    if (FHIRAddress != null)
                    {
                        return FHIRAddress.Host.Substring(FHIRAddress.Host.LastIndexOf('.', FHIRAddress.Host.LastIndexOf('.') - 1) + 1);
                    }
                    else
                    {
                        return "mynoid.com";
                    }
                }
                catch
                {
                    return "localhost";
                }
            }
        }
        */
        public string ServerName
        {
            get
            {
                if (FHIRAddress != null)
                {
                    return FHIRAddress.GetLeftPart(UriPartial.Authority).ToString();
                }
                else
                {
                    return "www.mynoid.com";
                }
            }
        }

        public Patient CreateFHIRPatientProfile()
        {
            Patient pt;
            try
            {
                /*

                _patientFHIRProfile.NoIDHubPassword = password;
                _patientFHIRProfile.NoIDHubName = patientHub;
                _patientFHIRProfile.GenderChangeFlag = genderChangeFlag;

                */
                pt = new Patient();

                BiometricsCaptured = GetBiometricsCaptured();
                // Add message status New, Return or Update
                Meta meta = new Meta();

                meta.Extension.Add(FHIRUtilities.MessageTypeExtension(NoIDStatus, NoIDType, CheckinDateTime));
                meta.Extension.Add(FHIRUtilities.ClinicLocationExtension(ClinicArea, DevicePhysicalLocation));
                if (FingerPrintMinutiasList.Count > 0)
                {
                    meta.Extension.Add(FHIRUtilities.CaptureSummaryExtension(BiometricsCaptured, 
                        FingerPrintMinutiasList[0].DeviceName,
                        FingerPrintMinutiasList[0].OriginalDpi,
                        FingerPrintMinutiasList[0].OriginalHeight,
                        FingerPrintMinutiasList[0].OriginalWidth)
                        );
                }
                meta.Extension.Add(FHIRUtilities.NoIDHubInfo(NoIDHubName, NoIDHubPassword));
                if (GenderChangeFlag.ToLower().Contains("yes") == true || MultipleBirth.ToLower().Contains("no") == false)
                {
                    meta.Extension.Add(FHIRUtilities.GenderAndTwinInfo(GenderChangeFlag, MultipleBirth));
                }

                pt.Meta = meta;

                //TODO: Move fingerprint minutias to Identifier.Extension
                Identifier idSession = new Identifier();
                idSession.System = ServerName + "/fhir/SessionID";
                if (SessionID == null || SessionID.Length == 0)
                {
                    NewSession();
                }
                idSession.Value = SessionID;
                pt.Identifier.Add(idSession);

                
                if (BiometricExceptionMissingReason.Length > 0)
                {
                    if (SecretQuestion1.Length > 0 && SecretAnswer1.Length > 0 && SecretQuestion2.Length > 0 && SecretAnswer2.Length > 0)
                    {
                        Identifier idBiometricException = new Identifier();
                        idBiometricException.System = ServerName + "/fhir/BiometricException";
                        idBiometricException.Value = "Biometric Exception Pathway";
                        Extension extBiometricException;
                        extBiometricException = FHIRUtilities.BiometricException(
                                BiometricExceptionMissingReason, 
                                SecretQuestion1, SecretAnswer1, 
                                SecretQuestion2, SecretAnswer2
                                );
                        idBiometricException.Extension.Add(extBiometricException);
                        pt.Identifier.Add(idBiometricException);
                    }
                    {
                        //throw an error back to the page.
                    }
                }

                ResourceReference managingOrganization = new ResourceReference(OrganizationName, DomainName);
                pt.ManagingOrganization = managingOrganization;

                // Add patient demographics
                pt.Language = Language;
                pt.BirthDate = BirthDate;
                if (Gender.ToLower() == "f")
                {
                    pt.Gender = AdministrativeGender.Female;
                }
                else if (Gender.ToLower() == "m")
                {
                    pt.Gender = AdministrativeGender.Male;
                }
                else
                {
                    pt.Gender = AdministrativeGender.Unknown;
                }
                
                if (!(MultipleBirth == null) && MultipleBirth.Length > 0)
                {
                    if (MultipleBirth.ToLower().Substring(0,2) == "no")
                    {
                        pt.MultipleBirth = new FhirString("No");
                    }
                    else if (MultipleBirth.ToLower() == "yes")
                    {
                        pt.MultipleBirth = new FhirString("Yes");
                    }
                }
                // Add patient name
                HumanName ptName = new HumanName();
                ptName.Given = new string[] { FirstName, MiddleName };
                ptName.Family = LastName;
                pt.Name = new List<HumanName> { ptName };
                
                if (StreetAddress.Length > 0 || City.Length > 0 || State.Length > 0 || PostalCode.Length > 0)
                {
                    Address address = new Address(); // Add patient address
                    address.Line = new string[] { StreetAddress, StreetAddress2 };
                    address.City = City;
                    address.State = State;
                    address.Country = Country;
                    address.PostalCode = PostalCode;
                    pt.Address.Add(address);
                }

                // Add patient contact information (phone numbers and email)
                // TODO: make sure email and phone are valid.
                // Validate phone with UI.
                Patient.ContactComponent contact = new Patient.ContactComponent();
                bool addContact = false;
                if ((EmailAddress != null) && EmailAddress.Length > 0)
                {
                    ContactPoint emailAddress = new ContactPoint(ContactPoint.ContactPointSystem.Email, ContactPoint.ContactPointUse.Home, EmailAddress);
                    contact.Telecom.Add(emailAddress);
                    addContact = true;
                }
                if ((PhoneHome != null) && PhoneHome.Length > 0)
                {
                    ContactPoint phoneHome = new ContactPoint(ContactPoint.ContactPointSystem.Phone, ContactPoint.ContactPointUse.Home, PhoneHome);
                    contact.Telecom.Add(phoneHome);
                    addContact = true;
                }
                if ((PhoneCell != null) && PhoneCell.Length > 0)
                {
                    ContactPoint phoneCell = new ContactPoint(ContactPoint.ContactPointSystem.Phone, ContactPoint.ContactPointUse.Mobile, PhoneCell);
                    contact.Telecom.Add(phoneCell);
                    addContact = true;
                }
                if ((PhoneWork != null) && PhoneWork.Length > 0)
                {
                    ContactPoint phoneWork = new ContactPoint(ContactPoint.ContactPointSystem.Phone, ContactPoint.ContactPointUse.Work, PhoneWork);
                    contact.Telecom.Add(phoneWork);
                    addContact = true;
                }
                if (addContact)
                {
                    pt.Contact.Add(contact);
                }
                //TODO: Change location of minutias in Patient FHIR profile from attached Photo.Extension to Identity.Extension
                foreach (FingerPrintMinutias minutias in FingerPrintMinutiasList)
                {
                    Attachment attach = new Attachment();
                    Media fingerprintMedia = FingerPrintFHIRMedia(minutias, minutias.DeviceName, minutias.OriginalDpi, minutias.OriginalHeight, minutias.OriginalWidth);
                    byte[] mediaBytes = FhirSerializer.SerializeToJsonBytes(fingerprintMedia, summary: SummaryType.Data);
                    attach.Data = mediaBytes;
                    pt.Photo.Add(attach);
                }
            }
            catch (Exception ex)
            {
                BaseException = new Exception("Error in PatientProfile::CreateFHIRProfile(). Failed to create a new FHIR profile: " + ex.Message);
                return null;
            }
            return pt;
        }

        private CodeableConcept GetBodySite(FHIRUtilities.CaptureSiteSnoMedCode captureSite, FHIRUtilities.LateralitySnoMedCode laterality)
        {
            int captureSiteCode = (int)captureSite;
            int lateralityCode = (int)laterality;
            CodeableConcept bodyCaptureSite = new CodeableConcept("SNOMED", captureSiteCode.ToString(), FHIRUtilities.CaptureSiteToString(captureSite));
            Extension extLaterality = new Extension(lateralityCode.ToString(), new FhirString(FHIRUtilities.LateralityToString(laterality)));
            bodyCaptureSite.AddExtension("Laterality", extLaterality);
            return bodyCaptureSite;
        }

        public Media FingerPrintFHIRMedia(FingerPrintMinutias fingerPrints, string deviceName, int originalDPI, int originalHeight, int originalWidth)
        {
            Media FingerPrintMedia = null;
            
            try
            {
                if ((fingerPrints != null))
                {
                    //TODO: add capture date/time to message
                    FingerPrintMedia = new Media(); //Creates the fingerprint minutia template FHIR object as media type.
                    FingerPrintMedia.AddExtension("Healthcare Node", FHIRUtilities.OrganizationExtension(OrganizationName, DomainName, ServerName));
                    FingerPrintMedia.AddExtension("Biometic Capture", FHIRUtilities.CaptureSiteExtension(fingerPrints.CaptureSiteSnoMedCode, fingerPrints.LateralitySnoMedCode, deviceName, originalDPI, originalHeight, originalWidth));
                    
                    FingerPrintMedia.Identifier = new List<Identifier>();

                    Identifier idSession;
                    Identifier idPatientCertificate;
                    if ((SessionID != null))
                    {
                        if (SessionID.Length > 0)
                        {
                            idSession = new Identifier();
                            idSession.System = ServerName + "/fhir/SessionID";
                            idSession.Value = SessionID;
                            FingerPrintMedia.Identifier.Add(idSession);
                        }
                        else
                        {
                            //TODO this is a critical error.  all need a unique session id.
                        }
                    }
                    else
                    {
                        //TODO this is critical.  all need a unique session id.
                    }
                    if (LocalNoID != null)
                    {
                        if (LocalNoID.Length > 0)
                        {
                            idPatientCertificate = new Identifier();
                            idPatientCertificate.System = ServerName + "/fhir/LocalNoID";
                            idPatientCertificate.Value = LocalNoID;
                            FingerPrintMedia.Identifier.Add(idPatientCertificate);
                        }
                    }
                    // TODO: Add RemoteNoID.

                    foreach (var Minutia in fingerPrints.Minutiae)
                    {
                        if (FingerPrintMedia is null)
                        {
                            FingerPrintMedia = new Media();
                        }

                        Extension extFingerPrintMedia = FHIRUtilities.FingerPrintMediaExtension(
                            Minutia.PositionX.ToString(),
                            Minutia.PositionY.ToString(),
                            Minutia.Direction.ToString(),
                            Minutia.Type.ToString()
                        );
                        
                        FingerPrintMedia.AddExtension("Biometrics", extFingerPrintMedia);
                    }
                }
            }
            catch (Exception ex)
            {
                BaseException = ex;
            }
            return FingerPrintMedia;
        }
        
        public bool TestEnrollmentSave()
        {
            Uri endpoint = FHIRAddress;
            FhirClient client = new FhirClient(endpoint);
            Patient newPatient = CreateFHIRPatientProfile();
            
            if (!(newPatient is null))
            {
                try
                {
                    client.Create(newPatient);
                }
                catch (Exception ex)
                {
                    BaseException = ex;
                    return false;
                }
            }
            return true;
        }
    }
}
