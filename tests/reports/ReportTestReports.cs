using Microsoft.VisualStudio.TestTools.UnitTesting;
using somReporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            report = new LeagueStandingsReport("test");
            Assert.AreEqual(report.getReportType(), "LEAGUE STANDINGS");

            report = new LeagueGrandTotalsReport("test");
            Assert.AreEqual(report.getReportType(), "LEAGUE GRAND TOTALS");
        }


    }
}