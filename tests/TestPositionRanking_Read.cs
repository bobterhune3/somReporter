
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using LineupEngine;
using System;

namespace somReporter
{
    [TestClass()]
    public class TestPositionRanking_Read
    {
        private IRankDefScorer scorer = null;
        [TestInitialize()]
        public void Initialize()
        {
            scorer = RankDepthFactory.createDepthFactory(RankDepthFactory.DEPTH_ALGO.READ);
        }

        [TestCleanup()]
        public void Cleanup() { }

        [TestMethod]
        public void testFirstBase()
        {
            List<TestItem> testCases = new List<TestItem>();
            testCases.Add(TestItem.create(1, 4, .3));
            testCases.Add(TestItem.create(1, 25, 12.4));
            testCases.Add(TestItem.create(2, 7, 2.9));
            testCases.Add(TestItem.create(2, 9, 3.9));
            testCases.Add(TestItem.create(2, 20, 11.9));
            testCases.Add(TestItem.create(3, 14, 11.6));
            testCases.Add(TestItem.create(3, 10, 8.5));
            testCases.Add(TestItem.create(4, 25, 30.1));
            testCases.Add(TestItem.create(4, 30, 37.2));
            testCases.Add(TestItem.create(4, 5, 9.7));
            testCases.Add(TestItem.create(4, 12, 15.7));

            foreach (TestItem item in testCases)
            {
                double result = scorer.calculateFirstBaseDefScore(item.Range, item.EFact);
                Assert.AreEqual(item.Expect, Math.Round(result,1), item.ToString());
            }
        }

        [TestMethod]
        public void testSecondBase()
        {
            List<TestItem> testCases = new List<TestItem>();
            testCases.Add(TestItem.create(1, 4,  .1));
            testCases.Add(TestItem.create(1, 28, 5.4));
            testCases.Add(TestItem.create(2, 8, 3.8));
            testCases.Add(TestItem.create(2, 15, 6.5));
            testCases.Add(TestItem.create(2, 30, 13.8));
            testCases.Add(TestItem.create(3, 30, 26.1));
            testCases.Add(TestItem.create(3, 10, 13.0));
            testCases.Add(TestItem.create(3, 20, 19.1));
            testCases.Add(TestItem.create(4, 15, 30.0));
            testCases.Add(TestItem.create(4, 39, 51.1));
            testCases.Add(TestItem.create(5, 20, 52.5));

            foreach (TestItem item in testCases)
            {
                double result = scorer.calculateSecondBaseDefScore(item.Range, item.EFact);
                Assert.AreEqual(item.Expect, Math.Round(result, 1), item.ToString());
            }
        }
        [TestMethod]
        public void testThirdBase()
        {
            List<TestItem> testCases = new List<TestItem>();
            testCases.Add(TestItem.create(1, 10, 10));
            testCases.Add(TestItem.create(1, 20, 20));
            testCases.Add(TestItem.create(2, 10, 18.4));
            testCases.Add(TestItem.create(2, 20, 28.4));
            testCases.Add(TestItem.create(3, 10, 26.8));
            testCases.Add(TestItem.create(3, 20, 36.8));
            testCases.Add(TestItem.create(4, 10, 35.2));
            testCases.Add(TestItem.create(4, 20, 45.2));
            testCases.Add(TestItem.create(5, 20, 53.6));

            foreach (TestItem item in testCases)
            {
                double result = scorer.calculateThirdBaseDefScore(item.Range, item.EFact);
                Assert.AreEqual(item.Expect, Math.Round(result, 1), item.ToString());
            }
        }

        [TestMethod]
        public void testShortStop()
        {
            List<TestItem> testCases = new List<TestItem>();
            testCases.Add(TestItem.create(1, 10, 10));
            testCases.Add(TestItem.create(2, 10, 29.6));
            testCases.Add(TestItem.create(3, 10, 49.2));
            testCases.Add(TestItem.create(4, 10, 68.8));
            testCases.Add(TestItem.create(1, 20, 20));
            testCases.Add(TestItem.create(2, 20, 39.6));
            testCases.Add(TestItem.create(3, 20, 59.2));
            testCases.Add(TestItem.create(4, 20, 78.8));
            testCases.Add(TestItem.create(5, 20, 98.4));

            foreach (TestItem item in testCases)
            {
                double result = scorer.calculateShortStopDefScore(item.Range, item.EFact);
                Assert.AreEqual(item.Expect, Math.Round(result, 1), item.ToString());
            }
        }

        [TestMethod]
        public void testLeftField()
        {
            List<TestItem> testCases = new List<TestItem>();
            testCases.Add(TestItem.create(1, 10, 3.5));
            testCases.Add(TestItem.create(2, 10, 10.0));
            testCases.Add(TestItem.create(3, 10, 21.1));
            testCases.Add(TestItem.create(4, 10, 46.6));
            testCases.Add(TestItem.create(1, 20, 14.1));
            testCases.Add(TestItem.create(2, 20, 24.1));
            testCases.Add(TestItem.create(3, 20, 38.3));
            testCases.Add(TestItem.create(4, 20, 66.7));
            testCases.Add(TestItem.create(5, 20, 99.9));

            foreach (TestItem item in testCases)
            {
                double result = scorer.calculateLeftFieldDefScore(item.Range, item.EFact);
                Assert.AreEqual(item.Expect, Math.Round(result, 1), item.ToString());
            }
        }

        [TestMethod]
        public void testCenterField()
        {
            List<TestItem> testCases = new List<TestItem>();
            testCases.Add(TestItem.create(1, 10, 2.3));
            testCases.Add(TestItem.create(2, 10, 10.0));
            testCases.Add(TestItem.create(3, 10, 24.7));
            testCases.Add(TestItem.create(4, 10, 61.0));
            testCases.Add(TestItem.create(1, 20, 9.5));
            testCases.Add(TestItem.create(2, 20, 21.2));
            testCases.Add(TestItem.create(3, 20, 39.7));
            testCases.Add(TestItem.create(4, 20, 79.8));
            testCases.Add(TestItem.create(5, 20, 129.0));

            foreach (TestItem item in testCases)
            {
                double result = scorer.calculateCenterFieldDefScore(item.Range, item.EFact);
                Assert.AreEqual(item.Expect, Math.Round(result, 1), item.ToString());
            }
        }

        [TestMethod]
        public void testRightField()
        {
            List<TestItem> testCases = new List<TestItem>();
            testCases.Add(TestItem.create(1, 10, 3.5));
            testCases.Add(TestItem.create(2, 10, 10.0));
            testCases.Add(TestItem.create(3, 10, 21.1));
            testCases.Add(TestItem.create(4, 10, 46.6));
            testCases.Add(TestItem.create(1, 20, 14.1));
            testCases.Add(TestItem.create(2, 20, 24.1));
            testCases.Add(TestItem.create(3, 20, 38.3));
            testCases.Add(TestItem.create(4, 20, 66.7));
            testCases.Add(TestItem.create(5, 20, 99.9));

            foreach (TestItem item in testCases)
            {
                double result = scorer.calculateRightFieldDefScore(item.Range, item.EFact);
                Assert.AreEqual(item.Expect, Math.Round(result, 1), item.ToString());
            }
        }
        /*
        [TestMethod]
        public void testCatcher()
        {
            List<TestItem> testCases = new List<TestItem>();
            testCases.Add(TestItem.create(1, 10, 11.4));
            testCases.Add(TestItem.create(2, 10, 11.4));
            testCases.Add(TestItem.create(3, 10, 11.4));
            testCases.Add(TestItem.create(4, 10, 11.4));
            testCases.Add(TestItem.create(1, 20, 21.4));
            testCases.Add(TestItem.create(2, 20, 21.4));
            testCases.Add(TestItem.create(3, 20, 21.4));
            testCases.Add(TestItem.create(4, 20, 21.4));
            testCases.Add(TestItem.create(5, 20, 21.4));

            foreach (TestItem item in testCases)
            {
                double result = scorer.calculateCatcherDefScore(item.Range, item.EFact);
                Assert.AreEqual(item.Expect, Math.Round(result, 1), item.ToString());
            }
        }
        */
    }

}
