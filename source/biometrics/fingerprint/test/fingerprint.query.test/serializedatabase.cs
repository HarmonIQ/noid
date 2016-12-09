using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using SourceAFIS.Templates;
using SourceAFIS.Extraction;
using System.IO;

namespace NoID.Biometrics
{
    class SerializeDatabaseProto
    {
        public SerializeDatabaseProto()
        {
        }

        ~SerializeDatabaseProto()
        {
        }

        public Exception ExceptionError { get; set; }
        public bool WriteToDisk(string databasePath, List<PatientFingerprintMinutia> listFingerprintMinutia)
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
                ExceptionError = e;
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

        public List<PatientFingerprintMinutia> ReadFromDisk(string path)
        {
            List<PatientFingerprintMinutia> readData = null;

            return readData;
        }
    }
}
