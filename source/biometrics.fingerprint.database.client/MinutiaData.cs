using System;

namespace NoID.Biometrics.Fingerprint
{
    public enum MinutiaType : byte
    {
        Ending = 0,
        Bifurcation = 1,
        Other = 2
    }

    public struct Minutia
    {
        public short X;
        public short Y;
        public byte Direction;
        public MinutiaType Type;
    }
}