using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using somReporter.team;
using somReporter.util.somReporter;
using LineupEngine;

namespace somReporter
{
    [TestClass()]
    public class TestTeamReportFileParser
    {
        private SOMTeamReportFile teamReportFile;

        [TestInitialize()]
        public void Initialize()
        {
            teamReportFile = new SOMTeamReportFile(util.somReporter.Config.getConfigurationFile("rosterReport.PRT"));
        }

        [TestCleanup()]
        public void Cleanup() { }

        [TestMethod]
        public void teamReport_verifyTheFileLoaded()
        {
            Assert.IsNotNull(teamReportFile, "SOM Team Roster Report File is null");
        }


        [TestMethod()]
        public void teamReport_cleanUpLineTestCleanupLine(SOMTeamReportFile file)
        {
            internal_cleanUpLineTestCleanupLine(teamReportFile);
        }

        private void internal_cleanUpLineTestCleanupLine(SOMTeamReportFile file)
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
        public void teamReport_testFirstLineOfLeagueReportSuccess()
        {
            String TeamName = teamReportFile.isTeamNameLineOfReport("2017 Arizona D-Backs   [2]Black=In Majors(14/11) [4]Blue=In Minors(4/7)");
            Assert.AreEqual(TeamName, "Arizona D-Backs");
        }

        [TestMethod()]
        public void teamReport_testFirstLeagueLineOfReportFailure()
        {
            bool testFailed = false;
            try
            {
                String TeamName = teamReportFile.isTeamNameLineOfReport("THIS IS NOT A VALID FIRST LINE");
                Assert.IsNull(TeamName);
            }
            catch (Exception)
            {
                testFailed = true;
            }
            Assert.IsTrue(testFailed, "Expected Line is not a valid");
        }

        [TestMethod()]
        public void teamReport_testParseTheFile()
        {
            teamReportFile.parse();
            Dictionary<String, List<Player>> balanceData = teamReportFile.getBalanceData();

            String[] types = { "9L", "8L", "7L", "6L", "5L", "4L", "3L", "2L", "1L", "E", "1R", "2R", "3R", "4R", "5R", "6R", "7R", "8R", "9R" };
            Assert.AreEqual( balanceData.Keys.Count, types.Length);
            foreach( String type in types)
            {
                Assert.IsTrue(balanceData[type].Count > 0);
                foreach( Player player in balanceData[type])
                {
                    Assert.AreEqual(player.Bal, type);
                }
            }
        }
    }
}
