// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using SourceAFIS.Templates;

namespace NoID.FHIR.Profile
{
    /// <summary cref="PatientFingerprintMinutia">  
    /// Utility class used to convert Templates to FingerPrintMinutia Lists
    /// </summary>  
    public static class FingerprintMinutiaConvertor
    {
        public static List<FingerPrintMinutia> ConvertTemplate(Template template)
        {
            List<FingerPrintMinutia> listFingerPrintMinutia = new List<FingerPrintMinutia>();
            foreach (Template.Minutia minutia in template.Minutiae)
            {
                FingerPrintMinutia fm = new FingerPrintMinutia(minutia.Position.X, minutia.Position.Y, minutia.Direction, (byte)minutia.Type);
                listFingerPrintMinutia.Add(fm);
            }
            return listFingerPrintMinutia;
        }

        static Template ConvertTemplate(FingerPrintMinutias template, ulong certificateID, ushort fingerOrdinal)
        {
            TemplateBuilder tb = new TemplateBuilder();
            Template convertedTemplate = new Template(tb);

            return convertedTemplate;
        }
    }
}
