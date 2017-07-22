using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using somReporter.util;

namespace somReporter
{

    [TestClass()]
    public class TestSOMReportFile
    {
        [TestMethod]
        public void loadTheFile()
        {
            bool testFailed = false;
            try
            {
                new SOMReportFile(Config.getConfigurationFile("BAD_FILE_NAME.PRT"));
            }
            catch (Exception)
            {
                testFailed = true;
            }
            Assert.IsTrue(testFailed, "Expected Report to fail to load");
        }

        [TestMethod()]
        public void testGetSignificantDigits()
        {
            Assert.AreEqual( .123, Report.RoundToSignificantDigits(.12345678, 3));
            Assert.AreEqual( .1235, Report.RoundToSignificantDigits(.12345678, 4));
            Assert.AreEqual(.1, Report.RoundToSignificantDigits(.12345678, 1));
            Assert.AreEqual(0, Report.RoundToSignificantDigits(.12345678, 0));
            Assert.AreEqual(.12345678, Report.RoundToSignificantDigits(.12345678, 13));
            Assert.AreEqual(.17, Report.RoundToSignificantDigits(.1677, 2));
            Assert.AreEqual(.168, Report.RoundToSignificantDigits(.1677, 3));
            Assert.AreEqual(4, Report.RoundToSignificantDigits(4.1333333, 1));
            Assert.AreEqual(4300, Report.RoundToSignificantDigits(4321.1333333, 2));
        }

    }
}
