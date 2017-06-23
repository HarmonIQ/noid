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
        private MinutiaMatch _minutiaMatch;
        private readonly FHIRUtilities.LateralitySnoMedCode _laterality;
        private readonly FHIRUtilities.CaptureSiteSnoMedCode _captureSite;

        public FingerPrintMatchDatabase(string databaseLocation, string datbaseBackupLocation, string lateralityCode, string captureSiteCode)
        {
            _minutiaMatch = new MinutiaMatch(databaseLocation, datbaseBackupLocation, MATCH_THRESHOLD);
            try
            {
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
            get
            {
                if (_minutiaMatch != null)
                {
                    return _minutiaMatch.DatabaseDirectoryPath;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public MinutiaMatch MinutiaMatch
        {
            get { return _minutiaMatch; }
        }

        public bool AddTemplate(Template template)
        {
            return _minutiaMatch.AddTemplate(template);
        }

        public MinutiaResult SearchPatients(Template probe)
        {
            MinutiaResult result = new MinutiaResult();
            if (!(_minutiaMatch == null))
            {
                if (_minutiaMatch.CandidateCount > 0)
                {
                    result = _minutiaMatch.SearchPatients(probe, false);
                }
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
