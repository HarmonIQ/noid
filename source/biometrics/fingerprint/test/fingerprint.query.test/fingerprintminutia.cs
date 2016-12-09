using System;
using ProtoBuf;
using System.IO;

namespace NoID.Biometrics
{
    /// <summary cref="FingerPrintMinutiaSerialize">  
    /// Lightweight fingerprint minutia objected used for persisting.
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
    /// Lightweight fingerprint minutia objected used for persisting.
    /// </summary>  
    [ProtoContract]
    public class FingerPrintMinutia : FingerPrintMinutiaSerialize
    {
        [ProtoMember(1)]
        public short PositionX { get; private set; }
        [ProtoMember(2)]
        public short PositionY { get; private set; }
        [ProtoMember(3)]
        public byte Direction { get; private set; }
        [ProtoMember(4)]
        public byte Type { get; private set; }  //Ending = 0,Bifurcation = 1,Other = 2

        public FingerPrintMinutia()
        { }

        public FingerPrintMinutia(short x, short y, byte direction, byte type)
        {
            PositionX = x;
            PositionY = y;
            Direction = direction;
            Type = type;
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
