// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.IO;
using System.Collections.Generic;
using ProtoBuf;
using SourceAFIS.Templates;
using NoID.FHIR.Profile;

namespace NoID.FHIR.Profile
{
    /// <summary cref="FingerPrintMinutiasSerialize">  
    /// Lightweight serializable list of minutias used for persisting data.
    /// </summary>  
    [ProtoContract]
    public abstract class FingerPrintMinutiasSerialize
    {
        public byte[] Serialize()
        {
            byte[] result;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, this);
                result = stream.ToArray();
            }
            return result;
        }
    }

    /// <summary cref="FingerprintMinutias">  
    /// Lightweight serializable list of minutias used for persisting data.
    /// </summary>  
    [ProtoContract]
    public class FingerPrintMinutias : FingerPrintMinutiasSerialize
    {
        [ProtoMember(1)]
        public string PatientCertificateID { get; private set; }

        [ProtoMember(2)]
        public PatientFHIRProfile.LateralitySnoMedCode LateralitySnoMedCode { get; private set; }

        [ProtoMember(3)]
        public PatientFHIRProfile.CaptureSiteSnoMedCode CaptureSiteSnoMedCode { get; private set; }

        [ProtoMember(4)]
        public List<FingerPrintMinutia> Minutiae { get; private set; }

        public FingerPrintMinutias(string patientCertificateID, Template template, PatientFHIRProfile.LateralitySnoMedCode laterality, PatientFHIRProfile.CaptureSiteSnoMedCode captureSiteSnoMedCode)
        {
            PatientCertificateID = patientCertificateID;
            LateralitySnoMedCode = laterality;
            CaptureSiteSnoMedCode = captureSiteSnoMedCode;
            Minutiae = FingerprintMinutiaConvertor.ConvertTemplate(template);
        }

        public static FingerPrintMinutias Deserialize(byte[] message)
        {
            FingerPrintMinutias result;
            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<FingerPrintMinutias>(stream);
            }
            return result;
        }
    }
}
