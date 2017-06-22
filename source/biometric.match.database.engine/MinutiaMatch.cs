// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ProtoBuf;
using SourceAFIS.Templates;
using SourceAFIS.Extraction;
using SourceAFIS.Matching;

namespace NoID.Match.Database.FingerPrint
{
    [ProtoContract]
    public class MinutiaMatch : MinutiaMatchSerialize
    {
        private readonly int _matchThreshold;
        private readonly string _databaseFilePath;
        private Extractor Extractor = new Extractor();
        private Exception _exception;

        public MinutiaMatch()
        {
            _databaseFilePath = "";
            _matchThreshold = 30;
        }

        public MinutiaMatch(string databaseFilePath, int matchTheshold)
        {
            _databaseFilePath = databaseFilePath;
            _matchThreshold = matchTheshold;
        }

        [ProtoMember(1)]
        public List<Template> FingerPrintCandidateList
        {
            get { return MatchDatabase.FingerPrintCandidateList; }
            private set { MatchDatabase.FingerPrintCandidateList = value; }
        }

        ~MinutiaMatch(){ }

        public Exception Exception
        {
            get { return _exception; }
            private set { _exception = value; }
        }

        public string DatabaseFilePath
        {
            get { return _databaseFilePath; }
        }

        public int MatchThreshold
        {
            get { return _matchThreshold; }
        }

        public int CandidateCount
        {
            get { return FingerPrintCandidateList.Count; }
        }

        public string SearchPatients(Template templateProbe, bool fAddIfNotFound)
        {
            // Searches the minutias in the candidate list and if found returns the local identifier.
            // Returns a blank string if not found.
            string patientCertificateID = IdentifyFinger(templateProbe);
            return patientCertificateID;
        }

        private string IdentifyFinger(Template probe)
        {
            string NoID = "";
            float[] scores;
            lock (this)
            {
                ParallelMatcher Matcher = new ParallelMatcher();
                ParallelMatcher.PreparedProbe probeIndex = Matcher.Prepare(probe);
                scores = Matcher.Match(probeIndex, FingerPrintCandidateList);

                if (scores.Length > 0)
                {
                    for (int i = 0; i < scores.Count(); i++)
                    {
                        if (scores[i] > _matchThreshold)
                        {
                            NoID = FingerPrintCandidateList[i].NoID;
                            break;
                        }
                    }
                }
            }
            return NoID;
        }

        public bool AddTemplate(Template template)
        {
            bool result = true;
            try
            {
                FingerPrintCandidateList.Add(template);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public static MinutiaMatch Deserialize(byte[] message)
        {
            MinutiaMatch result;
            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<MinutiaMatch>(stream);
            }
            return result;
        }

        public bool WriteToDisk(string databasePath)
        {
            try
            {
                FileStream fs = OpenFileStream(databasePath);
                byte[] fingerData;
                using (var ms = new MemoryStream())
                {
                    fingerData = Serialize();
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

        public bool ReadFromDisk(string path)
        {
            bool result = false;
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                FileStream fs = fileInfo.OpenRead();
                byte[] databaseBytes = ReadFully(fs);
                MinutiaMatch deserializeMinutiaMatch = Deserialize(databaseBytes);
                FingerPrintCandidateList = deserializeMinutiaMatch.FingerPrintCandidateList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        static byte[] ReadFully(Stream input)
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

        public bool UpdateTemplate(Template newBest, string NoID)
        {
            string _NoID = "";
            float[] scores;
            bool result = false;
            try
            {
                lock (this)
                {
                    ParallelMatcher Matcher = new ParallelMatcher();
                    ParallelMatcher.PreparedProbe probeIndex = Matcher.Prepare(newBest);
                    scores = Matcher.Match(probeIndex, FingerPrintCandidateList);

                    if (scores.Length > 0)
                    {
                        for (int i = 0; i < scores.Count(); i++)
                        {
                            if (scores[i] > _matchThreshold)
                            {
                                _NoID = FingerPrintCandidateList[i].NoID;
                                if (_NoID == NoID)
                                {
                                    FingerPrintCandidateList[i] = newBest;
                                    break;
                                }
                            }
                        }
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
            return result;
        }
        /*
        List<int> FlattenHierarchy(List<Template> templateList)
        {
            int n = 0;
            List<int> intList = new List<int>();
            foreach (Template var in templateList)
            {
                intList.Add(n);
                n++;
            }
            return intList;
        }
        */
    }

    internal static class MatchDatabase
    {
        private static List<Template> _fingerPrintCandidateList = new List<Template>();
        private static ushort _synTimeOutSeconds = 600;

        // synchronize fingerprints from the database into a new list
        // mutex lock when updating the list object
        // queue pending searches when locked and run the queue when unlocked.

        public static List<Template> FingerPrintCandidateList
        {
            get { return _fingerPrintCandidateList; }
            set { _fingerPrintCandidateList = value; }
        }

        public static ushort SynTimeOutSeconds
        {
            get { return _synTimeOutSeconds; }
            set { _synTimeOutSeconds = value; }
        }

        public static List<Template> CloneFingerPrintList
        {
            get { return CloneListTemplate(); }
        }

        private static List<Template> CloneListTemplate()
        {
            // deep clone here
            // check for mutex lock
            // queue if locked.
            return _fingerPrintCandidateList;
        }
    }
}
