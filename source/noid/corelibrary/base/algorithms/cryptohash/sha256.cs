// Copyright (c) 2016 NoID Developers
// Distributed under the MIT/X11 software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Security.Cryptography;

namespace NoID.Base.Algorithms
{
    public class sha256
    {
        private string _hash;

        public sha256()
        {
        }

        public sha256(string input, string salt)
        {
            _hash = SHA256Hash (input, salt);
        }

        public string SHA256Hash(string input, string salt)
        {
            SHA256Managed shaM = new SHA256Managed();
            _hash = Convert.ToBase64String(shaM.ComputeHash(GetBytes(string.Concat(salt, input))));
            return _hash;
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public string Hash { get { return _hash;}  }
    }
}