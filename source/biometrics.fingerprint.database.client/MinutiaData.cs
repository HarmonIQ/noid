// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

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