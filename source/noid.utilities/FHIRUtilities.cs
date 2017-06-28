// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.IO;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace NoID.Utilities
{
    public static class FHIRUtilities
    {
        //TODO: these are made up.  Needs to register NoID HL7 OID sequence.
        public static readonly string NoID_OID = "3.15.750.1.113883.6.35";
        public static readonly string FingerPrint_OID = "3.15.750.1.113883.6.36";
        public static readonly string PositionX_OID = "3.15.750.1.113883.6.37";
        public static readonly string PositionY_OID = "3.15.750.1.113883.6.38";
        public static readonly string Direction_OID = "3.15.750.1.113883.6.39";
        public static readonly string Type_OID = "3.15.750.1.113883.6.40";

        private static readonly string FHIRDateExpression = "yyyy-MM-dd";
        private static readonly string FHIRDateTimeExpression = "yyyy-MM-dd HH:mm:ss -Z";

        //TODO: add right and left feet
        public enum CaptureSiteSnoMedCode : uint
        {
            IndexFinger = 48856004, // SnoMedCT Description: Skin structure of palmar surface of index finger
            MiddleFinger = 65271000, // SnoMedCT Description: Skin structure of palmar surface of middle finger
            RingFinger = 28404007, // SnoMedCT Description: Skin structure of palmar surface of ring finger
            LittleFinger = 43825009, // SnoMedCT Description: Skin structure of palmar surface of little finger
            Thumb = 72331001, // SnoMedCT Description: Skin structure of palmar surface of thumb
            Unknown = 0
        }

        public enum LateralitySnoMedCode : uint
        {
            Left = 419161000, // SnoMedCT Description: Unilateral left
            Right = 419165000, //  SnoMedCT Description: Unilateral right
            Bilateral = 51440002, // SnoMedCT Description: Bilateral
            Unknown = 0
        }

        public static Resource StreamToFHIR(StreamReader streamReader)
        {
            try
            {
                string jsonString = streamReader.ReadToEnd();
                FhirJsonParser fhirJsonParser = new FhirJsonParser();
                return fhirJsonParser.Parse<Resource>(jsonString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string FHIRToString(Resource _fhir)
        {
            string jsonString;
            try
            {
                jsonString = _fhir.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return jsonString;
        }

        public static Patient CreateTestFHIRPatientProfile
            (
                string organizationName, string sessionID, string internalNoID, string language, string birthDate,
                string genderCode, string multipleBirth, string firstName, string middleName, string lastName,
                string address1, string address2, string city, string state, string postalCode,
                string homePhone, string cellPhone, string workPhone, string emailAddress
            )
        {
            Patient pt = new Patient();
            Identifier idSession;
            Identifier idPatientCertificate;

            idSession = new Identifier();
            idSession.System = "http://www.mynoid.com/fhir/SessionID";
            idSession.Value = sessionID;
            pt.Identifier.Add(idSession);

            idPatientCertificate = new Identifier();
            idPatientCertificate.System = "http://www.mynoid.com/fhir/PatientCertificateID";
            idPatientCertificate.Value = internalNoID;
            pt.Identifier.Add(idPatientCertificate);

            ResourceReference managingOrganization = new ResourceReference(NoID_OID, organizationName);
            pt.ManagingOrganization = managingOrganization;

            pt.Language = language;
            pt.BirthDate = birthDate;
            if (genderCode.Substring(0,1).ToLower() == "f")
            {
                pt.Gender = AdministrativeGender.Female;
            }
            else if (genderCode.Substring(0, 1).ToLower() == "m")
            {
                pt.Gender = AdministrativeGender.Male;
            }
            else
            {
                pt.Gender = AdministrativeGender.Other;
            }
            
            // just use Yes or No
            if (multipleBirth.ToLower() == "no")
            {
                pt.MultipleBirth = new FhirString(multipleBirth);
            }
            else
            {
                pt.MultipleBirth = new FhirString("Yes");
            }
            
            // Add patient name
            HumanName ptName = new HumanName();
            ptName.Given = new string[] { firstName, middleName };
            ptName.Family = lastName;
            pt.Name = new List<HumanName> { ptName };
            // Add patient address
            Address address = new Address();
            address.Line = new string[] { address1, address2 };
            address.City = city;
            address.State = state;
            address.Country = "USA";
            address.PostalCode = postalCode;
            pt.Address.Add(address);

            Patient.ContactComponent contact = new Patient.ContactComponent();
            bool addContact = false;
            if ((emailAddress != null) && emailAddress.Length > 0)
            {
                ContactPoint newContact = new ContactPoint(ContactPoint.ContactPointSystem.Email, ContactPoint.ContactPointUse.Home, emailAddress);
                contact.Telecom.Add(newContact);
                addContact = true;
            }
            if ((homePhone != null) && homePhone.Length > 0)
            {
                ContactPoint newContact = new ContactPoint(ContactPoint.ContactPointSystem.Phone, ContactPoint.ContactPointUse.Home, homePhone);
                contact.Telecom.Add(newContact);
                addContact = true;
            }
            if ((cellPhone != null) && cellPhone.Length > 0)
            {
                ContactPoint newContact = new ContactPoint(ContactPoint.ContactPointSystem.Phone, ContactPoint.ContactPointUse.Mobile, cellPhone);
                contact.Telecom.Add(newContact);
                addContact = true;
            }
            if ((workPhone != null) && workPhone.Length > 0)
            {
                ContactPoint newContact = new ContactPoint(ContactPoint.ContactPointSystem.Phone, ContactPoint.ContactPointUse.Work, workPhone);
                contact.Telecom.Add(newContact);
                addContact = true;
            }
            if (addContact)
            {
                pt.Contact.Add(contact);
            }
            return pt;
        }

        public static Patient CreateTestFHIRPatientProfile()
        {
            Patient pt = new Patient();
            Identifier idSession;
            Identifier idPatientCertificate;

            idSession = new Identifier();
            idSession.System = "http://www.mynoid.com/fhir/SessionID";
            idSession.Value = "S12348";
            pt.Identifier.Add(idSession);

            idPatientCertificate = new Identifier();
            idPatientCertificate.System = "http://www.mynoid.com/fhir/PatientCertificateID";
            idPatientCertificate.Value = "PT67891";
            pt.Identifier.Add(idPatientCertificate);

            ResourceReference managingOrganization = new ResourceReference(NoID_OID, "Test NoID");
            pt.ManagingOrganization = managingOrganization;

            pt.Language = "English";
            pt.BirthDate = "2006-07-03";
            pt.Gender = AdministrativeGender.Female;
            pt.MultipleBirth = new FhirString("No");
            // Add patient name
            HumanName ptName = new HumanName();
            ptName.Given = new string[] { "Mary", "J" };
            ptName.Family = "Bling";
            pt.Name = new List<HumanName> { ptName };
            // Add patient address
            Address address = new Address();
            address.Line = new string[] { "300 Exit St", "Unit 5" };
            address.City = "New Orleans";
            address.State = "LA";
            address.Country = "USA";
            address.PostalCode = "70112-1202";
            pt.Address.Add(address);
            Attachment attach = new Attachment();
            Media media = new Media();

            media.AddExtension("Healthcare Node", FHIRUtilities.OrganizationExtension("Test NoID FHIR Message", "noidtest.net", "devtest.noidtest.net"));
            media.AddExtension("Biometic Capture", FHIRUtilities.CaptureSiteExtension(
                CaptureSiteSnoMedCode.IndexFinger, LateralitySnoMedCode.Left, "Test Scanner Device", 500, 350, 290));
            Extension extFingerPrintMedia = FHIRUtilities.FingerPrintMediaExtension(
                            "123",
                            "211",
                            "43",
                            "0"
                        );

            media.Extension.Add(extFingerPrintMedia);

            extFingerPrintMedia = FHIRUtilities.FingerPrintMediaExtension(
                            "180",
                            "91",
                            "211",
                            "1"
                        );

            media.Extension.Add(extFingerPrintMedia);

            extFingerPrintMedia = FHIRUtilities.FingerPrintMediaExtension(
                            "201",
                            "154",
                            "44",
                            "1"
                        );

            media.Extension.Add(extFingerPrintMedia);

            extFingerPrintMedia = FHIRUtilities.FingerPrintMediaExtension(
                            "21",
                            "279",
                            "310",
                            "0"
                        );

            media.Extension.Add(extFingerPrintMedia);


            attach.Data = FhirSerializer.SerializeToJsonBytes(media, summary: Hl7.Fhir.Rest.SummaryType.False);

            pt.Photo.Add(attach);

            return pt;
        }

        public static Extension FingerPrintMediaExtension(string PositionX, string PositionY, string Direction, string Type)
        {
            /*
                Example JSON FHIR Message
                Media 
                content:"content": 
                {  
                    "extension": 
                    [ 
                        {      "url": "PositionX",      "valueInteger": 1    },    
                        {      "url": "PositionY",      "valueInteger": 2    },    
                        {      "url": "Direction",      "valueInteger": 3    },    
                        {      "url": "Type",           "valueInteger": 4    }  
                    ]
                }
            */

            Extension extMinutiaPoint = new Extension("Minutia", new FhirString("Points"));
            Extension extMinutiaPositionX = extMinutiaPoint.AddExtension("PositionX", new FhirString(PositionX));
            Extension extMinutiaPositionY = extMinutiaPoint.AddExtension("PositionY", new FhirString(PositionY));
            Extension extMinutiaDirection = extMinutiaPoint.AddExtension("Direction", new FhirString(Direction));
            Extension extMinutiaType = extMinutiaPoint.AddExtension("Type", new FhirString(Type));
            return extMinutiaPoint;
        }

        public static Extension CaptureSiteExtension
            (
            CaptureSiteSnoMedCode captureSite, 
            LateralitySnoMedCode laterality,
            string ScannerName,
            int OriginalDPI,
            int OriginalHeight,
            int OriginalWidth
            )
        {
            /*
                Example JSON FHIR Message
                CaptureSite
                content:"content": 
                {  
                    "extension": 
                    [ 
                        {      "url": "Coding System",                  "valueString": SNOMED           },    
                        {      "url": "Capture Site Description",       "valueString": IndexFinger      },    
                        {      "url": "Laterality Code",                "valueString": 419161000        },    
                        {      "url": "Laterality Description",         "valueString": Left             },
                        {      "url": "Scanner Name",                   "valueString": U.are.U 4500     },  
                        {      "url": "Original DPI",                   "valueInt": 500                 },  
                        {      "url": "Original Height",                "valueInt": 300                 },  
                        {      "url": "Original Width",                 "valueInt": 200                 }  
                    ]
                }
            */
            //TODO: Use bodysite instead of an extension but bodysite currently not working with Spark FHIR.
            int captureSiteCode = (int)captureSite;
            int lateralityCode = (int)laterality;

            Extension extCaptureSite = new Extension("Capture Site", new FhirString(captureSiteCode.ToString()));

            Extension extCodingSystem = extCaptureSite.AddExtension("Coding System", new FhirString("SNOMED"));
            Extension extCaptureSiteDescription = extCaptureSite.AddExtension("Capture Site Description", new FhirString(CaptureSiteToString(captureSite)));
            //
            extCaptureSite.AddExtension("Laterality Code", new FhirString(lateralityCode.ToString()));
            extCaptureSite.AddExtension("Laterality Description", new FhirString(LateralityToString(laterality)));
            //
            extCaptureSite.AddExtension("Scanner Name", new FhirString(ScannerName));
            extCaptureSite.AddExtension("Original DPI", new FhirString(OriginalDPI.ToString()));
            extCaptureSite.AddExtension("Original Height", new FhirString(OriginalHeight.ToString()));
            extCaptureSite.AddExtension("Original Width", new FhirString(OriginalWidth.ToString()));
            return extCaptureSite;
        }

        public static Extension OrganizationExtension(string organizationName, string domainName, string fhirServerName)
        {
            /*
                Example JSON FHIR Message
                Organization 
                content:"content": 
                {  
                    "extension": 
                    [ 
                        {      "url": "Domain",                         "valueString": noidtest.net              },    
                        {      "url": "FHIR Server",                    "valueString": https://hn1.noidtest.net  }
                    ]
                }
            */

            Extension ext = new Extension("Organization", new FhirString(organizationName));
            Extension extCodingSystem = ext.AddExtension("Domain", new FhirString(domainName));
            Extension extCaptureSiteDescription = ext.AddExtension("FHIR Server", new FhirString(fhirServerName));
            return ext;
        }

        public static string LateralityToString(LateralitySnoMedCode laterality)
        {
            return Enum.GetName(typeof(LateralitySnoMedCode), laterality);
        }

        public static LateralitySnoMedCode StringToLaterality(string laterality)
        {
            Type latType = typeof(LateralitySnoMedCode);
            if (laterality.ToLower() == "left")
            {
                return LateralitySnoMedCode.Left;
            }
            else if (laterality.ToLower() == "right")
            {
                return LateralitySnoMedCode.Right;
            }
            else
            {
                return LateralitySnoMedCode.Unknown;
            }   
        }

        public static CaptureSiteSnoMedCode StringToCaptureSite(string captureSite)
        {
            Type latType = typeof(LateralitySnoMedCode);
            if (captureSite.ToLower() == "indexfinger")
            {
                return CaptureSiteSnoMedCode.IndexFinger;
            }
            else if (captureSite.ToLower() == "ringfinger")
            {
                return CaptureSiteSnoMedCode.RingFinger;
            }
            else if (captureSite.ToLower() == "middlefinger")
            {
                return CaptureSiteSnoMedCode.MiddleFinger;
            }
            else if (captureSite.ToLower() == "thumb")
            {
                return CaptureSiteSnoMedCode.Thumb;
            }
            else if (captureSite.ToLower() == "littlefinger")
            {
                return CaptureSiteSnoMedCode.LittleFinger;
            }
            else
            {
                return CaptureSiteSnoMedCode.Unknown;
            }
        }

        public static string CaptureSiteToString(CaptureSiteSnoMedCode captureSite)
        {
            return Enum.GetName(typeof(CaptureSiteSnoMedCode), captureSite);
        }

        public static string DateToFHIRString(DateTime _date)
        {
            string dateString;
            try
            {
                dateString = _date.ToString(FHIRDateExpression);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dateString;
        }

        public static string DateTimeToFHIRString(DateTime _date)
        {
            string dateString;
            try
            {
                dateString = _date.ToString(FHIRDateTimeExpression);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dateString;
        }

        public static string NowToFHIRString()
        {
            // Uses the current UTC date and time.
            string dateString;
            try
            {
                dateString = DateTime.UtcNow.ToString(FHIRDateTimeExpression);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dateString;
        }

        public static CaptureSiteSnoMedCode SnoMedCodeToCaptureSite(string snoMedCode)
        {
            /*
            CaptureSite SnoMedCode Map:
            IndexFinger = 48856004, // SnoMedCT Description: Skin structure of palmar surface of index finger
            MiddleFinger = 65271000, // SnoMedCT Description: Skin structure of palmar surface of middle finger
            RingFinger = 28404007, // SnoMedCT Description: Skin structure of palmar surface of ring finger
            LittleFinger = 43825009, // SnoMedCT Description: Skin structure of palmar surface of little finger
            Thumb = 72331001 // SnoMedCT Description: Skin structure of palmar surface of thumb
            */
            switch (UInt32.Parse(snoMedCode))
            {
                case 48856004:
                    return CaptureSiteSnoMedCode.IndexFinger;
                case 65271000:
                    return CaptureSiteSnoMedCode.MiddleFinger;
                case 28404007:
                    return CaptureSiteSnoMedCode.RingFinger;
                case 43825009:
                    return CaptureSiteSnoMedCode.LittleFinger;
                case 72331001:
                    return CaptureSiteSnoMedCode.Thumb;
                default:
                    return CaptureSiteSnoMedCode.Unknown;
            }
        }

        public static LateralitySnoMedCode SnoMedCodeToLaterality(string snoMedCode)
        {
            /*
            Laterality SnoMedCode Map:
            Left = 419161000 
            Right = 419165000 
            */
            switch (UInt32.Parse(snoMedCode))
            {
                case 419161000:
                    return LateralitySnoMedCode.Left;
                case 419165000:
                    return LateralitySnoMedCode.Right;
                default:
                    return LateralitySnoMedCode.Unknown;
            }
        }

        public static uint SnoMedCaptureSiteNameToCode(string snoMedCodeName)
        {
            return (uint)Enum.Parse(typeof(CaptureSiteSnoMedCode), snoMedCodeName, true);
        }

        public static uint SnoMedLateralityNameToCode(string snoMedCodeName)
        {
            return (uint)Enum.Parse(typeof(LateralitySnoMedCode), snoMedCodeName, true);
        }
    }
}
