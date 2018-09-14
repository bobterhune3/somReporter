using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using somReporter.util.somReporter;

namespace somReporter
{
    [TestClass()]
    public class TestFileParser
    {
        private SOMReportFile leagueReportFile;
        private SOMReportFile teamReportFile;

        [TestInitialize()]
        public void Initialize()
        {
            leagueReportFile = new SOMReportFile(Config.getConfigurationFile("ALL_REPORTS.PRT"));
            teamReportFile = new SOMReportFile(Config.getConfigurationFile("TEAM_ALL_REPORTS.PRT"));
        }

        [TestCleanup()]
        public void Cleanup() { }

        [TestMethod]
        public void verifyTheFileLoaded()
        {
            Assert.IsNotNull(leagueReportFile, "SOM League Report File is null");
            Assert.IsNotNull(teamReportFile, "SOM Team Report File is null");
        }


        [TestMethod()]
        public void cleanUpLineTestCleanupLine(SOMReportFile file)
        {
            internal_cleanUpLineTestCleanupLine(leagueReportFile);
            internal_cleanUpLineTestCleanupLine(teamReportFile);
        }

        private void internal_cleanUpLineTestCleanupLine(SOMReportFile file)
        {
            Assert.AreEqual(file.cleanUpLine("AA AAA"), "AA AAA");
            Assert.AreEqual(file.cleanUpLine("AA [A]AA"), "AA [A]AA");
            Assert.AreEqual(file.cleanUpLine("AA [0]AA"), "AA [0]AA");
            Assert.AreEqual(file.cleanUpLine("AA [1]AA"), "AA AA");
            Assert.AreEqual(file.cleanUpLine("AA [2]AA"), "AA AA");
            Assert.AreEqual(file.cleanUpLine("AA [3]AA"), "AA AA");
            Assert.AreEqual(file.cleanUpLine("AA [4]AA"), "AA [4]AA");
            Assert.AreEqual(file.cleanUpLine("AA [5]AA"), "AA [5]AA");
            Assert.AreEqual(file.cleanUpLine("[0]AA AA"), "[0]AA AA");
            //Assert.AreEqual(file.cleanUpLine("AA AA[0]"), "AA AA");
        }

        [TestMethod()]
        public void testFirstLineOfLeagueReportSuccess()
        {
            leagueReportFile.parseLeagueFirstLineOfReport("LEAGUE STANDINGS FOR 2015 No Dice");
            Assert.AreEqual(leagueReportFile.Season, "2015");
            Assert.AreEqual(leagueReportFile.LeagueName, "No Dice");
            Assert.AreEqual(leagueReportFile.SeasonTitle, "2015 No Dice");
            leagueReportFile.parseLeagueFirstLineOfReport("LEAGUE STANDINGS FOR 2016 No Dice");
            Assert.AreEqual(leagueReportFile.Season, "2016");
            Assert.AreEqual(leagueReportFile.LeagueName, "No Dice");
            Assert.AreEqual(leagueReportFile.SeasonTitle, "2016 No Dice");
            leagueReportFile.parseLeagueFirstLineOfReport("LEAGUE STANDINGS FOR 999 No Dice");
            Assert.AreEqual(leagueReportFile.Season, "999");
            Assert.AreEqual(leagueReportFile.LeagueName, "No Dice");
            Assert.AreEqual(leagueReportFile.SeasonTitle, "999 No Dice");
            leagueReportFile.parseLeagueFirstLineOfReport("LEAGUE STANDINGS FOR 2015 TEST LEAGUE NAME");
            Assert.AreEqual(leagueReportFile.Season, "2015");
            Assert.AreEqual(leagueReportFile.LeagueName, "TEST LEAGUE NAME");
            Assert.AreEqual(leagueReportFile.SeasonTitle, "2015 TEST LEAGUE NAME");
        }

        [TestMethod()]
        public void testFirstLineOfTeamReportSuccess()
        {
            teamReportFile.parseTeamFirstLineOfReport("Team Statistics For 2015 Oakland Athletics Totals After 162 Games");
            Assert.AreEqual(teamReportFile.Season, "2015");
            Assert.AreEqual(teamReportFile.TeamName, "Oakland Athl");
        }

        [TestMethod()]
        public void testFirstLeagueLineOfReportFailure()
        {
            bool testFailed = false;
            try
            {
                leagueReportFile.parseLeagueFirstLineOfReport("THIS IS NOT A VALID FIRST LINE");
            }
            catch (Exception)
            {
                testFailed = true;
            }
            Assert.IsTrue(testFailed, "Expected Line is not a valid");
        }

        [TestMethod()]
        public void testFirstTeamLineOfReportFailure()
        {
            bool testFailed = false;
            try
            {
                teamReportFile.parseLeagueFirstLineOfReport("THIS IS NOT A VALID FIRST LINE");
            }
            catch (Exception)
            {
                testFailed = true;
            }
            Assert.IsTrue(testFailed, "Expected Line is not a valid");
        }
    }
}
