// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Configuration;
using SourceAFIS.Simple;

namespace NoID.Match.Database.Tests
{
    public partial class TestMatchEngineForm : Form
    {
        private readonly bool LOAD_TEST_FINGERPRINTS = true;
        private readonly string _databaseDirectory = ConfigurationManager.AppSettings["DatabaseLocation"].ToString();
        private readonly string _backupDatabaseDirectory = ConfigurationManager.AppSettings["BackupLocation"].ToString();
        private string _lateralityCode = ConfigurationManager.AppSettings["Laterality"].ToString();
        private string _captureSiteCode = ConfigurationManager.AppSettings["CaptureSite"].ToString();
        private MatchProbesTest _matchProbesTest;
        private TaskScheduler scheduler;
        private Fingerprint currentFinger;
        private Fingerprint bestFinger;
        private Fingerprint newMatchFinger;
        private string _errorMessage = "";
        private float highScore = 0;

        public TestMatchEngineForm()
        {
            InitializeComponent();
            scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _matchProbesTest = new MatchProbesTest(_databaseDirectory, _backupDatabaseDirectory, _lateralityCode, _captureSiteCode);
            // load test data into dbreeze database
            if (LOAD_TEST_FINGERPRINTS == true)
            {
                _matchProbesTest.LoadTestFingerPrintImages(@"F:\fingerprobes", false);
            }

            _matchProbesTest.FingerCaptured += FingerCaptured;
            _matchProbesTest.GoodPairFound += GoodPairFound;
            _matchProbesTest.NewBestMatchFound += NewBestMatchFound;
            _matchProbesTest.DatabaseMatchFound += DatabaseMatchFound;
            _matchProbesTest.DoesNotMatch += DoesNotMatch;
            _matchProbesTest.DatabaseMatchError += DatabaseMatchError;
            _matchProbesTest.PoorCaputure += PoorCaputure;
        }

        void FingerCaptured(object sender, EventArgs e)
        {
            currentFinger = (Fingerprint)sender;
            imageCurrentFinger.Image = currentFinger.AsBitmap;
            Task task = new Task(() => this.ClearScoreLabel());
            task.Start(scheduler);
        }

        void PoorCaputure(object sender, EventArgs e)
        {
            Task task = new Task(() => this.SetPoorCapture());
            task.Start(scheduler);
        }

        void GoodPairFound(object sender, EventArgs e)
        {
            bestFinger = (Fingerprint)sender;
            imageMatchedFinger.Image = bestFinger.AsBitmap;
            Task task = new Task(() => this.UpdateMatchScoreLabel());
            task.Start(scheduler);
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
            _errorMessage = "Critical match error occured!";
            Task task = new Task(() => this.UpdateErrorMessage());
            task.Start(scheduler);
        }

        private void SetPoorCapture()
        {
            labelBestScore.Text = "Poorly captured fingerprint.  Error code = " + (int)_matchProbesTest.Quality;
        }

        private void UpdateMatchScoreLabel()
        {
            labelBestScore.Text = "Match found! Score = " + _matchProbesTest.Score.ToString();

            if (_matchProbesTest.Score > highScore)
            {
                highScore = _matchProbesTest.Score;
                labelHighScore.Text = highScore.ToString();
            }
        }

        private void ClearScoreLabel()
        {
            labelBestScore.Text = "";
        }

        private void UpdateErrorMessage()
        {
            labelBestScore.Text = _errorMessage;
        }

        private void ScoreDoesNotMatch()
        {
            labelBestScore.Text = "Match NOT found! Score = " + _matchProbesTest.Score.ToString();
        }
    }
}
