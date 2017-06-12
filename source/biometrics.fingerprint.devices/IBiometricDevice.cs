// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using DPUruNet;
using System;

namespace NoID.Biometrics.Managers
{
    public interface IBiometricDevice
    {
        BiometricDevice[] ListDevices();

        BiometricDevice GetDevice(string name);

        BiometricDevice GetDefaultDevice();

        void OnBiometricCaptured(BiometricResult captureResult);
    }
}
