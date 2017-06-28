﻿// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Linq;
using System.Collections.Generic;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using NoID.Utilities;
using Hl7.Fhir.Serialization;

namespace NoID.FHIR.Profile
{

    /// <summary cref="PatientFHIRProfileSerialize">  
    /// Lightweight NoID Patient FHIR profile
    /// </summary>  

    public class PatientProfile
    {
        private SourceAFIS.Templates.NoID _noID;
        private readonly string _organizationName;
        private readonly string _fhirAddress;

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

        public PatientProfile(string organizationName, string fhirAddress)
        {
            _organizationName = organizationName;
            _fhirAddress = fhirAddress;
            _noID = new SourceAFIS.Templates.NoID();
        }

        public PatientProfile(string organizationName, string fhirAddress, Patient loadPatient, string noidStatus, DateTime checkinDateTime)
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

        public string FHIRAddress
        {
            get { return _fhirAddress; }
        }

        public string OrganizationName
        {
            get { return _organizationName; }
        }

        public string PatientCertificateID
        {
            get { return _noID.LocalNoID; }
            set { _noID.LocalNoID = value; }
        }

        public string SessionID
        {
            get { return _noID.SessionID; }
            private set { _noID.SessionID = value; }
        }

        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; }
        }

        public string Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

        public string BirthDate
        {
            get { return _birthDate; }
            set { _birthDate = value; }
        }

        public string StreetAddress
        {
            get { return _streetAddress; }
            set { _streetAddress = value; }
        }

        public string StreetAddress2
        {
            get { return _streetAddress2; }
            set { _streetAddress2 = value; }
        }

        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        public string State
        {
            get { return _state; }
            set { _state = value; }
        }

        public string PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }

        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        public string PhoneHome
        {
            get { return _phoneHome; }
            set { _phoneHome = value; }
        }

        public string PhoneCell
        {
            get { return _phoneCell; }
            set { _phoneCell = value; }
        }

        public string PhoneWork
        {
            get { return _phoneWork; }
            set { _phoneWork = value; }
        }

        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        public string MultipleBirth
        {
            get { return _multipleBirth; }
            set { _multipleBirth = value; }
        }

        public string NoIDStatus
        {
            get { return _noidStatus; }
            set { _noidStatus = value; }
        }

        public string CheckinDateTime
        {
            get { return _checkinDateTime; }
            set { _checkinDateTime = value; }
        }
    }

    public class PatientFHIRProfile : PatientProfile
    {
        private Exception _exception;
        private List <FingerPrintMinutias> _fingerPrintMinutiasList = new List<FingerPrintMinutias>();

        public string DeviceName;
        public int OriginalDpi;
        public int OriginalHeight;
        public int OriginalWidth;

        public PatientFHIRProfile(string organizationName, string endPoint) : base(organizationName, endPoint)
        {
        }

        public PatientFHIRProfile(string organizationName, string endPoint, Patient loadPatient, string noidStatus, DateTime checkinDateTime) : base(organizationName, endPoint, loadPatient, noidStatus, checkinDateTime)
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
            get { return _exception; }
        }

        public string DomainName
        {
            get
            {
                try
                {
                    Uri fhirURI = new Uri(FHIRAddress);
                    return fhirURI.Host.Substring(fhirURI.Host.LastIndexOf('.', fhirURI.Host.LastIndexOf('.') - 1) + 1);
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
                Uri fhirURI = new Uri(FHIRAddress);
                return fhirURI.GetLeftPart(UriPartial.Authority).ToString();
            }
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

        public Patient CreateFHIRPatientProfile()
        {
            Patient pt;
            try
            {
                pt = new Patient();

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
                // Add patient address
                Address address = new Address();
                address.Line = new string[] { StreetAddress, StreetAddress2 };
                address.City = City;
                address.State = State;
                address.Country = Country;
                address.PostalCode = PostalCode;
                pt.Address.Add(address);

                // Add patient contact information (phone numbers and email)
                // TODO: make sure email and phone are valid.
                // Validate phone with UI.
                Patient.ContactComponent contact = new Patient.ContactComponent();
                bool addContact = false;
                if (!(EmailAddress != null) && EmailAddress.Length > 0)
                {
                    ContactPoint emailAddress = new ContactPoint(ContactPoint.ContactPointSystem.Email, ContactPoint.ContactPointUse.Home, EmailAddress);
                    contact.Telecom.Add(emailAddress);
                    addContact = true;
                }
                if (!(PhoneHome != null) && PhoneHome.Length > 0)
                {
                    ContactPoint phoneHome = new ContactPoint(ContactPoint.ContactPointSystem.Phone, ContactPoint.ContactPointUse.Home, PhoneHome);
                    contact.Telecom.Add(phoneHome);
                    addContact = true;
                }
                if (!(PhoneCell != null) && PhoneCell.Length > 0)
                {
                    ContactPoint phoneCell = new ContactPoint(ContactPoint.ContactPointSystem.Phone, ContactPoint.ContactPointUse.Mobile, PhoneCell);
                    contact.Telecom.Add(phoneCell);
                    addContact = true;
                }
                if (!(PhoneWork != null) && PhoneWork.Length > 0)
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
                foreach (FingerPrintMinutias minutias in _fingerPrintMinutiasList)
                {
                    Attachment attach = new Attachment();
                    Media fingerprintMedia = FingerPrintFHIRMedia(minutias, DeviceName, OriginalDpi, OriginalHeight, OriginalWidth);
                    byte[] mediaBytes = FhirSerializer.SerializeToJsonBytes(fingerprintMedia, summary: SummaryType.False);
                    attach.Data = mediaBytes;
                    pt.Photo.Add(attach);
                }
            }
            catch (Exception ex)
            {
                _exception = new Exception("PatientProfile.CreateFHIRProfile() failed to create a new profile: " + ex.Message);
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
                _exception = ex;
            }
            return FingerPrintMedia;
        }
        
        public bool TestEnrollmentSave()
        {
            Uri fhirURI = new Uri(FHIRAddress);
            FhirClient client = new FhirClient(fhirURI);
            Patient newPatient = CreateFHIRPatientProfile();
            
            if (!(newPatient is null))
            {
                try
                {
                    client.Create(newPatient);
                }
                catch (Exception ex)
                {
                    _exception = ex;
                    return false;
                }
            }
            return true;
        }
    }
}
