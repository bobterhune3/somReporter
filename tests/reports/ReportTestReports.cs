using Microsoft.VisualStudio.TestTools.UnitTesting;
using somReporter.features;

namespace somReporter.Tests
{
    [TestClass()]
    public class ReportTestReports
    {
        [TestMethod()]
        public void getReportTypetestReportType()
        {
            Report report = new Report("test");
            Assert.AreEqual(report.getReportType(), "UNKNOWN TYPE");

            report = new LeagueStandingsReport("test", true);
            Assert.AreEqual(report.getReportType(), "LEAGUE STANDINGS");

            report = new LeagueGrandTotalsReport("test");
            Assert.AreEqual(report.getReportType(), "LEAGUE GRAND TOTALS");
        }


    }
}