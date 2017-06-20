// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.IO;
using System.Collections.Generic;
using ProtoBuf;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using NoID.Utilities;
using NoID.Cryptographic.Hash;

namespace NoID.FHIR.Profile
{

    /// <summary cref="PatientFHIRProfileSerialize">  
    /// Lightweight NoID Patient FHIR profile
    /// </summary>  
    [ProtoContract]
    public abstract class PatientFHIRProfileSerialize
    {
        public byte[] Serialize()
        {
            byte[] result;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, this);
                result = stream.ToArray();
            }
            return result;
        }
    }

    [ProtoContract]
    public class PatientFHIRProfile : PatientFHIRProfileSerialize
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

        private string FingerTipSnoMedCTCode(FHIRUtilities.CaptureSiteSnoMedCode fingerTip)
        {
            return fingerTip.ToString();
        }

        private string SnoMedCTLaterality(FHIRUtilities.LateralitySnoMedCode laterality)
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

        private FingerPrintMinutias _leftFingerPrints;
        private FingerPrintMinutias _rightFingerPrints;
        private FingerPrintMinutias _leftAlternateFingerPrints;
        private FingerPrintMinutias _rightAlternateFingerPrints;

        public Exception Exception
        {
            get { return _exception; }
        }

        public Uri FHIRAddress
        {
            get { return _fhirAddress; }
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

        [ProtoMember(1)]
        public string SessionID
        {
            get { return _sessionID; }
            private set { _organizationName = value; }
        }

        [ProtoMember(2)]
        public string OrganizationName
        {
            get { return _organizationName; }
            set { _organizationName = value; }
        }

        public string DomainName
        {
            get
            {
                return _fhirAddress.Host.Substring(_fhirAddress.Host.LastIndexOf('.', _fhirAddress.Host.LastIndexOf('.') - 1) + 1);
            }
        }

        public string ServerName
        {
            get
            {
                return _fhirAddress.GetLeftPart(UriPartial.Authority).ToString();
            }
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

        [ProtoMember(3)]
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        public string FirstNameHash
        {
            get { return HashWriter.Hash(_firstName, hashSalt, argonParams); }
        }

        [ProtoMember(4)]
        public string LastName
        {
            get { return _lastName;}
            set { _lastName = value; }
        }

        public string LastNameHash
        {
            get { return HashWriter.Hash(_lastName, hashSalt, argonParams); }
        }

        [ProtoMember(5)]
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

        [ProtoMember(6)]
        public string BirthDay
        {
            get { return _birthDay; }
            set { _birthDay = value; }
        }

        public string BirthDayHash
        {
            get { return HashWriter.Hash(_birthDay, hashSalt, argonParams); }
        }

        [ProtoMember(7)]
        public string StreetAddress
        {
            get { return _streetAddress; }
            set { _streetAddress = value; }
        }

        public string StreetAddressHash
        {
            get { return HashWriter.Hash(_streetAddress, hashSalt, argonParams); }
        }

        [ProtoMember(8)]
        public string StreetAddress2
        {
            get { return _streetAddress2; }
            set { _streetAddress2 = value; }
        }

        public string StreetAddress2Hash
        {
            get { return HashWriter.Hash(_streetAddress2, hashSalt, argonParams); }
        }

        [ProtoMember(9)]
        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        public string CityHash
        {
            get { return HashWriter.Hash(_city, hashSalt, argonParams); }
        }

        [ProtoMember(10)]
        public string State
        {
            get { return _state; }
            set { _state = value; }
        }

        public string StateHash
        {
            get { return HashWriter.Hash(_state, hashSalt, argonParams); }
        }

        [ProtoMember(11)]
        public string PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }

        public string PostalCodeHash
        {
            get { return HashWriter.Hash(_postalCode, hashSalt, argonParams); }
        }

        [ProtoMember(12)]
        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        public string CountryHash
        {
            get { return HashWriter.Hash(_country, hashSalt, argonParams); }
        }

        [ProtoMember(13)]
        public string PhoneHome
        {
            get { return _phoneHome; }
            set { _phoneHome = value; }
        }

        public string PhoneHomeHash
        {
            get { return HashWriter.Hash(_phoneHome, hashSalt, argonParams); }
        }

        [ProtoMember(14)]
        public string PhoneCell
        {
            get { return _phoneCell; }
            set { _phoneCell = value; }
        }

        public string PhoneCellHash
        {
            get { return HashWriter.Hash(_phoneCell, hashSalt, argonParams); }
        }

        [ProtoMember(15)]
        public string PhoneWork
        {
            get { return _phoneWork; }
            set { _phoneWork = value; }
        }

        public string PhoneWorkHash
        {
            get { return HashWriter.Hash(_phoneWork, hashSalt, argonParams); }
        }

        [ProtoMember(16)]
        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        public string EmailAddressHash
        {
            get { return HashWriter.Hash(_emailAddress, hashSalt, argonParams); }
        }

        [ProtoMember(17)]
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
                FHIRUtilities.LateralitySnoMedCode lateralitySnoMedCode = patientFingerprintMinutia.LateralitySnoMedCode;
                FHIRUtilities.CaptureSiteSnoMedCode captureSiteSnoMedCode = patientFingerprintMinutia.CaptureSiteSnoMedCode;

                switch (captureSiteSnoMedCode)
                {
                    case FHIRUtilities.CaptureSiteSnoMedCode.IndexFinger:
                        {
                            if (lateralitySnoMedCode == FHIRUtilities.LateralitySnoMedCode.Left)
                            {
                                _leftFingerPrints = patientFingerprintMinutia;
                            }
                            else if (lateralitySnoMedCode == FHIRUtilities.LateralitySnoMedCode.Right)
                            {
                                _rightFingerPrints = patientFingerprintMinutia;
                            }
                            break;
                        }
                    case FHIRUtilities.CaptureSiteSnoMedCode.MiddleFinger:
                    case FHIRUtilities.CaptureSiteSnoMedCode.RingFinger:
                    case FHIRUtilities.CaptureSiteSnoMedCode.LittleFinger:
                    case FHIRUtilities.CaptureSiteSnoMedCode.Thumb:
                        {
                            if (lateralitySnoMedCode == FHIRUtilities.LateralitySnoMedCode.Left)
                            {
                                _leftAlternateFingerPrints = patientFingerprintMinutia;
                            }
                            else if (lateralitySnoMedCode == FHIRUtilities.LateralitySnoMedCode.Right)
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

        public Patient CreateFHIRPatientProfile()
        {
            Patient pt;
            try
            {
                pt = new Patient();

                // Add patient certificate hash.
                Identifier idSession;
                Identifier idPatientCertificate;
                if (SessionID.Length > 0)
                {
                    idSession = new Identifier();
                    idSession.System = ServerName + "/fhir/SessionID";
                    idSession.Value = SessionID;
                    pt.Identifier.Add(idSession);
                }
                if (PatientCertificateID.Length > 0)
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
                pt.BirthDate = BirthDay;
                pt.Gender = Gender;
                if (!(MultipleBirth == null))
                {
                    pt.MultipleBirth = MultipleBirth;
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

        private CodeableConcept GetBodySite(FHIRUtilities.CaptureSiteSnoMedCode captureSite, FHIRUtilities.LateralitySnoMedCode laterality)
        {
            int captureSiteCode = (int)captureSite;
            int lateralityCode = (int)laterality;
            CodeableConcept bodyCaptureSite = new CodeableConcept("SNOMED", captureSiteCode.ToString(), FHIRUtilities.CaptureSiteToString(captureSite));
            Extension extLaterality = new Extension(lateralityCode.ToString(), new FhirString(FHIRUtilities.LateralityToString(laterality)));
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
                    FingerPrintMedia.AddExtension("Biometic Capture", FHIRUtilities.CaptureSiteExtension(fingerPrints.CaptureSiteSnoMedCode, fingerPrints.LateralitySnoMedCode));
                    FingerPrintMedia.AddExtension("Healthcare Node", FHIRUtilities.OrganizationExtension(OrganizationName, DomainName, ServerName));
                    FingerPrintMedia.Identifier = new List<Identifier>();

                    Identifier idSession;
                    Identifier idPatientCertificate;
                    if (SessionID.Length > 0)
                    {
                        idSession = new Identifier();
                        idSession.System = ServerName + "/fhir/SessionID";
                        idSession.Value = SessionID;
                        FingerPrintMedia.Identifier.Add(idSession);
                    }
                    if (PatientCertificateID.Length > 0)
                    {
                        idPatientCertificate = new Identifier();
                        idPatientCertificate.System = ServerName + "/fhir/PatientCertificateID";
                        idPatientCertificate.Value = PatientCertificateID;
                        FingerPrintMedia.Identifier.Add(idPatientCertificate);
                    }

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

        public static PatientFHIRProfile Deserialize(byte[] message)
        {
            PatientFHIRProfile result;
            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<PatientFHIRProfile>(stream);
            }
            return result;
        }

    }
}
