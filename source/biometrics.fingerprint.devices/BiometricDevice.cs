using System;

namespace NoID.Biometrics.Managers
{
    public abstract class BiometricDevice
    {
        public virtual string Name { get { return ""; } }
        public virtual string Manufacturer { get { return ""; } }
        public virtual string VendorID { get { return ""; } } //for USB devices
        public virtual string ProductID { get { return ""; } } //for USB devices
        public virtual string Driver { get { return ""; } }
        public virtual string DriverLocation { get { return ""; } }
        public virtual int Count { get { return -2; } }

    }
}
