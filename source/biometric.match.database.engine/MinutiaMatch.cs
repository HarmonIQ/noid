// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Linq;
using System.Collections.Generic;
using SourceAFIS.Simple;
using SourceAFIS.Templates;
using SourceAFIS.Extraction;
using SourceAFIS.Matching;

namespace NoID.Match.Database.FingerPrint
{
    public class MinutiaMatch
    {
        private readonly int _matchThreshold;
        private readonly string _databaseFilePath;
        private Extractor Extractor = new Extractor();
        private Exception _exception;

        public MinutiaMatch(string databaseFilePath, int matchTheshold)
        {
            _databaseFilePath = databaseFilePath;
            _matchThreshold = matchTheshold;
        }

        private List<Template> FingerPrintCandidateList
        {
            get { return MatchDatabase.FingerPrintCandidateList; }
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
