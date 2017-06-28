﻿// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;

namespace NoID.Browser
{
    abstract class CEFBridge
    {
        string _organizationName;
        string _endPoint;
        string _serviceName;
        string _errorDescription;
        string _alertFunction = "";
        string _currentPage = "";

        public CEFBridge(string organizationName, string endPoint, string serviceName)
        {
            _organizationName = organizationName;
            _endPoint = endPoint;
            _serviceName = serviceName;
        }

        ~CEFBridge() { }

        public string organizationName
        {
            get { return _organizationName; }
        }

        public string endPoint
        {
            get { return _endPoint; }
        }

        public string serviceName
        {
            get { return _serviceName; }
        }

        public string errorDescription
        {
            get { return _errorDescription; }
            set { _errorDescription = value; }
        }
        public string alertFunction
        {
            get { return _alertFunction; }
            set { _alertFunction = value; }
        }

        public string currentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; }
        }
    }
}
