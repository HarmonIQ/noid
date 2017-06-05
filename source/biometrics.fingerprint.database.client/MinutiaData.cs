using System;

namespace NoID.Biometrics.Fingerprint
{
    public enum MinutiaDataType : byte
    {
        Ending = 0,
        Bifurcation = 1,
        Other = 2
    }

    public struct MinutiaData
    {
        public short X;
        public short Y;
        public byte Direction;
        public MinutiaDataType Type;
    }
}