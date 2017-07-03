// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Net;
using System.Security.Principal;
using System.Net.NetworkInformation;
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

        public static string GetUserName()
        {
            //TODO: Make this work for Linux, Mac, etc...
            return WindowsIdentity.GetCurrent().Name;
        }

        public static string GetComputerName()
        {
            // code from this stack exchange:
            // https://stackoverflow.com/questions/804700/how-to-find-fqdn-of-local-machine-in-c-net
            string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string hostName = Dns.GetHostName();

            domainName = "." + domainName;
            if (!hostName.EndsWith(domainName))  // if hostname does not already include domain name
            {
                hostName += domainName;   // add the domain name part
            }
            return hostName;
        }
    }
}
