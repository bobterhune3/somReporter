using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace somReporter
{
    [TestClass]
    public class RecordBookReportTest
    {
        [TestMethod]
        public void simpleTest()
        {
            Report.DATABASE.reset();
            SOMReportFile file = new SOMReportFile("ALL_REPORTS.PRT");
            file.parseFile();
            RecordBookReport recordBookReport = (RecordBookReport)file.FindReport("RECORD BOOK FOR FOR");
            recordBookReport.processReport();
        }
    }
}
