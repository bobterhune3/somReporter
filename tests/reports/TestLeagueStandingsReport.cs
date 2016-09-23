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

        [TestInitialize()]
        public void Initialize()
        {
            Report.DATABASE.reset();
            file = new SOMReportFile("ALL_REPORTS.PRT");
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
            file.parseFile();
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
            file.parseFile();
            Report report = file.FindReport("LEAGUE STANDINGS FOR");
            report.processReport();

            foreach (Team team in Report.DATABASE.Teams())
            {
                Debug.WriteLine(team.Name);
            }
            Assert.AreEqual(28, Report.DATABASE.Teams().Count);
        }

        [TestMethod()]
        public void lookupTeamByName()
        {
            file.parseFile();
            LeagueStandingsReport report = (LeagueStandingsReport)file.FindReport("LEAGUE STANDINGS FOR");
            report.processReport();

            Team team = report.getTeamDataByName("New York");
            Assert.AreEqual("New York", team.Name);
            Assert.AreEqual("NYB", team.Abrv);
            Assert.AreEqual(82, team.Wins);
            Assert.AreEqual(78, team.Loses);
            Assert.AreEqual(25.0, team.Gb);
            Assert.AreEqual("AL East", team.Full_div);
            Assert.AreEqual("AL", team.League);
            Assert.AreEqual("East", team.Division);

            Assert.IsNull(report.getTeamDataByName("BAD NAME"));
        }

        [TestMethod()]
        public void lookupTeamByCode()
        {
            file.parseFile();
            LeagueStandingsReport report = (LeagueStandingsReport)file.FindReport("LEAGUE STANDINGS FOR");
            report.processReport();

            Team team = report.getTeamDataByCode("NYB");
            Assert.AreEqual("New York", team.Name);
        }

        [TestMethod()]
        public void getTeamsByDivision()
        {
            file.parseFile();
            LeagueStandingsReport report = (LeagueStandingsReport)file.FindReport("LEAGUE STANDINGS FOR");
            report.processReport();

            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.League = "AL";
            scope.Division = "East";
            
            List<Team> teams = report.getTeamsByWinPercentage(scope);
            Assert.AreEqual(7, teams.Count);
        }

        [TestMethod()]
        public void getTeamsByLeague()
        {
            file.parseFile();
            LeagueStandingsReport report = (LeagueStandingsReport)file.FindReport("LEAGUE STANDINGS FOR");
            report.processReport();

            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.League = "AL";

            List<Team> teams = report.getTeamsByWinPercentage(scope);
            Assert.AreEqual(14, teams.Count);
        }

        [TestMethod()]
        public void getTeamsByInvalidLeague()
        {
            file.parseFile();
            LeagueStandingsReport report = (LeagueStandingsReport)file.FindReport("LEAGUE STANDINGS FOR");
            report.processReport();

            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.League = "XX";

            List<Team> teams = report.getTeamsByWinPercentage(scope);
            Assert.AreEqual(0, teams.Count);
        }

        [TestMethod()]
        public void getSortedTeamsAscendingWPctByLeague()
        {
            file.parseFile();
            LeagueStandingsReport report = (LeagueStandingsReport)file.FindReport("LEAGUE STANDINGS FOR");
            report.processReport();

            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.League = "AL";
            scope.Division = "East";
            scope.OrderAscending = true;

            List<Team> teams = report.getTeamsByWinPercentage(scope);
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
            file.parseFile();
            LeagueStandingsReport report = (LeagueStandingsReport)file.FindReport("LEAGUE STANDINGS FOR");
            report.processReport();

            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.League = "AL";
            scope.Division = "East";
            scope.OrderAscending = false;

            List<Team> teams = report.getTeamsByWinPercentage(scope);
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
            file.parseFile();
            LeagueStandingsReport report = (LeagueStandingsReport)file.FindReport("LEAGUE STANDINGS FOR");
            report.processReport();

            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.AllTeams = true;
            scope.OrderAscending = true;

            List<Team> teams = report.getTeamsByName(scope);
            Assert.AreEqual(28, teams.Count);

            Assert.AreEqual("ANAHEIM", teams[0].Name.ToUpper());
            Assert.AreEqual("ARIZONA", teams[1].Name.ToUpper());
            Assert.AreEqual("ATLANTA", teams[2].Name.ToUpper());
            Assert.AreEqual("BALTIMORE", teams[3].Name.ToUpper());
        }

        [TestMethod()]
        public void getAllTeamsByOwner()
        {
            file.parseFile();
            LeagueStandingsReport report = (LeagueStandingsReport)file.FindReport("LEAGUE STANDINGS FOR");
            report.processReport();

            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.AllTeams = true;

            Assert.AreEqual(5, report.getTeamsByOwner("B").Count);
            Assert.AreEqual(6, report.getTeamsByOwner("M").Count);
            Assert.AreEqual(5, report.getTeamsByOwner("J").Count);
            Assert.AreEqual(6, report.getTeamsByOwner("S").Count);
            Assert.AreEqual(6, report.getTeamsByOwner("G").Count);
        }

    }
}