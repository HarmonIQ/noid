// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;

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
            pt.BirthDate = "20060703";
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

        public static Extension CaptureSiteExtension(CaptureSiteSnoMedCode captureSite, LateralitySnoMedCode laterality)
        {
            /*
                Example JSON FHIR Message
                CaptureSite
                content:"content": 
                {  
                    "extension": 
                    [ 
                        {      "url": "Coding System",                  "valueString": SNOMED       },    
                        {      "url": "Capture Site Description",       "valueString": IndexFinger  },    
                        {      "url": "Laterality Code",                "valueString": 419161000    },    
                        {      "url": "Laterality Description",         "valueString": Left         }  
                    ]
                }
            */
            //TODO: Use bodysite instead of an extension but bodysite currently not working with Spark FHIR.
            int captureSiteCode = (int)captureSite;
            int lateralityCode = (int)laterality;

            Extension extCaptureSite = new Extension("Capture Site", new FhirString(captureSiteCode.ToString()));
            Extension extCodingSystem = extCaptureSite.AddExtension("Coding System", new FhirString("SNOMED"));
            Extension extCaptureSiteDescription = extCaptureSite.AddExtension("Capture Site Description", new FhirString(CaptureSiteToString(captureSite)));
            Extension extLateralityCode = extCaptureSite.AddExtension("Laterality Code", new FhirString(lateralityCode.ToString()));
            Extension extLateralityDescription = extCaptureSite.AddExtension("Laterality Description", new FhirString(LateralityToString(laterality)));
            return extCaptureSite;
        }

        public static string LateralityToString(LateralitySnoMedCode laterality)
        {
            return Enum.GetName(typeof(LateralitySnoMedCode), laterality);
        }

        public static string CaptureSiteToString(CaptureSiteSnoMedCode captureSite)
        {
            return Enum.GetName(typeof(CaptureSiteSnoMedCode), captureSite);
        }
    }
}
