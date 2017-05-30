using Liphsoft.Crypto.Argon2;
using System.Security.Cryptography;
using System.Text;

namespace NoID.Cryptographic.Hash
{
    public class HashWriter
    {
        public struct ArgonParams
        {
            public uint TimeCost; //in milliseconds
            public uint MemoryCost; //in kilobytes
            public uint ParallelLanes; //degree of parallelism

            //defaults to time cost = 2, memory cost = 8192, lanes = 4
            public ArgonParams(uint _timeCost = 2, uint _memoryCost = 8192, uint _parallelLanes = 4)
            {
                this.TimeCost = _timeCost;
                this.MemoryCost = _memoryCost;
                this.ParallelLanes = _parallelLanes;
            }
        }

        public static string Hash(string _value, string _salt, ArgonParams _argonParams)
        {
            return sha256Hash(string.Concat(_salt, argon2iHash(_value, _salt, _argonParams), _salt));
        }

        private static string argon2iHash(string _value, string _salt, ArgonParams _argonParams)
        {
            PasswordHasher ArgonHasher = new PasswordHasher(_argonParams.TimeCost, _argonParams.MemoryCost, _argonParams.ParallelLanes, Argon2Type.Argon2i, 256);
            return System.Convert.ToBase64String(ArgonHasher.HashRaw(_value, _salt));
        }

        private static string sha256Hash(string _value)
        {
            SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(_value), 0, Encoding.UTF8.GetByteCount(_value));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
