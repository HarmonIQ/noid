// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.IO;
using System.Text;

namespace NoID.Utilities
{
    public static class ByteUtilities
    {
        public static byte[] StreamToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static byte[] StringToByteArray(string input)
        {
            return Encoding.ASCII.GetBytes(input);
        }

        public static string ByteArrayToString(byte[] input)
        {
            return Encoding.UTF8.GetString(input);
        }

        public static void ResetStreamReaderToBegining(StreamReader inputStreamReader, out Stream outputStream)
        {
            //resets the StreamReader to the begining
            inputStreamReader.BaseStream.Position = 0;
            inputStreamReader.DiscardBufferedData();
            outputStream = inputStreamReader.BaseStream;
        }
    }
}
