// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using Hl7.Fhir.Model;
using System;
using static NoID.FHIR.Profile.PatientFHIRProfile;

namespace NoID.FHIR.Profile
{
    public static class Utilities
    {
        public static readonly string NoID_OID          = "3.15.750.1.113883.6.35";
        public static readonly string FingerPrint_OID   = "3.15.750.1.113883.6.36";
        public static readonly string PositionX_OID     = "3.15.750.1.113883.6.37";
        public static readonly string PositionY_OID     = "3.15.750.1.113883.6.38";
        public static readonly string Direction_OID     = "3.15.750.1.113883.6.39";
        public static readonly string Type_OID          = "3.15.750.1.113883.6.40";

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
