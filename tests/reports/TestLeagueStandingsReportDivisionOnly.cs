using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using somReporter.util.somReporter;

namespace somReporter
{
    [TestClass()]
    public class TestLeagueStandingsReportDivisionOnly
    {
        private SOMReportFile file;
        private LeagueStandingsReport leagueStandingsReport;

        [TestInitialize()]
        public void Initialize()
        {
            Config.PRT_FILE_LOCATION = "testData";
            Config.LEAGUE_NAME = "";
            Report.DATABASE.reset();
            string [] leagues = { "" };
            Program.LEAGUES = leagues;
            file = new SOMReportFile(Config.getConfigurationFile("ALL_REPORTS.PRT"));
            file.parseLeagueFile();
            leagueStandingsReport = (LeagueStandingsReport)file.FindReport("LEAGUE STANDINGS FOR");
            leagueStandingsReport.processReport(Program.LEAGUES[0].Length);
        }

        [TestCleanup()]
        public void Cleanup() { }

        //broken
        [TestMethod()]
        public void lookupTeamByName()
        {
            Team team = leagueStandingsReport.getTeamDataByName("New York");
            Assert.AreEqual("New York", team.Name);
            Assert.AreEqual("NYB", team.Abrv);
            Assert.AreEqual(92, team.Wins);
            Assert.AreEqual(70, team.Loses);
            Assert.AreEqual(1.0, team.Gb);
        //    Assert.AreEqual("American", team.Full_div);
            Assert.AreEqual("", team.League);
            Assert.AreEqual("Federal", team.Division);

            Assert.IsNull(leagueStandingsReport.getTeamDataByName("BAD NAME"));
        }

        //broken
        [TestMethod()]
        public void getTeamsByDivision()
        {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.League = "";
            scope.Division = "Federal";
            
            List<Team> teams = leagueStandingsReport.getTeamsByWinPercentage(scope);
            Assert.AreEqual(6, teams.Count);
        }

        //broken
    /*    [TestMethod()]
        public void getTeamsByLeague()
        {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.League = "";

            List<Team> teams = leagueStandingsReport.getTeamsByWinPercentage(scope);
            Assert.AreEqual(18, teams.Count);
        }
        */
        //broken
        [TestMethod()]
        public void getSortedTeamsAscendingWPctByLeague()
        {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.League = "";
            scope.Division = "American";
            scope.OrderAscending = true;

            List<Team> teams = leagueStandingsReport.getTeamsByWinPercentage(scope);
            Assert.AreEqual(6, teams.Count);

            double lastWpct = -1;
            foreach( Team team in teams )
            {
                if (lastWpct == -1)
                    lastWpct = team.Wpct;
                else {
                    Assert.IsTrue(team.Wpct >= lastWpct);
                    Debug.WriteLine(team.Wpct );
                    lastWpct = team.Wpct;
                }
            }
        }

        //broken
        [TestMethod()]
        public void getSortedTeamsDescendingWPctByLeague()
        {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
      //      scope.League = "X";
            scope.Division = "National";
            scope.OrderAscending = false;

            List<Team> teams = leagueStandingsReport.getTeamsByWinPercentage(scope);
            Assert.AreEqual(6, teams.Count);

            double lastWpct = -1;
            foreach (Team team in teams)
            {
                if (lastWpct == -1)
                    lastWpct = team.Wpct;
                else
                {
                    Assert.IsTrue(team.Wpct <= lastWpct);
                    Debug.WriteLine(team.Wpct);
                    lastWpct = team.Wpct;
                }
            }
        }
    }
}