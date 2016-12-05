using System.IO;
using SourceAFIS.Simple;
using SourceAFIS.General;
using System.Collections.Generic;

namespace NoID.Biometrics
{
    class MatchProbes
    {

        List<Fingerprint> _probeList = new List<Fingerprint>();
        public int _ordinal = -1;
        private Fingerprint _currentProbe = null;

        public MatchProbes()
        {
        }

        ~MatchProbes()
        {
        }

        public Fingerprint GetNextProbe()
        {
            if (_ordinal < 0)
                return null;

            if (_ordinal > _probeList.Count)
                _ordinal = 0; //start over

            _currentProbe = _probeList[_ordinal];
            ++_ordinal;

            return _currentProbe;
        }

        public void LoadProbeImages(string imageDirectory)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(imageDirectory);
            Fingerprint fingerprint = null;
            if (dirInfo.Exists)
            {
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    fingerprint = new Fingerprint();
                    fingerprint.AsBitmapSource = WpfIO.Load(file.FullName);
                    if (fingerprint.Image != null)
                    {
                        _probeList.Add(fingerprint);
                    }
                }
                if (_probeList.Count > 0)
                {
                    _currentProbe = _probeList[0];
                    _ordinal = 1;
                }
            }
        }
    }
}
