// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.
// Code ported from https://stackoverflow.com/questions/32548714/how-to-store-and-retrieve-credentials-on-windows-using-c-sharp

using System;
using System.Net;
using CredentialManagement;

namespace NoID.Security
{
    public static class PasswordManager
    {
        public static void SavePassword(string password, string targetName)
        {
            using (var cred = new Credential())
            {
                cred.Password = password;
                cred.Target = targetName;
                cred.Type = CredentialType.Generic;
                cred.PersistanceType = PersistanceType.LocalComputer;
                cred.Save();
            }
        }

        public static string GetPassword(string targetName)
        {
            using (var cred = new Credential())
            {
                cred.Target = targetName;
                cred.Load();
                return cred.Password;
            }
        }

        public static string GetCredentials()
        {
            return "";
        }
    }
}
