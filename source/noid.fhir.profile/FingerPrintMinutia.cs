using ProtoBuf;
using System.IO;

namespace NoID.FHIR.Profile
{
    /// <summary cref="FingerPrintMinutiaSerialize">  
    /// Lightweight object used for transport and persisting a fingerprint minutia.
    /// </summary>  
    public abstract class FingerPrintMinutiaSerialize
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
    /// <summary cref="FingerPrintMinutia">  
    /// Lightweight object used for transport and persisting a fingerprint minutia template.
    /// </summary>  
    [ProtoContract]
    public class FingerPrintMinutia : FingerPrintMinutiaSerialize
    {
        public enum MinutiaType
        {
            Ending = 0,
            Bifurcation = 1,
            Other = 2
        }

        [ProtoMember(1)]
        public short PositionX { get; private set; }
        [ProtoMember(2)]
        public short PositionY { get; private set; }
        [ProtoMember(3)]
        public short Direction { get; private set; }
        [ProtoMember(4)]
        public MinutiaType Type { get; private set; }  //Ending = 0,Bifurcation = 1,Other = 2

        [ProtoMember(5)]
        public string PatientPointer { get; private set; }

        public FingerPrintMinutia()
        { }

        public FingerPrintMinutia(short x, short y, short direction, MinutiaType type, string referencePointer)
        {
            PositionX = x;
            PositionY = y;
            Direction = direction;
            Type = type;
            PatientPointer = referencePointer;
        }

        public static FingerPrintMinutia Deserialize(byte[] message)
        {
            FingerPrintMinutia result;
            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<FingerPrintMinutia>(stream);
            }
            return result;
        }
    }
}