// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;

namespace NoID.Utilities
{
    public static class StringUtilities
    {
        public static string Reverse(string s)
        {
            char[] charArray = null;
            if (!(s == null) && s.Length > 0)
            {
                charArray = s.ToCharArray();
                Array.Reverse(charArray);
            }
            else
            {
                return "";
            }
            return new string(charArray);
        }

        public static string RemoveTrailingBackSlash(string s)
        {
            string newString;
            if (!(s == null) && s.Length > 0)
            {
                string lastChar = Reverse(s).Substring(0, 1);
                if (lastChar == "/")
                {
                    newString = s.Substring(0, s.Length - 1);
                }
                else
                {
                    newString = s;
                }
            }
            else
            {
                newString = "";
            }
            return newString;
        }
    }
}
