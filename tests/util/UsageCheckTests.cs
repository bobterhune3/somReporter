using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using somReporter.team;



namespace somReporter.util.Tests
{
    [TestClass()]
    public class UsageCheckTests
    {
        [TestMethod()]
        public void testHitters()
        {
            Assert.AreEqual(75, ComparisonReport.getPlayerTargetUsage(50));
            Assert.AreEqual(150, ComparisonReport.getPlayerTargetUsage(100));
            Assert.AreEqual(152, ComparisonReport.getPlayerTargetUsage(101));
            Assert.AreEqual(152, ComparisonReport.getPlayerTargetUsage(102));
            Assert.AreEqual(550, ComparisonReport.getPlayerTargetUsage(500));
        }

        [TestMethod()]
        public void testPitchers()
        {
            Assert.AreEqual(75, ComparisonReport.getPitcherTargetUsage(50));
            Assert.AreEqual(88, ComparisonReport.getPitcherTargetUsage(59));
            Assert.AreEqual(90, ComparisonReport.getPitcherTargetUsage(60));
            Assert.AreEqual(130, ComparisonReport.getPitcherTargetUsage(100));
            Assert.AreEqual(230, ComparisonReport.getPitcherTargetUsage(200));
        }
    }
}