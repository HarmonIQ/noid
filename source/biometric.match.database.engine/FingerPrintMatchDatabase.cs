// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Configuration;
using NoID.Utilities;
using SourceAFIS.Templates;

namespace NoID.Match.Database.FingerPrint
{
    public class FingerPrintMatchDatabase
    {
        /*
         * TODO:
         * Should we make MATCH_THRESHOLD and MAX_CANDIDATE_CAPASITY 
         * a variable or keep it at a protocol level so all match nodes behave the same?
        */
        private readonly static int MATCH_THRESHOLD = 30;
        private readonly static int MAX_CANDIDATE_CAPACITY = 20000;
        private static string _databasePath;
        private MinutiaMatch _minutiaMatch;
        private readonly FHIRUtilities.LateralitySnoMedCode _laterality;
        private readonly FHIRUtilities.CaptureSiteSnoMedCode _captureSite;

        public FingerPrintMatchDatabase(string databaseLocation, string lateralityCode, string captureSiteCode)
        {
            _minutiaMatch = new MinutiaMatch(_databasePath, MATCH_THRESHOLD);
            try
            {
                _databasePath = databaseLocation;
                _laterality = FHIRUtilities.SnoMedCodeToLaterality(lateralityCode);
                _captureSite = FHIRUtilities.SnoMedCodeToCaptureSite(captureSiteCode);
            }
            catch { }
        }

        public int CandidateCount
        {
            get { return _minutiaMatch.CandidateCount; }
        }

        public int MatchThreshold
        {
            get { return MATCH_THRESHOLD; }
        }

        public int MaximumCandidateCapacity
        {
            get { return MAX_CANDIDATE_CAPACITY; }
        }

        public string DatabasePath
        {
            get { return _databasePath; }
        }

        public MinutiaMatch MinutiaMatch
        {
            get { return _minutiaMatch; }
        }

        public bool AddTemplate(Template template)
        {
            return _minutiaMatch.AddTemplate(template);
        }

        public string SearchPatients(Template probe)
        {
            string results;
            if (!(_minutiaMatch == null))
            {
                if (_minutiaMatch.CandidateCount > 0)
                {
                    results = _minutiaMatch.SearchPatients(probe, false);
                }
                else
                {
                    results = "Minutia Match Database is empty.";
                }
            }
            else
            {
                results = "Minutia Match Database is null.";
            }
            return results;
        }

        public bool WriteToDisk(string databasePath)
        {
            bool result = false;
            if (!(_minutiaMatch == null))
            {
                if (_minutiaMatch.CandidateCount > 0)
                {
                    result = _minutiaMatch.WriteToDisk(databasePath);
                }
            }
            return result;
        }

        public bool ReadFromDisk(string databasePath)
        {
            bool result = false;
            try
            {
                _minutiaMatch = new MinutiaMatch(_databasePath, MATCH_THRESHOLD);
                _minutiaMatch.ReadFromDisk(databasePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return result;
        }

        public bool UpdateTemplate(Template newBest, string NoID)
        {
            bool result = false;
            try
            {
                _minutiaMatch.UpdateTemplate(newBest, NoID);
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}
