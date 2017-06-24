// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Linq;
using System.Collections.Generic;
using SourceAFIS.Simple;
using SourceAFIS.Templates;

namespace NoID.Match.Database.Client
{
    /// <summary>
    /// Helps capture the best set of minutia templates
    /// </summary>
    
    public class MinutiaCaptureController
    {
        private const uint CAPTURE_ACCEPT_THRESHOLD = 70;
        private List<Template> _controlledMinutiaList = new List<Template>();
        private static AfisEngine Afis = new AfisEngine();
        private float _highMatchScore = 0;
        public int OtherBestFingerprintItem;
        public bool MatchFound = false;

        public MinutiaCaptureController()
        {
            Afis.Threshold = CAPTURE_ACCEPT_THRESHOLD;
        }

        ~MinutiaCaptureController() { }

        public bool AddMinutiaTemplateProbe(Template newFingerPrintTemplate)
        {
            bool fTemplatePairFound = false;

            float[] scores = Afis.IdentifyFingers(newFingerPrintTemplate, _controlledMinutiaList);
            if (scores.Length > 0)
            {
                for (int i = 0; i < scores.Count(); i++)
                {
                    Template temp = _controlledMinutiaList[i];
                    float score = scores[i];
                    if (score > CAPTURE_ACCEPT_THRESHOLD)
                    {
                        fTemplatePairFound = true;
                        if (score > _highMatchScore)
                        {
                            _highMatchScore = score;
                            OtherBestFingerprintItem = i;
                        }
                        MatchFound = true;
                    }
                }
            }
            _controlledMinutiaList.Add(newFingerPrintTemplate);

            return fTemplatePairFound;
        }
    }
}
