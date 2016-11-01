using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace somReporter
{
    [TestClass()]
    public class TestLeagueStandingsReport
    {
        private SOMReportFile file;
        private LeagueStandingsReport leagueStandingsReport;

        [TestInitialize()]
        public void Initialize()
        {
            Report.DATABASE.reset();
            file = new SOMReportFile("ALL_REPORTS.PRT");
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

            report = new LeagueStandingsReport("test");
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
            Assert.AreEqual(28, Report.DATABASE.Teams().Count);
        }

        [TestMethod()]
        public void lookupTeamByName()
        {
            Team team = leagueStandingsReport.getTeamDataByName("New York");
            Assert.AreEqual("New York", team.Name);
            Assert.AreEqual("NYB", team.Abrv);
            Assert.AreEqual(82, team.Wins);
            Assert.AreEqual(78, team.Loses);
            Assert.AreEqual(25.0, team.Gb);
            Assert.AreEqual("AL East", team.Full_div);
            Assert.AreEqual("AL", team.League);
            Assert.AreEqual("East", team.Division);

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
            scope.League = "AL";
            scope.Division = "East";
            
            List<Team> teams = leagueStandingsReport.getTeamsByWinPercentage(scope);
            Assert.AreEqual(7, teams.Count);
        }

        [TestMethod()]
        public void getTeamsByLeague()
        {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.League = "AL";

            List<Team> teams = leagueStandingsReport.getTeamsByWinPercentage(scope);
            Assert.AreEqual(14, teams.Count);
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
            scope.League = "AL";
            scope.Division = "East";
            scope.OrderAscending = true;

            List<Team> teams = leagueStandingsReport.getTeamsByWinPercentage(scope);
            Assert.AreEqual(7, teams.Count);

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
            scope.League = "AL";
            scope.Division = "East";
            scope.OrderAscending = false;

            List<Team> teams = leagueStandingsReport.getTeamsByWinPercentage(scope);
            Assert.AreEqual(7, teams.Count);

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
            Assert.AreEqual(28, teams.Count);

            Assert.AreEqual("ANAHEIM", teams[0].Name.ToUpper());
            Assert.AreEqual("ARIZONA", teams[1].Name.ToUpper());
            Assert.AreEqual("ATLANTA", teams[2].Name.ToUpper());
            Assert.AreEqual("BALTIMORE", teams[3].Name.ToUpper());
        }

        [TestMethod()]
        public void getAllTeamsByOwner()
        {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.AllTeams = true;

            Assert.AreEqual(5, leagueStandingsReport.getTeamsByOwner("B").Count);
            Assert.AreEqual(6, leagueStandingsReport.getTeamsByOwner("M").Count);
            Assert.AreEqual(5, leagueStandingsReport.getTeamsByOwner("J").Count);
            Assert.AreEqual(6, leagueStandingsReport.getTeamsByOwner("S").Count);
            Assert.AreEqual(6, leagueStandingsReport.getTeamsByOwner("G").Count);
        }

        [TestMethod()]
        public void testGettingTeamByAbbreviation()
        {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.AllTeams = true;

            Assert.IsNotNull(leagueStandingsReport.getTeamByAbbreviation("MNB"));
            Assert.IsNotNull(leagueStandingsReport.getTeamByAbbreviation("CHJ"));
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