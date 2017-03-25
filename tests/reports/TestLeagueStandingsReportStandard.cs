using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using somReporter.util;

namespace somReporter
{
    [TestClass()]
    public class TestLeagueStandingsReportStandard
    {
        private SOMReportFile file;
        private LeagueStandingsReport leagueStandingsReport;

        [TestInitialize()]
        public void Initialize()
        {
            Config.PRT_FILE_LOCATION = "testData";
            Config.LEAGUE_NAME = "";
            Report.DATABASE.reset();
         //   string [] leagues = { "AL", "NL" };
         //   Program.LEAGUES = leagues;
            file = new SOMReportFile(Config.getConfigurationFile("ALL_REPORTS.PRT"));
            file.parseLeagueFile();
            leagueStandingsReport = (LeagueStandingsReport)file.FindReport("LEAGUE STANDINGS FOR");
            leagueStandingsReport.processReport();
        }

        [TestCleanup()]
        public void Cleanup() { }

        [TestMethod()]
        public void getReportTypetestReportType()
        {
            Report report = new Report("test");
            Assert.AreEqual(report.getReportType(), "UNKNOWN TYPE");

            report = new LeagueStandingsReport("test", true);
            Assert.AreEqual(report.getReportType(), "LEAGUE STANDINGS");
        }



        [TestMethod()]
        public void readReportLinesTestLineReader()
        {
            List<Report> reports = file.Reports;
            Assert.AreEqual(reports.Count, 12);

            foreach (Report report in reports)
            {
                Debug.WriteLine(report.Name);
            }

            Assert.IsNotNull(file.FindReport("LEAGUE STANDINGS FOR"));
            Assert.IsNull(file.FindReport("XXX"));
        }

        [TestMethod()]
        public void getListOfTeams()
        {
            foreach (Team team in Report.DATABASE.Teams())
            {
                Debug.WriteLine(team.Name);
            }
            Assert.AreEqual(18, Report.DATABASE.Teams().Count);
        }

        [TestMethod()]
        public void lookupTeamByName()
        {
            Team team = leagueStandingsReport.getTeamDataByName("New York");
            Assert.AreEqual("New York", team.Name);
            Assert.AreEqual("NYB", team.Abrv);
            Assert.AreEqual(92, team.Wins);
            Assert.AreEqual(70, team.Loses);
            Assert.AreEqual(1.0, team.Gb);
        //    Assert.AreEqual("AL East", team.Full_div);
        //    Assert.AreEqual("AL", team.League);
            Assert.AreEqual("Federal", team.Division);

            Assert.IsNull(leagueStandingsReport.getTeamDataByName("BAD NAME"));
        }

        [TestMethod()]
        public void lookupTeamByCode()
        {
            Team team = leagueStandingsReport.getTeamByAbbreviation("NYB");
            Assert.AreEqual("New York", team.Name);
        }

        [TestMethod()]
        public void getTeamsByDivision()
        {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
        //    scope.League = "X";
            scope.Division = "Federal";
            
            List<Team> teams = leagueStandingsReport.getTeamsByWinPercentage(scope);
            Assert.AreEqual(6, teams.Count);
        }

        [TestMethod()]
        public void getTeamsByLeague()
        {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.League = "";

            List<Team> teams = leagueStandingsReport.getTeamsByWinPercentage(scope);
            Assert.AreEqual(18, teams.Count);
        }

        [TestMethod()]
        public void getTeamsByInvalidLeague()
        {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.League = "XX";

            List<Team> teams = leagueStandingsReport.getTeamsByWinPercentage(scope);
            Assert.AreEqual(0, teams.Count);
        }

        [TestMethod()]
        public void getSortedTeamsAscendingWPctByLeague()
        {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
        //    scope.League = "X";
            scope.Division = "Federal";
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

        [TestMethod()]
        public void getSortedTeamsDescendingWPctByLeague()
        {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
      //      scope.League = "X";
            scope.Division = "Federal";
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

        [TestMethod()]
        public void getAllSortedTeamsByName()
        {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.AllTeams = true;
            scope.OrderAscending = true;

            List<Team> teams = leagueStandingsReport.getTeamsByName(scope);
            Assert.AreEqual(18, teams.Count);

            Assert.AreEqual("ARIZONA", teams[0].Name.ToUpper());
            Assert.AreEqual("CHICAGO", teams[1].Name.ToUpper());
            Assert.AreEqual("CLEVELAND", teams[2].Name.ToUpper());
            Assert.AreEqual("DETROIT", teams[3].Name.ToUpper());
        }

        [TestMethod()]
        public void getAllTeamsByOwner()
        {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.AllTeams = true;

            Assert.AreEqual(6, leagueStandingsReport.getTeamsByOwner("B").Count);
            Assert.AreEqual(6, leagueStandingsReport.getTeamsByOwner("M").Count);
      //      Assert.AreEqual(5, leagueStandingsReport.getTeamsByOwner("J").Count);
      //      Assert.AreEqual(6, leagueStandingsReport.getTeamsByOwner("S").Count);
            Assert.AreEqual(6, leagueStandingsReport.getTeamsByOwner("G").Count);
        }

        [TestMethod()]
        public void testGettingTeamByAbbreviation()
        {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.AllTeams = true;

            Assert.IsNotNull(leagueStandingsReport.getTeamByAbbreviation("PTB"));
            Assert.IsNotNull(leagueStandingsReport.getTeamByAbbreviation("CHB"));
            Assert.IsNull(leagueStandingsReport.getTeamByAbbreviation("XXX"));
        }

        [TestMethod()]
        public void testPropertyStringUtility()
        {
            Dictionary<string, string> d1 = leagueStandingsReport.loadStorageString("");
            Assert.IsTrue(d1.Count == 1);
            d1 = leagueStandingsReport.loadStorageString("X=1");
            Assert.IsTrue(d1["X"] == "1");
            d1 = leagueStandingsReport.loadStorageString("X=1|");
            Assert.IsTrue(d1["X"] == "1");
            d1 = leagueStandingsReport.loadStorageString("X=1|Y=2");
            Assert.IsTrue(d1["X"] == "1" && d1["Y"] == "2");
            d1 = leagueStandingsReport.loadStorageString("X=1|Y=2|");
            Assert.IsTrue(d1["X"] == "1" && d1["Y"] == "2");
        }

    }
}