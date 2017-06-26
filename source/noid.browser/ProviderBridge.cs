// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using NoID.FHIR.Profile;
using NoID.Security;
using NoID.Utilities;
using NoID.Network.Transport;

namespace NoID.Browser
{
    class ProviderBridge : CEFBridge
    {
        /// <summary>Binds JavaScript in the CEF browser with this .NET framework class.
        /// <para> Used to handle the Provider UI and briges calls between CEF and .NET.</para>
        /// <para> Uses JavaScript naming because CEF renames them anyways.</para>
        /// <para> Use CEFBridge as a base class and contains</para>
        /// <para> organizationName, endPoint, serviceName, errorDescription, alertFunction</para>
        /// <seealso cref="CEFBridge"/>
        /// </summary>

        private IList<PatientProfile> _patients;

        public ProviderBridge(string organizationName, Uri endPoint, string serviceName) : base(organizationName, endPoint, serviceName)
        {
            _patients = TestPatientList.GetTestPatients(organizationName);
        }

        ~ProviderBridge() { }

        public IList<PatientProfile> patients
        {
            get { return _patients; }
        }
    }
}
