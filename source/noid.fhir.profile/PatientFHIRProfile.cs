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
        private List<FingerPrintMinutias> _fingerPrintMinutiasList = new List<FingerPrintMinutias>();
        private Exception _exception;

        private string _language = "";
        private string _firstName = "";
        private string _lastName = "";
        private string _middleName = "";
        private string _gender = ""; // F, M or O
        private string _birthDate = "";
        private string _streetAddress = "";
        private string _streetAddress2;
        private string _city = "";
        private string _state = "";
        private string _country = "";
        private string _postalCode = "";
        private string _phoneHome = "";
        private string _phoneCell = "";
        private string _phoneWork = "";
        private string _emailAddress = "";
        private string _multipleBirth = ""; //Yes or No
        private string _noidStatus = ""; //new, return, error or critical
        private string _checkinDateTime = "";
        private string _noidHubName = "";
        private string _noidHubPassword = "";
        private string _biometricAlternateReason = "";
        private string _biometricAlternateQuestion1 = "";
        private string _biometricAlternateAnswer1 = "";
        private string _biometricAlternateQuestion2 = "";
        private string _biometricAlternateAnswer2 = "";


        [JsonConstructor]
        public PatientProfile()
        {
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

            if (loadPatient != null)
            {
                _noID = new SourceAFIS.Templates.NoID();
                // Gets the identifiers from the patient FHIR resource class
                if (loadPatient.Identifier.Count > 0)
                {
                    Identifier identifier = loadPatient.Identifier[0];

                    if (identifier.System.ToString().ToLower().Contains("sessionid") == true)
                    {
                        _noID.SessionID = identifier.Value.ToString();
                    }
                    else if (identifier.System.ToString().ToLower().Contains("local") == true)
                    {
                        _noID.LocalNoID = identifier.Value.ToString();
                    }
                    else if (identifier.System.ToString().ToLower().Contains("remote") == true)
                    {
                        _noID.RemoteNoID = identifier.Value.ToString();
                    }

                    if (loadPatient.Identifier.Count > 1)
                    {
                        identifier = loadPatient.Identifier[1];
                        if (identifier.System.ToString().ToLower().Contains("sessionid") == true)
                        {
                            _noID.SessionID = identifier.Value.ToString();
                        }
                        else if (identifier.System.ToString().ToLower().Contains("local") == true)
                        {
                            _noID.LocalNoID = identifier.Value.ToString();
                        }
                        else if (identifier.System.ToString().ToLower().Contains("remote") == true)
                        {
                            _noID.RemoteNoID = identifier.Value.ToString();
                        }
                    }
                }
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
                        string captureSiteCode = media.Extension[1].Value.Extension[1].Value.ToString();
                        string lateralityCode = media.Extension[1].Value.Extension[2].Value.ToString();

                        Template addMinutia = ConvertFHIR.FHIRToTemplate(media);
                        FingerPrintMinutias newFingerPrintMinutias = new FingerPrintMinutias(SessionID, addMinutia, FHIRUtilities.SnoMedCodeToLaterality(lateralityCode), FHIRUtilities.SnoMedCodeToCaptureSite(captureSiteCode));

                        AddFingerPrint(newFingerPrintMinutias);
                    }
                }
            }
            else
            {
                throw new Exception("Error in PatientProfile constructor.  loadPatient is null.");
            }
        }

        public PatientProfile(Patient loadPatient, bool loadBiometrics = false)
        {
            if (loadPatient != null)
            {
                _noID = new SourceAFIS.Templates.NoID();
                // Gets the identifiers from the patient FHIR resource class
                if (loadPatient.Identifier.Count > 0)
                {
                    Identifier identifier = loadPatient.Identifier[0];

                    if (identifier.System.ToString().ToLower().Contains("sessionid") == true)
                    {
                        _noID.SessionID = identifier.Value.ToString();
                    }
                    else if (identifier.System.ToString().ToLower().Contains("local") == true)
                    {
                        _noID.LocalNoID = identifier.Value.ToString();
                    }
                    else if (identifier.System.ToString().ToLower().Contains("remote") == true)
                    {
                        _noID.RemoteNoID = identifier.Value.ToString();
                    }

                    if (loadPatient.Identifier.Count > 1)
                    {
                        identifier = loadPatient.Identifier[1];
                        if (identifier.System.ToString().ToLower().Contains("sessionid") == true)
                        {
                            _noID.SessionID = identifier.Value.ToString();
                        }
                        else if (identifier.System.ToString().ToLower().Contains("local") == true)
                        {
                            _noID.LocalNoID = identifier.Value.ToString();
                        }
                        else if (identifier.System.ToString().ToLower().Contains("remote") == true)
                        {
                            _noID.RemoteNoID = identifier.Value.ToString();
                        }
                    }
                }
                if (_noID.LocalNoID.Length == 0)
                {
                    _noID.LocalNoID = loadPatient.Id;
                }

                Meta meta = loadPatient.Meta;
                if (meta != null)
                {
                    if (meta.LastUpdated != null)
                    {
                        CheckinDateTime = meta.LastUpdated.ToString();
                    }
                    if (meta.Extension.Count > 0)
                    {
                        Extension ext = meta.Extension[0];
                        NoIDStatus = ext.Value.ToString();
                    }
                }

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
                        string captureSiteCode = media.Extension[1].Value.Extension[1].Value.ToString();
                        string lateralityCode = media.Extension[1].Value.Extension[2].Value.ToString();

                        Template addMinutia = ConvertFHIR.FHIRToTemplate(media);
                        FingerPrintMinutias newFingerPrintMinutias = new FingerPrintMinutias(SessionID, addMinutia, FHIRUtilities.SnoMedCodeToLaterality(lateralityCode), FHIRUtilities.SnoMedCodeToCaptureSite(captureSiteCode));

                        AddFingerPrint(newFingerPrintMinutias);
                    }
                }
            }
            else
            {
                throw new Exception("Error in PatientProfile constructor.  loadPatient is null.");
            }
        }

        ~PatientProfile() { }

        public void NewSession()
        {
            _noID = new SourceAFIS.Templates.NoID();
            _noID.SessionID = StringUtilities.SHA256(Guid.NewGuid().ToString());
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

        [JsonProperty("PatientCertificateID")]
        public string PatientCertificateID
        {
            get { if (_noID != null) { return _noID.LocalNoID; } else { return ""; } }
            set { if (_noID != null) { _noID.LocalNoID = value; } }
        }

        [JsonProperty("SessionID")]
        public string SessionID
        {
            get { if (_noID != null) { return _noID.SessionID; } else { return ""; } }
            private set { if (_noID != null) { _noID.SessionID = value; } }
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

        [JsonProperty("BiometricAlternateReason")]
        public string BiometricAlternateReason
        {
            get { return _biometricAlternateReason; }
            set { _biometricAlternateReason = value; }
        }

        [JsonProperty("BiometricAlternateQuestion1")]
        public string BiometricAlternateQuestion1
        {
            get { return _biometricAlternateQuestion1; }
            set { _biometricAlternateQuestion1 = value; }
        }

        [JsonProperty("BiometricAlternateAnswer1")]
        public string BiometricAlternateAnswer1
        {
            get { return _biometricAlternateAnswer1; }
            set { _biometricAlternateAnswer1 = value; }
        }

        [JsonProperty("BiometricAlternateQuestion2")]
        public string BiometricAlternateQuestion2
        {
            get { return _biometricAlternateQuestion2; }
            set { _biometricAlternateQuestion2 = value; }
        }

        [JsonProperty("BiometricAlternateAnswer2")]
        public string BiometricAlternateAnswer2
        {
            get { return _biometricAlternateAnswer2; }
            set { _biometricAlternateAnswer2 = value; }
        }

        [JsonProperty("NoIDStatus")]
        public string NoIDStatus
        {
            get { return _noidStatus; }
            set { _noidStatus = value; }
        }

        [JsonProperty("CheckinDateTime")]
        public string CheckinDateTime
        {
            get { return _checkinDateTime; }
            set { _checkinDateTime = value; }
        }


        [JsonIgnore]
        public SourceAFIS.Templates.NoID NoID
        {
            get { return _noID; }
            set { _noID = value; }
        }

        [JsonIgnore]
        public Exception BaseException
        {
            get { return _exception; }
            set { _exception = value; }
        }

        public bool AddFingerPrint(FingerPrintMinutias patientFingerprintMinutia)
        {
            bool result = false;
            try
            {
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
        public string DeviceName;
        public int OriginalDpi;
        public int OriginalHeight;
        public int OriginalWidth;

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

        public string DomainName
        {
            get
            {
                try
                {
                    return FHIRAddress.Host.Substring(FHIRAddress.Host.LastIndexOf('.', FHIRAddress.Host.LastIndexOf('.') - 1) + 1);
                }
                catch
                {
                    return "localhost";
                }
            }
        }

        public string ServerName
        {
            get
            {
                return FHIRAddress.GetLeftPart(UriPartial.Authority).ToString();
            }
        }

        public Patient CreateFHIRPatientProfile()
        {
            Patient pt;
            try
            {
                pt = new Patient();
                // Add message status New, Return or Update
                Meta meta = new Meta();
                meta.Extension.Add(FHIRUtilities.MessageTypeExtension(NoIDStatus));
                pt.Meta = meta;
                // Add patient certificate hash.
                Identifier idSession;
                Identifier idPatientCertificate;
                if (SessionID != null && SessionID.Length > 0)
                {
                    idSession = new Identifier();
                    idSession.System = ServerName + "/fhir/SessionID";
                    idSession.Value = SessionID;
                    pt.Identifier.Add(idSession);
                }
                if (PatientCertificateID != null && PatientCertificateID.Length > 0)
                {
                    idPatientCertificate = new Identifier();
                    idPatientCertificate.System = ServerName + "/fhir/PatientCertificateID";
                    idPatientCertificate.Value = PatientCertificateID;
                    pt.Identifier.Add(idPatientCertificate);
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
                    if (MultipleBirth.ToLower() == "no")
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
                //TODO: Change location of minutias in Patient FHIR profile from attached photo to a more appropriate location.
                foreach (FingerPrintMinutias minutias in FingerPrintMinutiasList)
                {
                    Attachment attach = new Attachment();
                    Media fingerprintMedia = FingerPrintFHIRMedia(minutias, DeviceName, OriginalDpi, OriginalHeight, OriginalWidth);
                    byte[] mediaBytes = FhirSerializer.SerializeToJsonBytes(fingerprintMedia, summary: SummaryType.Data);
                    attach.Data = mediaBytes;
                    pt.Photo.Add(attach);
                }
            }
            catch (Exception ex)
            {
                BaseException = new Exception("PatientProfile.CreateFHIRProfile() failed to create a new profile: " + ex.Message);
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
            DeviceName = deviceName;
            OriginalDpi = originalDPI;
            OriginalHeight = originalHeight;
            OriginalWidth = originalWidth;

            try
            {
                if ((fingerPrints != null))
                {
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
                    if (PatientCertificateID != null)
                    {
                        if (PatientCertificateID.Length > 0)
                        {
                            idPatientCertificate = new Identifier();
                            idPatientCertificate.System = ServerName + "/fhir/LocalNoID";
                            idPatientCertificate.Value = PatientCertificateID;
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
