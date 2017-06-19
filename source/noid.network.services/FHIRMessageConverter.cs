// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.IO;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace NoID.Network.Services
{
    public static class FHIRMessageConverter
    {
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
    }
}