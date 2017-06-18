// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using NoID.Cryptographic.Hash;

namespace NoID.FHIR.Profile
{
    public class PatientFHIRProfile
    {
        

        //TODO: add right and left feet
        public enum CaptureSiteSnoMedCode : uint
        {
            IndexFinger = 48856004, // SnoMedCT Description: Skin structure of palmar surface of index finger
            MiddleFinger = 65271000, // SnoMedCT Description: Skin structure of palmar surface of middle finger
            RingFinger = 28404007, // SnoMedCT Description: Skin structure of palmar surface of ring finger
            LittleFinger = 43825009, // SnoMedCT Description: Skin structure of palmar surface of little finger
            Thumb = 72331001 // SnoMedCT Description: Skin structure of palmar surface of thumb
        }

        public enum LateralitySnoMedCode : uint
        {
            Left = 419161000, // SnoMedCT Description: Unilateral left
            Right = 419165000, //  SnoMedCT Description: Unilateral right
            Bilateral = 51440002 // SnoMedCT Description: Bilateral
        }

        // Argon2 Hash parameters. 
        // Currently a protocol level setting for now but could be a setting defined per healthcare and/or match hub.
        // TODO: increase the difficulty of the hash parameters after the prototype.
        const int ARGON2_TIME_COST = 1;
        const int ARGON2_MEMORY_COST = 250;
        const int ARGON2_PARALLEL_LANES = 4;

        HashWriter.ArgonParams argonParams = new HashWriter.ArgonParams(ARGON2_TIME_COST, ARGON2_MEMORY_COST, ARGON2_PARALLEL_LANES);

        //TODO: load and use saltList from matching hubs
        private string hashSalt = "C560325F";

        private string FingerTipSnoMedCTCode(CaptureSiteSnoMedCode fingerTip)
        {
            return fingerTip.ToString();
        }

        private string SnoMedCTLaterality(LateralitySnoMedCode laterality)
        {
            return laterality.ToString();
        }

        public PatientFHIRProfile(string organizationName, Uri fhirAddress)
        {
            _fhirAddress = fhirAddress;
            _organizationName = organizationName;
            NewSession();
        }

        private readonly Uri _fhirAddress;
        private Exception _exception;

        private string _organizationName = "";
        private string _language = "";
        private string _firstName = "";
        private string _lastName = "";
        private string _middleName = "";
        private AdministrativeGender _gender;
        private string _birthDay = "";
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
        private bool   _twinIndicator = false;
        private Element _multipleBirth;
        private string _patientCertificateID;
        private string _sessionID;

        //TODO: store captured finger SnoMedCT code with minutias
        private FingerPrintMinutias _leftFingerPrints;
        private FingerPrintMinutias _rightFingerPrints;
        private FingerPrintMinutias _leftAlternateFingerPrints;
        private FingerPrintMinutias _rightAlternateFingerPrints;

        public Uri FHIRAddress
        {
            get { return _fhirAddress; }
        }

        public string OrganizationName
        {
            get { return _organizationName; }
            set { _organizationName = value; }
        }
        public Exception Exception
        {
            get { return _exception; }
        }

        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }
        public string LanguageHash
        {
            get { return HashWriter.Hash(_language, hashSalt, argonParams); }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        public string FirstNameHash
        {
            get { return HashWriter.Hash(_firstName, hashSalt, argonParams); }
        }

        public string LastName
        {
            get { return _lastName;}
            set { _lastName = value; }
        }

        public string LastNameHash
        {
            get { return HashWriter.Hash(_lastName, hashSalt, argonParams); }
        }

        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; }
        }
        public string MiddleNameHash
        {
            get { return HashWriter.Hash(_middleName, hashSalt, argonParams); }
        }
        public AdministrativeGender Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }
        public string GenderHash
        {
            get { return HashWriter.Hash(_gender.ToString(), hashSalt, argonParams); }
        }
        public string BirthDay
        {
            get { return _birthDay; }
            set { _birthDay = value; }
        }
        public string BirthDayHash
        {
            get { return HashWriter.Hash(_birthDay, hashSalt, argonParams); }
        }

        public string StreetAddress
        {
            get { return _streetAddress; }
            set { _streetAddress = value; }
        }

        public string StreetAddressHash
        {
            get { return HashWriter.Hash(_streetAddress, hashSalt, argonParams); }
        }

        public string StreetAddress2
        {
            get { return _streetAddress2; }
            set { _streetAddress2 = value; }
        }

        public string StreetAddress2Hash
        {
            get { return HashWriter.Hash(_streetAddress2, hashSalt, argonParams); }
        }

        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        public string CityHash
        {
            get { return HashWriter.Hash(_city, hashSalt, argonParams); }
        }

        public string State
        {
            get { return _state; }
            set { _state = value; }
        }

        public string StateHash
        {
            get { return HashWriter.Hash(_state, hashSalt, argonParams); }
        }

        public string PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }

        public string PostalCodeHash
        {
            get { return HashWriter.Hash(_postalCode, hashSalt, argonParams); }
        }

        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        public string CountryHash
        {
            get { return HashWriter.Hash(_country, hashSalt, argonParams); }
        }

        public string PhoneHome
        {
            get { return _phoneHome; }
            set { _phoneHome = value; }
        }

        public string PhoneHomeHash
        {
            get { return HashWriter.Hash(_phoneHome, hashSalt, argonParams); }
        }

        public string PhoneCell
        {
            get { return _phoneCell; }
            set { _phoneCell = value; }
        }

        public string PhoneCellHash
        {
            get { return HashWriter.Hash(_phoneCell, hashSalt, argonParams); }
        }

        public string PhoneWork
        {
            get { return _phoneWork; }
            set { _phoneWork = value; }
        }

        public string PhoneWorkHash
        {
            get { return HashWriter.Hash(_phoneWork, hashSalt, argonParams); }
        }

        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        public string EmailAddressHash
        {
            get { return HashWriter.Hash(_emailAddress, hashSalt, argonParams); }
        }

        public bool TwinIndicator
        {
            get { return _twinIndicator; }
            set { _twinIndicator = value; }
        }

        public string TwinIndicatorHash
        {
            get { return HashWriter.Hash(_twinIndicator.ToString(), hashSalt, argonParams); }
        }

        public Element MultipleBirth
        {
            get { return _multipleBirth; }
            set { _multipleBirth = value; }
        }

        public string MultipleBirthHash
        {
            get { return HashWriter.Hash(_multipleBirth.ToString(), hashSalt, argonParams); }
        }

        public string PatientCertificateID
        {
            get { return _patientCertificateID; }
            set { _patientCertificateID = value; }
        }

        public string PatientCertificateIDHash
        {
            get { return HashWriter.Hash(_patientCertificateID, hashSalt, argonParams); }
        }

        public FingerPrintMinutias LeftFingerPrints
        {
            get { return _leftFingerPrints; }
            set { _leftFingerPrints = value; }
        }

        public string LeftFingerPrintHash
        {
            get { return HashWriter.Hash(_leftFingerPrints.ToString(), hashSalt, argonParams); }
        }

        public FingerPrintMinutias RightFingerPrints
        {
            get { return _rightFingerPrints; }
            set { _rightFingerPrints = value; }
        }

        public string RightFingerPrintHash
        {
            get { return HashWriter.Hash(_rightFingerPrints.ToString(), hashSalt, argonParams); }
        }

        public FingerPrintMinutias LeftAlternateFingerPrints
        {
            get { return _leftAlternateFingerPrints; }
            set { _leftAlternateFingerPrints = value; }
        }

        public string LeftAlternateFingerPrintsHash
        {
            get { return HashWriter.Hash(_leftAlternateFingerPrints.ToString(), hashSalt, argonParams); }
        }

        public FingerPrintMinutias RightAlternateFingerPrints
        {
            get { return _rightAlternateFingerPrints; }
            set { _rightAlternateFingerPrints = value; }
        }

        public string RightAlternateFingerPrintsHash
        {
            get { return HashWriter.Hash(_rightAlternateFingerPrints.ToString(), hashSalt, argonParams); }
        }

        public void NewSession()
        {
            _sessionID = HashWriter.Hash(Guid.NewGuid().ToString(), hashSalt, argonParams);
        }

        public bool AddFingerPrint(FingerPrintMinutias patientFingerprintMinutia)
        {
            try   
            {
                LateralitySnoMedCode lateralitySnoMedCode = patientFingerprintMinutia.LateralitySnoMedCode;
                CaptureSiteSnoMedCode captureSiteSnoMedCode = patientFingerprintMinutia.CaptureSiteSnoMedCode;

                switch (captureSiteSnoMedCode)
                {
                    case CaptureSiteSnoMedCode.IndexFinger:
                        {
                            if (lateralitySnoMedCode == LateralitySnoMedCode.Left)
                            {
                                _leftFingerPrints = patientFingerprintMinutia;
                            }
                            else if (lateralitySnoMedCode == LateralitySnoMedCode.Right)
                            {
                                _rightFingerPrints = patientFingerprintMinutia;
                            }
                            break;
                        }
                    case CaptureSiteSnoMedCode.MiddleFinger:
                    case CaptureSiteSnoMedCode.RingFinger:
                    case CaptureSiteSnoMedCode.LittleFinger:
                    case CaptureSiteSnoMedCode.Thumb:
                        {
                            if (lateralitySnoMedCode == LateralitySnoMedCode.Left)
                            {
                                _leftAlternateFingerPrints = patientFingerprintMinutia;
                            }
                            else if (lateralitySnoMedCode == LateralitySnoMedCode.Right)
                            {
                                _rightAlternateFingerPrints = patientFingerprintMinutia;
                            }
                            break;
                        }
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
                return false;
            }
            return true;
        }

        public string PatientCertificateHash
        {
            get { return HashWriter.Hash(_patientCertificateID.ToString(), hashSalt, argonParams); }
        }
        public string SessionID
        {
            get { return _sessionID; }
        }

        public Patient CreateFHIRPatientProfile()
        {
            Patient pt;
            try
            {
                pt = new Patient();
                // Add patient certificate hash.
                Identifier id = new Identifier();
                id.Value = PatientCertificateID; //hash of local patient certificate
                pt.Identifier = new List<Identifier> { id };
                // Add healthcare node certificate hash.
                ResourceReference managingOrganization = new ResourceReference("1.2.1.3.4", OrganizationName);
                pt.ManagingOrganization = managingOrganization;
                pt.ManagingOrganization.Identifier = new Identifier("", PatientCertificateHash);
                // Add patient demographics
                pt.Language = Language;
                pt.BirthDate = BirthDay;
                pt.Gender = Gender;
                pt.MultipleBirth = MultipleBirth;
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
                if (!(EmailAddress is null) && EmailAddress.Length > 0)
                {
                    ContactPoint emailAddress = new ContactPoint(ContactPoint.ContactPointSystem.Email, ContactPoint.ContactPointUse.Home, EmailAddress);
                    contact.Telecom.Add(emailAddress);
                    addContact = true;
                }
                if (!(PhoneHome is null) && PhoneHome.Length > 0)
                {
                    ContactPoint phoneHome = new ContactPoint(ContactPoint.ContactPointSystem.Phone, ContactPoint.ContactPointUse.Home, PhoneHome);
                    contact.Telecom.Add(phoneHome);
                    addContact = true;
                }
                if (!(PhoneCell is null) && PhoneCell.Length > 0)
                {
                    ContactPoint phoneCell = new ContactPoint(ContactPoint.ContactPointSystem.Phone, ContactPoint.ContactPointUse.Mobile, PhoneCell);
                    contact.Telecom.Add(phoneCell);
                    addContact = true;
                }
                if (!(PhoneWork is null) && PhoneWork.Length > 0)
                {
                    ContactPoint phoneWork = new ContactPoint(ContactPoint.ContactPointSystem.Phone, ContactPoint.ContactPointUse.Work, PhoneWork);
                    contact.Telecom.Add(phoneWork);
                    addContact = true;
                }
                if (addContact)
                {
                    pt.Contact.Add(contact);
                }
            }
            catch (Exception ex)
            {
                _exception = new Exception("PatientProfile.CreateFHIRProfile() failed to create a new profile: " + ex.Message);
                return null;
            }
            return pt;
        }

        public bool SendFHIRPatientProfile(Patient pt = null)
        {
            try
            {
                FhirClient client = new FhirClient(FHIRAddress);
                if (pt is null)
                {
                    pt = CreateFHIRPatientProfile();
                }
                Patient newpatient = client.Create(pt);
            }
            catch (Exception ex)
            {
                _exception = new Exception("PatientProfile.SendFHIRPatientProfile() failed to send to FHIR server: " + ex.Message);
                return false;
            }
            return true;
        }

        public bool SendFHIRMediaProfile(Media media)
        {
            try
            {
                FhirClient client = new FhirClient(FHIRAddress);
                client.Create(media);
            }
            catch (Exception ex)
            {
                _exception = new Exception("PatientProfile.SendFHIRMediaProfile() failed to send to FHIR server: " + ex.Message);
                return false;
            }
            return true;
        }

        private CodeableConcept GetBodySite(CaptureSiteSnoMedCode captureSite, LateralitySnoMedCode laterality)
        {
            int captureSiteCode = (int)captureSite;
            int lateralityCode = (int)laterality;
            CodeableConcept bodyCaptureSite = new CodeableConcept("SNOMED", captureSiteCode.ToString(), Utilities.CaptureSiteToString(captureSite));
            Extension extLaterality = new Extension(lateralityCode.ToString(), new FhirString(Utilities.LateralityToString(laterality)));
            bodyCaptureSite.AddExtension("Laterality", extLaterality);
            return bodyCaptureSite;
        }

        public Media FingerPrintFHIRMedia(FingerPrintMinutias fingerPrints)
        {
            Media FingerPrintMedia = null;
            try
            {
                if (!(fingerPrints is null))
                {
                    FingerPrintMedia = new Media(); //Creates the fingerprint minutia template FHIR object as media type.
                    FingerPrintMedia.AddExtension("Biometic Capture", Utilities.CaptureSiteExtension(fingerPrints.CaptureSiteSnoMedCode, fingerPrints.LateralitySnoMedCode));
                    FingerPrintMedia.Identifier = new List<Identifier>();
                    Identifier idSession;
                    Identifier idPatientCertificate;
                    if (SessionID.Length > 0)
                    {
                        idSession = new Identifier();
                        idSession.System = "http://www.mynoid.com/fhir/SessionID";
                        idSession.Value = SessionID;
                        FingerPrintMedia.Identifier.Add(idSession);
                    }
                    if (PatientCertificateID.Length > 0)
                    {
                        idPatientCertificate = new Identifier();
                        idPatientCertificate.System = "http://www.mynoid.com/fhir/PatientCertificateID";
                        idPatientCertificate.Value = PatientCertificateID;
                        FingerPrintMedia.Identifier.Add(idPatientCertificate);
                    }

                    foreach (var Minutia in fingerPrints.Minutiae)
                    {
                        if (FingerPrintMedia is null)
                        {
                            FingerPrintMedia = new Media();
                        }

                        Extension extFingerPrintMedia = Utilities.FingerPrintMediaExtension(
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
                    _exception = ex;
                    return false;
                }
            }
            return true;
        }
    }
}
