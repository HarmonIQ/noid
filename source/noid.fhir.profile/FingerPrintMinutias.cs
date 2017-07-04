// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using SourceAFIS.Templates;
using NoID.Utilities;

namespace NoID.FHIR.Profile
{
    /// <summary cref="FingerprintMinutias">  
    /// Lightweight serializable list of minutias used for persisting data.
    /// </summary>  
    
    public class FingerPrintMinutias
    {
        public SourceAFIS.Templates.NoID NoID { get; set; }

        public FHIRUtilities.LateralitySnoMedCode LateralitySnoMedCode { get; set; }

        public FHIRUtilities.CaptureSiteSnoMedCode CaptureSiteSnoMedCode { get; set; }

        public string DeviceName { get; set; }

        public int OriginalDpi { get; set; }

        public int OriginalHeight { get; set; }

        public int OriginalWidth { get; set; }

        public List<FingerPrintMinutia> Minutiae { get; set; }

        public FingerPrintMinutias(string sessionID, Template template, FHIRUtilities.LateralitySnoMedCode laterality, FHIRUtilities.CaptureSiteSnoMedCode captureSiteSnoMedCode)
        {
            SourceAFIS.Templates.NoID noID = new SourceAFIS.Templates.NoID();
            noID.SessionID = sessionID;
            LateralitySnoMedCode = laterality;
            CaptureSiteSnoMedCode = captureSiteSnoMedCode;
            Minutiae = FingerprintMinutiaConvertor.ConvertTemplate(template);
        }
    }
}
