// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using ProtoBuf;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using NoID.Utilities;
using System.Linq;

namespace NoID.FHIR.Profile
{

    /// <summary cref="PatientFHIRProfileSerialize">  
    /// Lightweight NoID Patient FHIR profile
    /// </summary>  
    public class PatientProfile
    {
        private SourceAFIS.Templates.NoID _noID;
        private readonly string _organizationName;
        private readonly Uri _fhirAddress;

        private string _language = "";
        private string _firstName = "";
        private string _lastName = "";
        private string _middleName = "";
        private string _gender; // F, M or O
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
        private string _multipleBirth; //Yes or No

        public PatientProfile(string organizationName, Uri fhirAddress)
        {
            _organizationName = organizationName;
            _fhirAddress = fhirAddress;
            _noID = new SourceAFIS.Templates.NoID();
        }

        public PatientProfile(string organizationName, Uri fhirAddress, Patient loadPatient)
        {
            _organizationName = organizationName;
            _fhirAddress = fhirAddress;
            _noID = new SourceAFIS.Templates.NoID();
            if (loadPatient != null)
            {
                _lastName   = loadPatient.Name[0].Family.ToString();
                List<string> givenNames = loadPatient.Name[0].Given.ToList();
                _firstName = givenNames[0].ToString();
                if (givenNames.Count > 1)
                {
                    _middleName = givenNames[1].ToString();
                }
                
                _gender = loadPatient.Gender.ToString().Substring(0, 1).ToUpper();
                _birthDate = loadPatient.BirthDate.ToString();

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
                if (loadPatient.Contact.Count > 0)
                {
                    //TODO: Load contact information, email, phones.
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
            _noID.SessionID = sha256Hash(Guid.NewGuid().ToString());
        }

        public Uri FHIRAddress
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

        private static string sha256Hash(string _value)
        {
            SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(_value), 0, Encoding.UTF8.GetByteCount(_value));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }

    public class PatientFHIRProfile : PatientProfile
    {

        private Exception _exception;

        private FingerPrintMinutias _leftFingerPrints;
        private FingerPrintMinutias _rightFingerPrints;
        private FingerPrintMinutias _leftAlternateFingerPrints;
        private FingerPrintMinutias _rightAlternateFingerPrints;

        public int OriginalDpi;
        public int OriginalHeight;
        public int OriginalWidth;

        public PatientFHIRProfile(string organizationName, Uri endPoint) : base(organizationName, endPoint)
        {
        }

        public PatientFHIRProfile(string organizationName, Uri endPoint, Patient loadPatient) : base(organizationName, endPoint, loadPatient)
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

        public FingerPrintMinutias LeftFingerPrints
        {
            get { return _leftFingerPrints; }
            set { _leftFingerPrints = value; }
        }

        public FingerPrintMinutias RightFingerPrints
        {
            get { return _rightFingerPrints; }
            set { _rightFingerPrints = value; }
        }
        
        public FingerPrintMinutias LeftAlternateFingerPrints
        {
            get { return _leftAlternateFingerPrints; }
            set { _leftAlternateFingerPrints = value; }
        }

        public FingerPrintMinutias RightAlternateFingerPrints
        {
            get { return _rightAlternateFingerPrints; }
            set { _rightAlternateFingerPrints = value; }
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

        public Media FingerPrintFHIRMedia(FingerPrintMinutias fingerPrints, string scannerName, int originalDPI, int originalHeight, int originalWidth)
        {
            Media FingerPrintMedia = null;
            try
            {
                if ((fingerPrints != null))
                {
                    FingerPrintMedia = new Media(); //Creates the fingerprint minutia template FHIR object as media type.
                    FingerPrintMedia.AddExtension("Healthcare Node", FHIRUtilities.OrganizationExtension(OrganizationName, DomainName, ServerName));
                    FingerPrintMedia.AddExtension("Biometic Capture", FHIRUtilities.CaptureSiteExtension(fingerPrints.CaptureSiteSnoMedCode, fingerPrints.LateralitySnoMedCode, scannerName, originalDPI, originalHeight, originalWidth));

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
