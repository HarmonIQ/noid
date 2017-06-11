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
