using Microsoft.VisualStudio.TestTools.UnitTesting;
using somReporter.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace somReporter.util.Tests
{
    [TestClass()]
    public class PairTestReports
    {
        [TestMethod()]
        public void contructionTest()
        {
            Assert.IsNotNull(new Pair("XXX"));
            Assert.IsNotNull(new Pair("XXX",1, 2));
        }

        [TestMethod()]
        public void checkInitialValues()
        {
            Pair p = new Pair("XXX");
            Assert.IsNotNull(p);
            Assert.AreEqual(0, p.Wins);
            Assert.AreEqual(0, p.Lost);
        }

        [TestMethod()]
        public void checkInitialValuesPassedInt()
        {
            Pair p = new Pair("XXX",99,55);
            Assert.IsNotNull(p);
            Assert.AreEqual(99, p.Wins);
            Assert.AreEqual(55, p.Lost);
        }

        [TestMethod()]
        public void testWinIncrement()
        {
            Pair p = new Pair("XXX", 0, 0);
            Assert.AreEqual(0, p.Wins);
            Assert.AreEqual(0, p.Lost);
            p.AddWin();
            Assert.AreEqual(1, p.Wins);
            Assert.AreEqual(0, p.Lost);
        }

        [TestMethod()]
        public void testLossIncrement()
        {
            Pair p = new Pair("XXX", 0, 0);
            Assert.AreEqual(0, p.Wins);
            Assert.AreEqual(0, p.Lost);
            p.AddLoss();
            Assert.AreEqual(0, p.Wins);
            Assert.AreEqual(1, p.Lost);
        }


        [TestMethod()]
        public void testToStringOutput()
        {
            Pair p = new Pair("XXX", 7, 22);
            Assert.AreEqual("XXX 7-22", p.ToString());
        }
    }
}