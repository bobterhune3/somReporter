using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using somReporter.util.somReporter;

namespace somReporter.team
{
    [TestClass]
    public class TestTeamReportLoader
    {
        private SOMReportFile teamReportFile;

        [TestInitialize()]
        public void Initialize()
        {
            teamReportFile = new SOMReportFile(Config.getConfigurationFile("TEAM_ALL_REPORTS.PRT"));
        }

        [TestCleanup()]
        public void Cleanup() { }

        [TestMethod]
        public void verifyTheFileLoaded()
        {
            Assert.IsNotNull(teamReportFile, "SOM Team Report File is null");
        }

        [TestMethod]
        public void testLoader()
        {
            teamReportFile.parseTeamFile();
            ComparisonReport teamComparisonReport = (ComparisonReport)teamReportFile.FindReport("Comparison Report");
            teamComparisonReport.processReport(Program.LEAGUES[0].Length);
        }

        [TestMethod]
        public void testParsingCompanionReport() {
            int length = Program.LEAGUES[0].Length;
            ComparisonReport teamComparisonReport = new ComparisonReport("COMPARISON");
            teamComparisonReport.collectData("TEAM","--AVERAGE-- AT-BATS DOUBLES TRIPLES --HRs-- -RBIS-- -WALKS- --Ks---", length);
            teamComparisonReport.collectData("TEAM", " ", length);
            teamComparisonReport.collectData("TEAM", "R.Flaherty [4] .202  .195 267 307   8  10   3   0   9  10  31  29  26  29  81  86", length);
            teamComparisonReport.collectData("TEAM", "P.Goldschmi[4] .321  .303 567 580  38  33   2   3  33  21 110  80 118  91 151 171", length);
            teamComparisonReport.collectData("TEAM", "S.Van Slyke[4] .219  .233 411 420  14  16   0   0   8   9  37  34  20  27  88  86", length);
            teamComparisonReport.collectData("TEAM", "----ERA---- -WINS-- -LOSS-- -SAVES- INNINGS -HITS-- -WALKS- --Ks---", length);
            teamComparisonReport.collectData("TEAM", "NAME ACT   REP ACT REP ACT REP ACT REP ACT REP ACT REP ACT REP ACT REP", length);
            teamComparisonReport.collectData("TEAM", "J.Garcia   [4] 2.43  2.82  10   8   6   6   0   0 130 153 106 138  30  46  97 122", length);
            teamComparisonReport.collectData("TEAM", "H.Iwakuma[4] 3.54  2.68   9  11   5   7   0   0 130 158 117 120  21  39 111 143", length);
            teamComparisonReport.collectData("TEAM", "", length);
        }

        [TestMethod]
        public void testHitter()
        {
            ComparisonReport teamComparisonReport = new ComparisonReport("COMPANION");
            int length = Program.LEAGUES[0].Length;

            teamComparisonReport.collectData("TEAM", "R.Flaherty [4] .202  .195 267 307   8  10   3   0   9  10  31  29  26  29  81  86", length);
            List<Player> player = teamComparisonReport.getPlayers();

            Assert.IsNotNull(player);
            Assert.IsTrue(player.Count == 1);

            Player p = player[0];
            Assert.AreEqual("R.Flaherty", p.Name);
            Assert.AreEqual(267, p.Actual);
            Assert.AreEqual(307, p.Replay);
            Assert.AreEqual(1.15, p.Usage);
            Assert.AreEqual("TEAM", p.Team.Abrv);
   
            /*
            teamComparisonReport.collectData("TEAM", "P.Goldschmi[4] .321  .303 567 580  38  33   2   3  33  21 110  80 118  91 151 171");
            teamComparisonReport.collectData("TEAM", "S.Van Slyke[4] .219  .233 411 420  14  16   0   0   8   9  37  34  20  27  88  86");
             teamComparisonReport.collectData("TEAM", "J.Garcia   [4] 2.43  2.82  10   8   6   6   0   0 130 153 106 138  30  46  97 122");
            teamComparisonReport.collectData("TEAM", "");
            */
        }

        [TestMethod]
        public void testPitcher()
        {
            int length = Program.LEAGUES[0].Length;

            ComparisonReport teamComparisonReport = new ComparisonReport("COMPANION");
            teamComparisonReport.collectData("TEAM", "----ERA---- -WINS-- -LOSS-- -SAVES- INNINGS -HITS-- -WALKS- --Ks---", length);
            teamComparisonReport.collectData("TEAM", "H.Iwakuma[4] 3.54  2.68   9  11   5   7   0   0 130 158 117 120  21  39 111 143", length);
            List<Player> player = teamComparisonReport.getPlayers();

            Assert.IsNotNull(player);
            Assert.IsTrue(player.Count == 1);

            Player p = player[0];
            Assert.AreEqual("H.Iwakuma", p.Name);
            Assert.AreEqual(130, p.Actual);
            Assert.AreEqual(158, p.Replay);
            Assert.AreEqual(1.22, p.Usage);
            Assert.AreEqual("TEAM", p.Team.Abrv);
         }

        [TestMethod]
        public void testPitcherNotUsed()
        {
            int length = Program.LEAGUES[0].Length;

            ComparisonReport teamComparisonReport = new ComparisonReport("COMPANION");
            teamComparisonReport.collectData("TEAM", "----ERA---- -WINS-- -LOSS-- -SAVES- INNINGS -HITS-- -WALKS- --Ks---", length);
            teamComparisonReport.collectData("TEAM", "D.Holland[4] 4.91   ---   4   0   3   0   0   0  59   0  59   0  17   0  41   0", length);
            List<Player> player = teamComparisonReport.getPlayers();

            Assert.IsNotNull(player);
            Assert.IsTrue(player.Count == 1);

            Player p = player[0];
            Assert.AreEqual("D.Holland", p.Name);
            Assert.AreEqual(59, p.Actual);
            Assert.AreEqual(0, p.Replay);
            Assert.AreEqual(0, p.Usage);
            Assert.AreEqual("TEAM", p.Team.Abrv);
        }

        [TestMethod]
        public void testPitcherLowERA()
        {
            int length = Program.LEAGUES[0].Length;

            ComparisonReport teamComparisonReport = new ComparisonReport("COMPANION");
            teamComparisonReport.collectData("TEAM", "----ERA---- -WINS-- -LOSS-- -SAVES- INNINGS -HITS-- -WALKS- --Ks---", length);
            teamComparisonReport.collectData("TEAM", "J.Manship[4]  .92   .52   1   5   0   3   0   0  39  52  20  29  10  14  33  44", length);
            List<Player> player = teamComparisonReport.getPlayers();

            Assert.IsNotNull(player);
            Assert.IsTrue(player.Count == 1);

            Player p = player[0];
            Assert.AreEqual("J.Manship", p.Name);
            Assert.AreEqual(39, p.Actual);
            Assert.AreEqual(52, p.Replay);
            Assert.AreEqual(1.33, p.Usage);
            Assert.AreEqual("TEAM", p.Team.Abrv);
        }

        [TestMethod]
        public void testPitcherHighERA()
        {
            int length = Program.LEAGUES[0].Length;

            ComparisonReport teamComparisonReport = new ComparisonReport("COMPANION");
            teamComparisonReport.collectData("TEAM", "----ERA---- -WINS-- -LOSS-- -SAVES- INNINGS -HITS-- -WALKS- --Ks---", length);
            teamComparisonReport.collectData("TEAM", "H.Santiago[4] 4.70 10.12  13   0  10   7   0   0 182  35 169  46  79  20 144  28", length);
            List<Player> player = teamComparisonReport.getPlayers();

            Assert.IsNotNull(player);
            Assert.IsTrue(player.Count == 1);

            Player p = player[0];
            Assert.AreEqual("H.Santiago", p.Name);
            Assert.AreEqual(182, p.Actual);
            Assert.AreEqual(35, p.Replay);
       //     Assert.AreEqual(1.33, p.Usage);
            Assert.AreEqual("TEAM", p.Team.Abrv);
        }

        
        
    }
}