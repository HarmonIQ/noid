using System;
using System.Collections.Generic;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using NoID.Cryptographic.Hash;

namespace NoID.FHIR.Profile
{
    public class PatientFHIRProfile
    {
        // Argon2 Hash parameters. 
        // Currently a protocol level setting for now but could be a setting defined per healthcare and/or match hub.
        // TODO: increase the difficulty of the hash parameters after the prototype.
        const int ARGON2_TIME_COST = 1;
        const int ARGON2_MEMORY_COST = 250;
        const int ARGON2_PARALLEL_LANES = 4;

        HashWriter.ArgonParams argonParams = new HashWriter.ArgonParams(ARGON2_TIME_COST, ARGON2_MEMORY_COST, ARGON2_PARALLEL_LANES);

        //TODO: load and use saltList from matching hubs
        private string hashSalt = "C560325F";

        public PatientFHIRProfile(string organizationName, Uri fhirAddress)
        {
            _fhirAddress = fhirAddress;
            _organizationName = organizationName;
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
        private string _patientCertificate;

        //TODO: store captured finger SnoMedCT code with minutias.
        private List<FingerPrintMinutia> _leftFingerPrints;
        private List<FingerPrintMinutia> _rightFingerPrints;
        private List<FingerPrintMinutia> _alternate1FingerPrints;
        private List<FingerPrintMinutia> _alternate2FingerPrints;

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

        public List<FingerPrintMinutia> LeftFingerPrints
        {
            get { return _leftFingerPrints; }
            set { _leftFingerPrints = value; }
        }

        public string LeftFingerPrintHash
        {
            get { return HashWriter.Hash(_leftFingerPrints.ToString(), hashSalt, argonParams); }
        }

        public List<FingerPrintMinutia> RightFingerPrints
        {
            get { return _rightFingerPrints; }
            set { _rightFingerPrints = value; }
        }

        public string RightFingerPrintHash
        {
            get { return HashWriter.Hash(_rightFingerPrints.ToString(), hashSalt, argonParams); }
        }

        public List<FingerPrintMinutia> Alternate1FingerPrints
        {
            get { return _alternate1FingerPrints; }
            set { _alternate1FingerPrints = value; }
        }

        public string Alternate1FingerPrintHash
        {
            get { return HashWriter.Hash(_alternate1FingerPrints.ToString(), hashSalt, argonParams); }
        }

        public List<FingerPrintMinutia> Alternate2FingerPrints
        {
            get { return _alternate2FingerPrints; }
            set { _alternate2FingerPrints = value; }
        }

        public string Alternate2FingerPrintHash
        {
            get { return HashWriter.Hash(_alternate2FingerPrints.ToString(), hashSalt, argonParams); }
        }

        public string PatientCertificate
        {
            get { return _patientCertificate; }
            set { _patientCertificate = value; }
        }

        public string PatientCertificateHash
        {
            get { return HashWriter.Hash(_patientCertificate.ToString(), hashSalt, argonParams); }
        }

        public Patient CreateFHIRProfile()
        {
            Patient pt;
            try
            {
                pt = new Patient();
                // Add patient certificate hash.
                Identifier id = new Identifier();
                id.Value = PatientCertificate; //hash of local patient certificate
                pt.Identifier = new List<Identifier> { id };
                // Add healthcare node certificate hash.
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

                // Add biometrics to FHIR messages
                // Add fingerprint minutia points to FHIR message
                Media leftFingerprintMedia = null;
                Media rightFingerprintMedia = null;
                Media alt1FingerprintMedia = null;
                Media alt2FingerprintMedia = null;
                Extension extMinutiaPositionX;
                Extension extMinutiaPositionY;
                Extension extMinutiaDirection;
                Extension extMinutiaType;

                foreach (var Minutia in LeftFingerPrints)
                {
                    if (leftFingerprintMedia is null) {
                        leftFingerprintMedia = new Media();
                    }
                    
                    extMinutiaPositionX = new Extension();
                    extMinutiaPositionX.Value.SetIntegerExtension("PositionX", (int)Minutia.PositionX);
                    extMinutiaPositionY = new Extension();
                    extMinutiaPositionY.Value.SetIntegerExtension("PositionY", (int)Minutia.PositionY);
                    extMinutiaDirection = new Extension();
                    extMinutiaDirection.Value.SetIntegerExtension("Direction", (int)Minutia.Direction);
                    extMinutiaType = new Extension();

                    extMinutiaType.Value.SetIntegerExtension("Type", (int)Minutia.Type);
                    leftFingerprintMedia.Extension.Add(extMinutiaPositionX);
                    leftFingerprintMedia.Extension.Add(extMinutiaPositionY);
                    leftFingerprintMedia.Extension.Add(extMinutiaDirection);
                    leftFingerprintMedia.Extension.Add(extMinutiaType);
                }

                foreach (var Minutia in LeftFingerPrints)
                {
                    if (rightFingerprintMedia is null)
                    {
                        rightFingerprintMedia = new Media();
                    }

                    extMinutiaPositionX = new Extension();
                    extMinutiaPositionX.Value.SetIntegerExtension("PositionX", (int)Minutia.PositionX);
                    extMinutiaPositionY = new Extension();
                    extMinutiaPositionY.Value.SetIntegerExtension("PositionY", (int)Minutia.PositionY);
                    extMinutiaDirection = new Extension();
                    extMinutiaDirection.Value.SetIntegerExtension("Direction", (int)Minutia.Direction);
                    extMinutiaType = new Extension();
                    extMinutiaType.Value.SetIntegerExtension("Type", (int)Minutia.Type);

                    rightFingerprintMedia.Extension.Add(extMinutiaPositionX);
                    rightFingerprintMedia.Extension.Add(extMinutiaPositionY);
                    rightFingerprintMedia.Extension.Add(extMinutiaDirection);
                    rightFingerprintMedia.Extension.Add(extMinutiaType);
                }

                foreach (var Minutia in Alternate1FingerPrints)
                {
                    if (alt1FingerprintMedia is null)
                    {
                        alt1FingerprintMedia = new Media();
                    }

                    extMinutiaPositionX = new Extension();
                    extMinutiaPositionX.Value.SetIntegerExtension("PositionX", (int)Minutia.PositionX);
                    extMinutiaPositionY = new Extension();
                    extMinutiaPositionY.Value.SetIntegerExtension("PositionY", (int)Minutia.PositionY);
                    extMinutiaDirection = new Extension();
                    extMinutiaDirection.Value.SetIntegerExtension("Direction", (int)Minutia.Direction);
                    extMinutiaType = new Extension();
                    extMinutiaType.Value.SetIntegerExtension("Type", (int)Minutia.Type);

                    alt1FingerprintMedia.Extension.Add(extMinutiaPositionX);
                    alt1FingerprintMedia.Extension.Add(extMinutiaPositionY);
                    alt1FingerprintMedia.Extension.Add(extMinutiaDirection);
                    alt1FingerprintMedia.Extension.Add(extMinutiaType);
                }

                foreach (var Minutia in Alternate2FingerPrints)
                {
                    if (alt2FingerprintMedia is null)
                    {
                        alt2FingerprintMedia = new Media();
                    }

                    extMinutiaPositionX = new Extension();
                    extMinutiaPositionX.Value.SetIntegerExtension("PositionX", (int)Minutia.PositionX);
                    extMinutiaPositionY = new Extension();
                    extMinutiaPositionY.Value.SetIntegerExtension("PositionY", (int)Minutia.PositionY);
                    extMinutiaDirection = new Extension();
                    extMinutiaDirection.Value.SetIntegerExtension("Direction", (int)Minutia.Direction);
                    extMinutiaType = new Extension();
                    extMinutiaType.Value.SetIntegerExtension("Type", (int)Minutia.Type);

                    alt2FingerprintMedia.Extension.Add(extMinutiaPositionX);
                    alt2FingerprintMedia.Extension.Add(extMinutiaPositionY);
                    alt2FingerprintMedia.Extension.Add(extMinutiaDirection);
                    alt2FingerprintMedia.Extension.Add(extMinutiaType);
                }
            }
            catch (Exception ex)
            {
                _exception = new Exception("PatientProfile.CreateFHIRProfile() failed to create a new profile: " + ex.Message);
                return null;
            }
            return pt;
        }

        public bool SendFHIRProfile()
        {
            try
            {
                var endpoint = FHIRAddress;
                var client = new FhirClient(endpoint);
                Patient pt = CreateFHIRProfile();
                var newpatient = client.Create(pt);
            }
            catch (Exception ex)
            {
                _exception = new Exception("PatientProfile.SendFHIRProfile() failed to send to FHIR server: " + ex.Message);
                return false;
            }
            return true;
        }

        public bool TestEnrollmentSave()
        {
            var endpoint = FHIRAddress;
            var client = new FhirClient(endpoint);
            Patient newPatient = CreateFHIRProfile();
            if (!(newPatient is null))
            {
                try
                {
                    client.Create(CreateFHIRProfile());
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
