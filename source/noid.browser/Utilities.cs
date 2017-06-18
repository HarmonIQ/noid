using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoID.Browser
{
    public static class Utilities
    {
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static string RemoveTrailingBackSlash(string s)
        {
            string newString = s;
            string lastChar = Reverse(s).Substring(0, 1);
            if (lastChar == "/")
            {
                newString = newString.Substring(0, newString.Length - 1);
            }
            return newString;
        }
    }
}
