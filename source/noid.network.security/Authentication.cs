// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Text;

namespace NoID.Network.Security
{
    public class Authentication
    {
        string _userName;
        string _password;

        public Authentication(string userName, string password)
        {
            _userName = userName;
            _password = password;
        }

        public string UserName
        {
            get { return _userName; }
        }

        public string Password
        {
            get { return _password; }
        }

        public string BasicAuthentication
        {
            get
            {
                return Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(_userName + ":" + _password));
            }
        }
    }
}
