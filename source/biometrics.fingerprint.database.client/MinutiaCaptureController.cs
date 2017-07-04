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
        private uint CAPTURE_ACCEPT_THRESHOLD = 75;
        private List<Template> _capturedFingerPrintMinutias = new List<Template>();
        private static AfisEngine _afis = new AfisEngine();
        private Template _bestTemplate1;
        private Template _bestTemplate2;
        private float _bestScore = 0;
        private float _lastScore = 0;
        private int _otherBestFingerprintItem;
        public bool MatchFound = false;

        public MinutiaCaptureController(uint minAcceptedMatchScore)
        {
            if (minAcceptedMatchScore != 0)
            {
                CAPTURE_ACCEPT_THRESHOLD = minAcceptedMatchScore;
            }
            _afis.Threshold = CAPTURE_ACCEPT_THRESHOLD;
        }

        ~MinutiaCaptureController() { }

        public bool AddMinutiaTemplateProbe(Template newFingerPrintTemplate)
        {
            bool fTemplatePairFound = false;
            _lastScore = 0;
            float[] scores = _afis.IdentifyFingers(newFingerPrintTemplate, _capturedFingerPrintMinutias);
            if (scores.Length > 0)
            {
                for (int i = 0; i < scores.Count(); i++)
                {
                    Template temp = _capturedFingerPrintMinutias[i];
                    float score = scores[i];
                    _lastScore = score;
                    if (score > CAPTURE_ACCEPT_THRESHOLD)
                    {
                        fTemplatePairFound = true;
                        if (score > _bestScore)
                        {
                            _bestScore = score;
                            _otherBestFingerprintItem = i;
                            _bestTemplate1 = newFingerPrintTemplate;
                            _bestTemplate2 = temp;
                        }
                        MatchFound = true;
                    }
                }
            }
            _capturedFingerPrintMinutias.Add(newFingerPrintTemplate);

            return fTemplatePairFound;
        }

        public bool MatchBest(Template probe)
        {
            bool result = false;

            if (BestTemplate1 != null && BestTemplate2 != null)
            {
                List<Template> bestFingerprints = new List<Template>();
                bestFingerprints.Add(BestTemplate1);
                bestFingerprints.Add(BestTemplate2);

                float[] scores = _afis.IdentifyFingers(probe, bestFingerprints);
                if (scores.Length > 0)
                {
                    for (int i = 0; i < scores.Count(); i++)
                    {
                        Template temp = _capturedFingerPrintMinutias[i];
                        float score = scores[i];
                        _lastScore = score;
                        if (score > 30)
                        {
                            result = true;
                            i = scores.Count() + 1;
                        }
                    }
                }
            }
            return result;
        }


        public List<Template> CapturedFingerPrintMinutias
        {
            get { return _capturedFingerPrintMinutias; }
        }

        public float BestScore
        {
            get { return _bestScore; }
        }

        public Template BestTemplate1
        {
            get { return _bestTemplate1; }
        }

        public Template BestTemplate2
        {
            get { return _bestTemplate2; }
        }

        public float LastScore
        {
            get { return _lastScore; }
        }

        public int OtherBestFingerprintItem
        {
           get { return _otherBestFingerprintItem; }
        }
    }
}
