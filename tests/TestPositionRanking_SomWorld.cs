
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using LineupEngine;

namespace somReporter
{
    [TestClass()]
    public class TestPositionRanking_SomWorld
    {
        private IRankDefScorer scorer = null;
        [TestInitialize()]
        public void Initialize()
        {
            scorer = RankDepthFactory.createDepthFactory(RankDepthFactory.DEPTH_ALGO.SOMWORD);
        }

        [TestCleanup()]
        public void Cleanup() { }

        [TestMethod]
        public void testFirstBase()
        {
            List<TestItem> testCases = new List<TestItem>();
            testCases.Add(TestItem.create(1, 4, 4.0));
            testCases.Add(TestItem.create(1, 25, 25.0));
            testCases.Add(TestItem.create(2, 7, 12.6));
            testCases.Add(TestItem.create(2, 9, 14.6));
            testCases.Add(TestItem.create(2, 20, 25.6));
            testCases.Add(TestItem.create(3, 14, 25.2));
            testCases.Add(TestItem.create(3, 10, 21.2));
            testCases.Add(TestItem.create(4, 25, 41.8));
            testCases.Add(TestItem.create(4, 30, 46.8));
            testCases.Add(TestItem.create(4, 5, 21.8));
            testCases.Add(TestItem.create(4, 12, 28.8));

            foreach (TestItem item in testCases)
            {
                double result = scorer.calculateFirstBaseDefScore(item.Range, item.EFact);
                Assert.AreEqual(item.Expect, result, item.ToString());
            }
        }

        [TestMethod]
        public void testSecondBase()
        {
            List<TestItem> testCases = new List<TestItem>();
            testCases.Add(TestItem.create(1, 4,  4));
            testCases.Add(TestItem.create(1, 28, 28));
            testCases.Add(TestItem.create(2, 8, 24.8));
            testCases.Add(TestItem.create(2, 15, 31.8));
            testCases.Add(TestItem.create(2, 30, 46.8));
            testCases.Add(TestItem.create(3, 30, 63.6));
            testCases.Add(TestItem.create(3, 10, 43.6));
            testCases.Add(TestItem.create(3, 20, 53.6));
            testCases.Add(TestItem.create(4, 15, 65.4));
            testCases.Add(TestItem.create(4, 39, 89.4));
            testCases.Add(TestItem.create(5, 20, 87.2));

            foreach (TestItem item in testCases)
            {
                double result = scorer.calculateSecondBaseDefScore(item.Range, item.EFact);
                Assert.AreEqual(item.Expect, result, item.ToString());
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
                Assert.AreEqual(item.Expect, result, item.ToString());
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
                Assert.AreEqual(item.Expect, result, item.ToString());
            }
        }

        [TestMethod]
        public void testLeftField()
        {
            List<TestItem> testCases = new List<TestItem>();
            testCases.Add(TestItem.create(1, 10, 10));
            testCases.Add(TestItem.create(2, 10, 18.4));
            testCases.Add(TestItem.create(3, 10, 26.8));
            testCases.Add(TestItem.create(4, 10, 40.8));
            testCases.Add(TestItem.create(1, 20, 20));
            testCases.Add(TestItem.create(2, 20, 28.4));
            testCases.Add(TestItem.create(3, 20, 36.8));
            testCases.Add(TestItem.create(4, 20, 50.8));
            testCases.Add(TestItem.create(5, 20, 62));

            foreach (TestItem item in testCases)
            {
                double result = scorer.calculateLeftFieldDefScore(item.Range, item.EFact, 0);
                Assert.AreEqual(item.Expect, result, item.ToString());
            }
        }

        [TestMethod]
        public void testCenterField()
        {
            List<TestItem> testCases = new List<TestItem>();
            testCases.Add(TestItem.create(1, 10, 10));
            testCases.Add(TestItem.create(2, 10, 22.6));
            testCases.Add(TestItem.create(3, 10, 35.2));
            testCases.Add(TestItem.create(4, 10, 56.2));
            testCases.Add(TestItem.create(1, 20, 20));
            testCases.Add(TestItem.create(2, 20, 32.6));
            testCases.Add(TestItem.create(3, 20, 45.2));
            testCases.Add(TestItem.create(4, 20, 66.2));
            testCases.Add(TestItem.create(5, 20, 83));

            foreach (TestItem item in testCases)
            {
                double result = scorer.calculateCenterFieldDefScore(item.Range, item.EFact, 0);
                Assert.AreEqual(item.Expect, result, item.ToString());
            }
        }

        [TestMethod]
        public void testRightField()
        {
            List<TestItem> testCases = new List<TestItem>();
            testCases.Add(TestItem.create(1, 10, 10));
            testCases.Add(TestItem.create(2, 10, 18.4));
            testCases.Add(TestItem.create(3, 10, 26.8));
            testCases.Add(TestItem.create(4, 10, 40.8));
            testCases.Add(TestItem.create(1, 20, 20));
            testCases.Add(TestItem.create(2, 20, 28.4));
            testCases.Add(TestItem.create(3, 20, 36.8));
            testCases.Add(TestItem.create(4, 20, 50.8));
            testCases.Add(TestItem.create(5, 20, 62));

            foreach (TestItem item in testCases)
            {
                double result = scorer.calculateRightFieldDefScore(item.Range, item.EFact, 0);
                Assert.AreEqual(item.Expect, result, item.ToString());
            }
        }

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
                double result = scorer.calculateCatcherDefScore(item.Range, item.EFact, 0);
                Assert.AreEqual(item.Expect, result, item.ToString());
            }
        }
    }

    class TestItem
    {
        public static TestItem create(int range, int efact, double expected)
        {
            return new TestItem(range, efact, 0, expected);
        }

        public static TestItem create(int range, int efact, int throwingArm, double expected)
        {
            return new TestItem(range, efact, throwingArm, expected);
        }


        private TestItem(int range, int efact, int throwingArm, double expected)
        {
            Range = range;
            EFact = efact;
            Expect = expected;
            ThrowingArm = throwingArm;
        }

        public int Range  { get; set; }
        public int EFact  { get; set; }
        public int ThrowingArm { get; set; }
        public double Expect { get; set; }

        public override string ToString()
        {
            return Range + " e" + EFact + "=" + Expect;
        }
    }

}
