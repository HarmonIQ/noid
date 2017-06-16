// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;
using NoID.FHIR.Profile;

namespace NoID.Match.Engine
{
    class SerializeDatabaseProto
    {
        public SerializeDatabaseProto()
        {
        }

        ~SerializeDatabaseProto()
        {
        }

        public Exception Exception { get; private set; }

        public bool WriteToDisk(string databasePath, List<FingerPrintMinutias> listFingerprintMinutia)
        {
            try
            {
                FileStream fs = OpenFileStream(databasePath);
                byte[] fingerData;
                using (var ms = new MemoryStream())
                {
                    Serializer.Serialize(ms, listFingerprintMinutia);
                    fingerData = ms.ToArray();
                }
                fs.Write(fingerData, 0, fingerData.Length);
                fs.Flush();
                fs.Close();
                fs.Dispose();
            }
            catch (Exception e)
            {
                Exception = e;
                return false;
            }
            return true;
        }
        private FileStream OpenFileStream(string path)
        {
            FileInfo fileInfo = DeleteFile(path);
            FileStream fs = fileInfo.OpenWrite();
            fileInfo = null;
            return fs;
        }
        private FileInfo DeleteFile(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
                fileInfo.Delete();
            return fileInfo;
        }

        public List<FingerPrintMinutias> ReadFromDisk(string path)
        {
            List<FingerPrintMinutias> readData = null;

            return readData;
        }
    }
}
