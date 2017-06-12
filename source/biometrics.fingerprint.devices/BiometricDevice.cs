// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

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
