// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Windows.Forms;
using SourceAFIS.Templates;
using SourceAFIS.Simple;
using System.Threading.Tasks;

namespace NoID.Match.Database.Tests
{
    public partial class TestMatchEngineForm : Form
    {
        private MatchProbesTest matchProbesTest = new MatchProbesTest();
        private TaskScheduler scheduler;
        private Fingerprint currentFinger;
        private Fingerprint bestFinger;
        private Fingerprint newMatchFinger;
        private string ErrorMessage = "";

        public TestMatchEngineForm()
        {
            InitializeComponent();
            scheduler = TaskScheduler.FromCurrentSynchronizationContext();

            matchProbesTest.LoadTestFingerPrintImages(@"F:\fingerprobes", true);
            matchProbesTest.FingerCaptured += FingerCaptured;
            matchProbesTest.GoodPairFound += GoodPairFound;
            matchProbesTest.NewBestMatchFound += NewBestMatchFound;
            matchProbesTest.DatabaseMatchFound += DatabaseMatchFound;
            matchProbesTest.DoesNotMatch += DoesNotMatch;
            matchProbesTest.DatabaseMatchError += DatabaseMatchError;
        }

        void FingerCaptured(object sender, EventArgs e)
        {
            currentFinger = (Fingerprint)sender;
            imageCurrentFinger.Image = currentFinger.AsBitmap;
            Task task = new Task(() => this.ClearScoreLabel());
            task.Start(scheduler);
        }

        void GoodPairFound(object sender, EventArgs e)
        {
            bestFinger = (Fingerprint)sender;
            imageMatchedFinger.Image = bestFinger.AsBitmap;
            Task task = new Task(() => this.UpdateMatchScoreLabel());
            task.Start(scheduler);
        }

        private void UpdateMatchScoreLabel()
        {
            labelBestScore.Text = "Match found! Score = " +  matchProbesTest.Score.ToString();
        }

        private void ClearScoreLabel()
        {
            labelBestScore.Text = "";
        }

        private void UpdateErrorMessage()
        {
            labelBestScore.Text = ErrorMessage;
        }

        private void ScoreDoesNotMatch()
        {
            labelBestScore.Text = "Match NOT found! Score = " + matchProbesTest.Score.ToString();
        }

        void DoesNotMatch(object sender, EventArgs e)
        {
            Task task = new Task(() => this.ScoreDoesNotMatch());
            task.Start(scheduler);
        }

        void NewBestMatchFound(object sender, EventArgs e)
        {
            bestFinger = (Fingerprint)sender;
            imageBestFinger.Image = bestFinger.AsBitmap;
        }

        void DatabaseMatchFound(object sender, EventArgs e)
        {
            newMatchFinger = (Fingerprint)sender;
            imageMatchedFinger.Image = newMatchFinger.AsBitmap;
            Task task = new Task(() => this.UpdateMatchScoreLabel());
            task.Start(scheduler);
        }

        void DatabaseMatchError(object sender, EventArgs e)
        {
            imageMatchedFinger.Image = null;
            ErrorMessage = "Critical match error occured!";
            Task task = new Task(() => this.UpdateErrorMessage());
            task.Start(scheduler);
        }
    }
}
