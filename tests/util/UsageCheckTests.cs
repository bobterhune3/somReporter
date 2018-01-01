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
        /*
            Hitters < 120 actual at bats are allowed at 150%      (ie ab * 1.5)
            Hitters > 120 actual at bats are actual at bats + 60  (ie ab + 60)
            Hitters > 600 actual at bats are allowed at 110%      (ie ab * 1.1)

            Pitchers < 60 is 150% of actual   (59 * 1.5) = +29
            Pitchers > 118 innings is innings + 30  
            Pitchers > 199 is 115% of actual (199 * 1.15) = +30
          */

        [TestMethod()]
        public void testHittersUsage()
        {
            Assert.AreEqual(75, ComparisonReport.getPlayerTargetUsage(50));
            Assert.AreEqual(178, ComparisonReport.getPlayerTargetUsage(119));
            Assert.AreEqual(180, ComparisonReport.getPlayerTargetUsage(120));
            Assert.AreEqual(181, ComparisonReport.getPlayerTargetUsage(121));
            Assert.AreEqual(659, ComparisonReport.getPlayerTargetUsage(599));
            Assert.AreEqual(660, ComparisonReport.getPlayerTargetUsage(600));
            Assert.AreEqual(661, ComparisonReport.getPlayerTargetUsage(601));
        }

        [TestMethod()]
        public void testPitchersUsage()
        {
            Assert.AreEqual(75, ComparisonReport.getPitcherTargetUsage(50));
            Assert.AreEqual(88, ComparisonReport.getPitcherTargetUsage(59));
            Assert.AreEqual(90, ComparisonReport.getPitcherTargetUsage(60));
            Assert.AreEqual(148, ComparisonReport.getPitcherTargetUsage(118));
            Assert.AreEqual(149, ComparisonReport.getPitcherTargetUsage(119));
            Assert.AreEqual(150, ComparisonReport.getPitcherTargetUsage(120));
            Assert.AreEqual(229, ComparisonReport.getPitcherTargetUsage(199));
            Assert.AreEqual(230, ComparisonReport.getPitcherTargetUsage(200));
        }
    }
}