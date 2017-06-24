// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using SourceAFIS.Templates;

namespace NoID.Match.Database.Client
{
    public static class ConvertFHIR
    {
        public static Template FHIRToTemplate(Resource fhirMessage)
        {
            Template converted = null;
            try
            {
                switch (fhirMessage.TypeName)
                {
                    case "Media":
                        converted = MediaFHIRToTemplate(fhirMessage);
                        break;
                    case "Patient":
                    case "NoID Profile":
                        throw new Exception("Processing the " + fhirMessage.TypeName + " is not implemented yet.");
                    default:
                        throw new Exception("Could not convert FHIR resource type " + fhirMessage.TypeName + " to a minutia template class.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return converted;
        }

        static Template MediaFHIRToTemplate(Resource fhirMessage)
        {
            Template converted;
            try
            {
                Media biometricFHIR = (Media)fhirMessage;
                Extension snoMedCodes = biometricFHIR.Extension[0];
                Extension fingerPrintMinutias = biometricFHIR.Extension[1];
                List<Extension> minutiasExtension = new List<Extension>();

                uint n = 0;

                TemplateBuilder templateBuilder = new TemplateBuilder();
                TemplateBuilder.Minutia minutia = new TemplateBuilder.Minutia();
                templateBuilder.Minutiae.Add(minutia);
                foreach (Extension extension in biometricFHIR.Extension)
                {
                    if (n == 0)
                    {
                        snoMedCodes = extension;
                    }
                    else if (n == 1)
                    {
                        fingerPrintMinutias = extension;
                    }
                    else
                    {
                        minutiasExtension.Add(extension);
                    }
                    
                    n += 1;
                }
                
                converted = new Template(templateBuilder);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return converted;
        }
    }
}
