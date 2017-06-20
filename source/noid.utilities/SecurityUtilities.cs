// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using NoID.Security;

namespace NoID.Utilities
{
    public static class SecurityUtilities
    {
        public static Authentication GetAuthentication(string userName)
        {
            Authentication auth = null;
            string password = PasswordManager.GetPassword(userName);
            if (!(password == null) && password.Length > 0)
            {
                auth = new Authentication(userName, password);
            }
            else
            {
                //TODO. Better error handling.
                throw new Exception("Error in SecurityUtilities.GetAuthentication.  Password for " + userName + "  not set or is blank.");
            }
            return auth;
        }
    }
}
