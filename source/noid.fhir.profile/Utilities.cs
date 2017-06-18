// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying

using Hl7.Fhir.Model;
using System;

namespace NoID.FHIR.Profile
{
    public static class Utilities
    {
        public static readonly string FingerPrint_OID   = "3.15.750.1.113883.6.35";
        public static readonly string PositionX_OID     = "3.15.750.1.113883.6.36";
        public static readonly string PositionY_OID     = "3.15.750.1.113883.6.37";
        public static readonly string Direction_OID     = "3.15.750.1.113883.6.38";
        public static readonly string Type_OID          = "3.15.750.1.113883.6.39";

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
    }
}
