using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using somReporter.util.somReporter;

namespace somReporter
{
    /// <summary>
    /// Summary description for LineScoreReportTest
    /// </summary>
    [TestClass]
    public class LineScoreReportTest
    {
        public LineScoreReportTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private LeagueStandingsReport leagueStandingsReport;
        private LineScoreAndMinorsReport lineScoreReport;

        [TestInitialize()]
        public void Initialize()
        {
            Report.DATABASE.reset();
            SOMReportFile file = new SOMReportFile(Config.getConfigurationFile("ALL_REPORTS.PRT"));
            file.parseLeagueFile();
            leagueStandingsReport = (LeagueStandingsReport)file.FindReport("LEAGUE STANDINGS FOR");
            leagueStandingsReport.processReport(Program.LEAGUES[0].Length);

            lineScoreReport = (LineScoreAndMinorsReport)file.FindReport("INJURY/MINOR LEAGUE REPORT FOR");
            lineScoreReport.processReport(Program.LEAGUES[0].Length);
        }

        [TestMethod]
        public void TestLineScoreAreRead()
        {
            Team team = leagueStandingsReport.getTeamDataByName("New York");

            Assert.AreEqual(team.Wins+team.Loses, team.LineScores.Count);
        }
    }
}
