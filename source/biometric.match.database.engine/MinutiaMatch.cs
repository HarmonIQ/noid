﻿// Copyright © 2016-2017 NoID Developers. All rights reserved.
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
        private const uint DATABASE_BACKUP_INTERVAL = 600;
        private static AfisEngine Afis = new AfisEngine();
        private readonly int _matchThreshold;
        private static List<Template> _fingerPrintCandidateList = new List<Template>();
        private DBreezeWrapper _dBreezeWrapper;
        private Extractor Extractor = new Extractor();
        private Exception _exception;

        public MinutiaMatch()
        {
            _matchThreshold = 30;
        }

        public MinutiaMatch(string databaseDirectoryPath, string databaseBackupPath, int matchTheshold)
        {
            _matchThreshold = matchTheshold;
            try
            {
                _dBreezeWrapper = new DBreezeWrapper(databaseDirectoryPath, databaseBackupPath, DATABASE_BACKUP_INTERVAL);
                _fingerPrintCandidateList = _dBreezeWrapper.GetMinutiaList();
            }
            catch(Exception ex)
            {
                Dispose();
                throw ex;
            }
        }

        ~MinutiaMatch()
        {
            Dispose();
        }

        public void Dispose()
        {
            try
            {
                _dBreezeWrapper.Dispose();
            }
            catch { }
        }

        public bool DeleteMatchDatabase()
        {
            return _dBreezeWrapper.DeleteMatchDatabase();
        }

        public Exception Exception
        {
            get { return _exception; }
            private set { _exception = value; }
        }

        public string DatabaseDirectoryPath
        {
            get
            {
                if (_dBreezeWrapper != null)
                {
                    return _dBreezeWrapper.DatabaseDirectory;
                }
                else
                {
                    return "";
                }
            }
        }

        public string DatabaseBackupPath
        {
            get
            {
                if (_dBreezeWrapper != null)
                {
                    return _dBreezeWrapper.BackupDirectoryPath;
                }
                else
                {
                    return "";
                }   
            }
        }

        public int MatchThreshold
        {
            get { return _matchThreshold; }
        }

        public int CandidateCount
        {
            get { return _fingerPrintCandidateList.Count; }
        }

        public MinutiaResult SearchPatients(Template templateProbe, bool fAddIfNotFound)
        {
            // Searches the minutias in the candidate list and if found returns the local identifier.
            return IdentifyFinger(templateProbe);
        }

        private MinutiaResult IdentifyFinger(Template probe)
        {
            MinutiaResult result = null;
            lock (this)
            {
                float[] scores = Afis.IdentifyFingers(probe, _fingerPrintCandidateList);
                if (scores.Length > 0)
                {
                    for (int i = 0; i < scores.Count(); i++)
                    {
                        if (scores[i] > _matchThreshold)
                        {
                            result = new MinutiaResult();
                            result.Score = scores[i];
                            result.NoID = _fingerPrintCandidateList[i].NoID.LocalNoID;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        public bool AddTemplate(Template template)
        {
            bool result = true;
            try
            {
                _fingerPrintCandidateList.Add(template);
                _dBreezeWrapper.AddMinutia(template);
            }
            catch
            {
                result = false;
            }
            return result;
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
                    scores = Matcher.Match(probeIndex, _fingerPrintCandidateList);

                    if (scores.Length > 0)
                    {
                        for (int i = 0; i < scores.Count(); i++)
                        {
                            if (scores[i] > _matchThreshold)
                            {
                                _NoID = _fingerPrintCandidateList[i].NoID.LocalNoID;
                                if (_NoID == NoID)
                                {
                                    newBest.NoID.LocalNoID = _NoID;
                                    _fingerPrintCandidateList[i] = newBest;
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
    }
}
