using System;
using SourceAFIS.Templates;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;

namespace NoID.Biometrics
{
    /// <summary cref="PatientFingerprintMinutiaSerialize">  
    /// Lightweight serializable list of minutias used for persisting data.
    /// </summary>  
    [ProtoContract]
    public abstract class PatientFingerprintMinutiaSerialize
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
    /// <summary cref="PatientFingerprintMinutia">  
    /// Lightweight serializable list of minutias used for persisting data.
    /// </summary>  
    [ProtoContract]
    public class PatientFingerprintMinutia : PatientFingerprintMinutiaSerialize
    {
        [ProtoMember(1)]
        public ulong CertificateID { get; private set; }
        [ProtoMember(2)]
        public ushort FingerOrdinal { get; private set; }
        [ProtoMember(3)]
        public List<FingerPrintMinutia> Minutiae { get; private set; }
        
        public PatientFingerprintMinutia()
        { }

        public PatientFingerprintMinutia(ulong certificateID, ushort fingerOrdinal, Template template)
        {
            CertificateID = certificateID;
            FingerOrdinal = fingerOrdinal;
            Minutiae = FingerprintMinutiaConvertor.ConvertTemplate(template);
        }

        public static PatientFingerprintMinutia Deserialize(byte[] message)
        {
            PatientFingerprintMinutia result;
            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<PatientFingerprintMinutia>(stream);
            }
            return result;
        }
    }
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

        static Template ConvertTemplate(PatientFingerprintMinutia template, ulong certificateID, ushort fingerOrdinal)
        {
            TemplateBuilder tb = new TemplateBuilder();
            Template convertedTemplate = new Template(tb);

            return convertedTemplate;
        }
    }

}
