// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using SourceAFIS.Templates;
using SourceAFIS.Simple;
using static SourceAFIS.Templates.TemplateBuilder;

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

        public static Template ConvertFingerPrintMinutias(FingerPrintMinutias fingerPrintMinutias)
        {
            List <Minutia> listMinutia = new List<Minutia>();
            foreach (FingerPrintMinutia fingerPrintMinutia in fingerPrintMinutias.Minutiae)
            {
                Minutia newMinutiaPoint = new Minutia();
                newMinutiaPoint.Position.X = fingerPrintMinutia.PositionX;
                newMinutiaPoint.Position.Y = fingerPrintMinutia.PositionY;
                newMinutiaPoint.Direction = fingerPrintMinutia.Direction;
                newMinutiaPoint.Type = (MinutiaType)fingerPrintMinutia.Type;

                listMinutia.Add(newMinutiaPoint);
            }
            TemplateBuilder tb = new TemplateBuilder();
            tb.Minutiae = listMinutia;
            Template convertedTemplate = new Template(tb);

            return convertedTemplate;
        }
    }
}
