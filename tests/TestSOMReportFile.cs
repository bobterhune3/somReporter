using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;

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
                new SOMReportFile("BAD_FILE_NAME.PRT");
            }
            catch (Exception ex)
            {
                testFailed = true;
            }
            Assert.IsTrue(testFailed, "Expected Report to fail to load");
        }

 
    }
}

 

