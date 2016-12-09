using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoID.Biometrics
{
    class FingerprintMinutiae : ICloneable
    {
        private byte[] _template = null;
        public FingerprintMinutiae() { }
        public FingerprintMinutiae(byte[] template)
        {
            _template = template;
        }
        ~FingerprintMinutiae() { }

        public byte[] Template
        {
            get { return _template; }
            set { _template = value; }
        }

        /// <summary>
        /// Create deep copy of the <see cref="Fingerprint"/>.
        /// </summary>
        /// <returns>Deep copy of this <see cref="Fingerprint"/>.</returns>
        public FingerprintMinutiae Clone()
        {
            return new FingerprintMinutiae(_template);
        }

        object ICloneable.Clone() { return Clone(); }
    }
}
