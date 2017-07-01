// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;

namespace NoID.Browser
{
    abstract class CEFBridge
    {
        string _organizationName;
        string _serviceName;
        string _errorDescription;
        string _alertFunction = "";
        string _currentPage = "";
		string _showExceptionModal = "";


		public CEFBridge(string organizationName, string serviceName)
        {
            _organizationName = organizationName;
            _serviceName = serviceName;
        }

        ~CEFBridge() { }

        public string organizationName
        {
            get { return _organizationName; }
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
		public string showExceptionModal
		{
			get { return _showExceptionModal; }
			set { _showExceptionModal = value; }
		}
	}
}
