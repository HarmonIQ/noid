// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Drawing;

namespace NoID.Biometrics.Managers
{
    public class BiometricResult
    {
        public Bitmap BiometricImage;
        public float SampleQualityScore;
    }
}
