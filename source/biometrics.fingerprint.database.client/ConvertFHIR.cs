// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using SourceAFIS.Templates;
using NoID.Utilities;

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

        static SourceAFIS.Templates.NoID FHIRToNoID(Resource fhirMessage)
        {
            SourceAFIS.Templates.NoID GetNoID = null;
            try
            {
                switch (fhirMessage.TypeName)
                {
                    case "Media":
                        Media mediaBiometrics = (Media)fhirMessage;
                        GetNoID = new SourceAFIS.Templates.NoID();
                        foreach (Identifier id in mediaBiometrics.Identifier)
                        {
                            if (id.System.Contains("SessionID") == true)
                            {
                                GetNoID.SessionID = id.Value;
                            }
                            else if (id.System.Contains("LocalNoID") == true)
                            {
                                GetNoID.LocalNoID = id.Value;
                            }
                            else if (id.System.Contains("RemoteNoID") == true)
                            {
                                GetNoID.RemoteNoID = id.Value;
                            }
                        }
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
            return GetNoID;
        }

        static Template MediaFHIRToTemplate(Resource fhirMessage)
        {
            Template converted = null;
            try
            {
                Media biometricFHIR = (Media)fhirMessage;
                
                Extension organizationExtension = null;
                Extension captureSiteExtension = null;
                uint n = 0;
                TemplateBuilder templateBuilder = new TemplateBuilder();
                foreach (Extension extension in biometricFHIR.Extension)
                {
                    if (n == 0)
                    {
                        organizationExtension = extension;
                    }
                    else if (n == 1)
                    {
                        captureSiteExtension = extension;
                    }
                    else
                    {
                        TemplateBuilder.Minutia minutia = new TemplateBuilder.Minutia();
                        List<Extension> ext = extension.Value.Extension;
                        
                        minutia.Position.X = Int32.Parse(ext[0].Value.ToString());
                        minutia.Position.Y = Int32.Parse(ext[1].Value.ToString());
                        minutia.Direction = Byte.Parse(ext[2].Value.ToString());
                        minutia.Type = ConvertStringToMinutiaType(ext[3].Value.ToString());
                        templateBuilder.Minutiae.Add(minutia);
                    }
                    n += 1;
                }
                if (organizationExtension != null && captureSiteExtension != null && templateBuilder.Minutiae.Count > 0)
                {
                    templateBuilder.OriginalDpi = Int32.Parse(captureSiteExtension.Value.Extension[5].Value.ToString());
                    templateBuilder.OriginalHeight = Int32.Parse(captureSiteExtension.Value.Extension[6].Value.ToString());
                    templateBuilder.OriginalWidth = Int32.Parse(captureSiteExtension.Value.Extension[7].Value.ToString());
                    converted = new Template(templateBuilder);
                }
                string captureSiteCode = biometricFHIR.Extension[1].Value.Extension[1].Value.ToString();
                string lateralityCode = biometricFHIR.Extension[1].Value.Extension[2].Value.ToString();

                converted.NoID = FHIRToNoID(fhirMessage); //Gets the NoID Identifiers
                converted.NoID.CaptureSiteSnoMedCode = FHIRUtilities.SnoMedCaptureSiteNameToCode(captureSiteCode);
                converted.NoID.LateralitySnoMedCode  = UInt32.Parse(lateralityCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return converted;
        }

        private static TemplateBuilder.MinutiaType ConvertStringToMinutiaType(string typeCode)
        {
            TemplateBuilder.MinutiaType type = TemplateBuilder.MinutiaType.Other;
            try
            {
                Int32 minutiaType = Int32.Parse(typeCode);
                if (minutiaType == 0)
                {
                    type = TemplateBuilder.MinutiaType.Ending;
                }
                else if (minutiaType == 1)
                {
                    type = TemplateBuilder.MinutiaType.Bifurcation;
                }
                else if (minutiaType == 2)
                {
                    type = TemplateBuilder.MinutiaType.Other;
                }
            }
            catch
            {
                throw new Exception("Invalid MinutiaType.");
            }
            return type;
        }
    }
}
