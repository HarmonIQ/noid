// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Configuration;
using NoID.Utilities;

namespace NoID.Match.Database.FingerPrint
{
    public class FingerPrintMatchDatabase
    {
        /*
         * TODO:
         * Should we make MATCH_THRESHOLD and MAX_CANDIDATE_CAPASITY 
         * a variable or keep it at a protocol level so all match nodes behave the same?
        */
        private readonly static int MATCH_THRESHOLD = 50;
        private readonly static int MAX_CANDIDATE_CAPACITY = 20000;
        private static string _databasePath = ConfigurationManager.AppSettings.Get("DatabaseLocation");
        private MinutiaMatch _minutiaMatch;
        private readonly FHIRUtilities.LateralitySnoMedCode _laterality;
        private readonly FHIRUtilities.CaptureSiteSnoMedCode _captureSite;

        public FingerPrintMatchDatabase()
        {
            _minutiaMatch = new MinutiaMatch(_databasePath, MATCH_THRESHOLD);
            string Laterality = "0";
            string CaptureSite = "0";
            try
            {
                Laterality = ConfigurationManager.AppSettings.Get("Laterality");
                CaptureSite = ConfigurationManager.AppSettings.Get("CaptureSite");
            }
            catch { }
            _laterality = FHIRUtilities.SnoMedCodeToLaterality(Laterality);
            _captureSite = FHIRUtilities.SnoMedCodeToCaptureSite(CaptureSite);
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
    }
}
