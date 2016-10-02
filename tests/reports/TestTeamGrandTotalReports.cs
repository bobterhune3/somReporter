﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace somReporter
{
    [TestClass()]
    public class TestTeamGrandTotalReports
    {
        private LeagueStandingsReport leagueStandingsReport;
        private LeagueGrandTotalsReport leaguePrimaryStatReport;

        [TestInitialize()]
        public void Initialize()
        {
            Report.DATABASE.reset();
            SOMReportFile file = new SOMReportFile("ALL_REPORTS.PRT");
            file.parseFile();
            leagueStandingsReport = (LeagueStandingsReport)file.FindReport("LEAGUE STANDINGS FOR");
            leagueStandingsReport.processReport();

            leaguePrimaryStatReport = (LeagueGrandTotalsReport)file.FindReport("LEAGUE GRAND TOTALS (primary report) FOR");
            leaguePrimaryStatReport.processReport();
        }

        [TestCleanup()]
        public void Cleanup() { }

        [TestMethod()]
        public void getReportTypetestReportType_withGrandTotal()
        {
            Report report = new Report("test");
            Assert.AreEqual(report.getReportType(), "UNKNOWN TYPE");

            report = new LeagueStandingsReport("test");
            Assert.AreEqual(report.getReportType(), "LEAGUE STANDINGS");

            report = new LeagueGrandTotalsReport("test");
            Assert.AreEqual(report.getReportType(), "LEAGUE GRAND TOTALS");
        }


        [TestMethod()]
        public void checkTeamRunsScored()
        {
            Team team1 = leagueStandingsReport.getTeamDataByName("New York");
            Team team2 = leagueStandingsReport.getTeamDataByName("Baltimore");

            Assert.AreEqual(714, team1.RunsScored);
            Assert.AreEqual(882, team2.RunsScored);
        }

        [TestMethod()]
        public void checkTeamRunsAllowed()
        {
            Team team1 = leagueStandingsReport.getTeamDataByName("New York");
            Team team2 = leagueStandingsReport.getTeamDataByName("Baltimore");

            Assert.AreEqual(675, team1.RunsAllowed);
            Assert.AreEqual(605, team2.RunsAllowed);
        }

        [TestMethod()]
        public void checkPythagoreanTheoremNY()
        {
            Team team = leagueStandingsReport.getTeamDataByName("New York");
            Assert.IsTrue(team.PythagoreanTheorem > .0);
        }


        [TestMethod()]
        public void checkPythagoreanTheoremTEX()
        {
            Team team = leagueStandingsReport.getTeamDataByName("Texas");
            Assert.IsTrue(team.PythagoreanTheorem > .0);
        }

        [TestMethod()]
        public void testPythagoreanTheorem()
        {
            Team team = new Team("AL TEST");
            team.RunsScored = 533;
            team.RunsAllowed = 788;

            Assert.AreEqual(0.314, team.PythagoreanTheorem);
        }

        [TestMethod()]
        public void testGamesBehindCalcultor()
        {
            Team leader = createTestTeam(77, 46);
            Team teamA = createTestTeam(73, 52);
            Team teamB = createTestTeam(69, 57);
            Team teamC = createTestTeam(67, 55);
            Team teamD = createTestTeam(64, 59);
            Team teamE = createTestTeam(61, 63);

            Assert.AreEqual(5.0, teamA.calculateGamesBehind(leader));
            Assert.AreEqual(9.5, teamB.calculateGamesBehind(leader));
            Assert.AreEqual(9.5, teamC.calculateGamesBehind(leader));
            Assert.AreEqual(13.0, teamD.calculateGamesBehind(leader));
            Assert.AreEqual(16.5, teamE.calculateGamesBehind(leader));
        }

        private Team createTestTeam(int w, int l)
        {
            Team Team = new Team("XXX");
            Team.Wins = w;
            Team.Loses = l;
            return Team;
        }
    }
}