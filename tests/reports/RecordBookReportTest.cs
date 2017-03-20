using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using somReporter.util;

namespace somReporter
{
    [TestClass]
    public class RecordBookReportTest
    {
        [TestMethod]
        public void simpleTest()
        {
            Report.DATABASE.reset();
            SOMReportFile file = new SOMReportFile(Config.getConfigurationFile("ALL_REPORTS.PRT"));
            file.parseLeagueFile();
            RecordBookReport recordBookReport = (RecordBookReport)file.FindReport("RECORD BOOK FOR FOR");
            recordBookReport.processReport();
        }
    }
}
