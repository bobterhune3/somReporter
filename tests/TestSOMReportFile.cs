using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using somReporter;
using System.Collections.Generic;
using System.Diagnostics;

namespace somReporter
{

    [TestClass()]
    public class TestSOMReportFile
    {
        private SOMReportFile file;

        [TestInitialize()]
        public void Initialize()
        {
    //        Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            file = new SOMReportFile("ALL_REPORTS.PRT");
        }

        [TestCleanup()]
        public void Cleanup() { }


        [TestMethod]
        public void TestLoadFail()
        {
            bool testFailed = false;
            try
            {
                new SOMReportFile("BAD_FILE_NAME.PRT");
            }
            catch (Exception ex)
            {
                testFailed = true;
            }
            Assert.IsTrue(testFailed, "Expected Report to fail to load");
        }

        [TestMethod]
        public void TestLoadFile()
        {
            Assert.IsNotNull(file, "SOM Report File is null");

        }


        [TestMethod()]
        public void readReportLinesTestLineReader()
        {
            file.parseFile();
            List<Report> reports = file.Reports;
            Assert.AreEqual(reports.Count, 11);

            foreach( Report report in reports )
            {
                Debug.WriteLine(report.Name);
            }


        }

        [TestMethod()]
        public void cleanUpLineTestCleanupLine()
        {
            Assert.AreEqual(file.cleanUpLine("AA AAA"), "AA AAA");
            Assert.AreEqual(file.cleanUpLine("AA [A]AA"), "AA [A]AA");
            Assert.AreEqual(file.cleanUpLine("AA [0]AA"), "AA AA");
            Assert.AreEqual(file.cleanUpLine("AA [1]AA"), "AA AA");
            Assert.AreEqual(file.cleanUpLine("AA [2]AA"), "AA AA");
            Assert.AreEqual(file.cleanUpLine("AA [3]AA"), "AA AA");
            Assert.AreEqual(file.cleanUpLine("AA [4]AA"), "AA AA");
            Assert.AreEqual(file.cleanUpLine("AA [5]AA"), "AA [5]AA");
            Assert.AreEqual(file.cleanUpLine("[0]AA AA"), "AA AA");
            //Assert.AreEqual(file.cleanUpLine("AA AA[0]"), "AA AA");
        }

        [TestMethod()]
        public void testFirstLineOfReportSuccess()
        {
            file.parseFirstLineOfReport("LEAGUE STANDINGS FOR 2015 No Dice");
            Assert.AreEqual(file.Season, "2015");
            Assert.AreEqual(file.LeagueName, "No Dice");
            Assert.AreEqual(file.SeasonTitle, "2015 No Dice");
            file.parseFirstLineOfReport("LEAGUE STANDINGS FOR 2016 No Dice");
            Assert.AreEqual(file.Season, "2016");
            Assert.AreEqual(file.LeagueName, "No Dice");
            Assert.AreEqual(file.SeasonTitle, "2016 No Dice");
            file.parseFirstLineOfReport("LEAGUE STANDINGS FOR 999 No Dice");
            Assert.AreEqual(file.Season, "999");
            Assert.AreEqual(file.LeagueName, "No Dice");
            Assert.AreEqual(file.SeasonTitle, "999 No Dice");
            file.parseFirstLineOfReport("LEAGUE STANDINGS FOR 2015 TEST LEAGUE NAME");
            Assert.AreEqual(file.Season, "2015");
            Assert.AreEqual(file.LeagueName, "TEST LEAGUE NAME");
            Assert.AreEqual(file.SeasonTitle, "2015 TEST LEAGUE NAME");

        }

        [TestMethod()]
        public void testFirstLineOfReportFailure()
        {
            bool testFailed = false;
            try
            {
                file.parseFirstLineOfReport("THIS IS NOT A VALID FIRST LINE");
            }
            catch (Exception ex)
            {
                testFailed = true;
            }
            Assert.IsTrue(testFailed, "Expected Line is not a valid");
        }
    }
}

 

