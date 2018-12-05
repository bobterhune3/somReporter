using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace somReporter.team
{
    [TestClass()]
    public class TestPlayerSorting
    {


        [TestInitialize()]
        public void Initialize()
        {

        }

        [TestMethod]
        public void basicPitcherSortingTest_Team1()
        {
            List<Player> pitchers= processTestData(TestTeamReportBalanceFinder.TEST_TEAM1);
            Assert.AreEqual(17, pitchers.Count);

            String[] expectedOrder = { "StartP1", "StartP3", "StartP5", "StartP4", "StartP2", "StartP8","StartP6",  };

            pitchers.Sort();

/*
            System.Console.WriteLine("AFTER SORT");
            foreach (Player p in pitchers)
                Console.WriteLine(p.Name);
*/
            for (int i=0;i<6; i++)
            {
                Assert.AreEqual(expectedOrder[i], pitchers[i].Name);
            }
        }

        [TestMethod]
        public void basicPitcherSortingTest_Team2()
        {
            List<Player> pitchers = processTestData(TestTeamReportBalanceFinder.TEST_TEAM2);
            String[] expectedOrder = { "StartP2", "StartP5", "StartP1", "StartP7", "StartP4", "StartP6",  "StartP3" };

            pitchers.Sort();
/*
            System.Console.WriteLine("AFTER SORT");
            foreach (Player p in pitchers)
                Console.WriteLine(p.Name);
                */
            for (int i = 0; i < 6; i++)
            {
                Assert.AreEqual(expectedOrder[i], pitchers[i].Name);
            }
        }
        [TestMethod]
        public void basicPitcherSortingTest_Team4()
        {
            List<Player> pitchers = processTestData(TestTeamReportBalanceFinder.TEST_TEAM4);
            String[] expectedOrder = { "StartP3", "StartP2", "StartP1", "StartP4", "StartP6", "StartP7", "StartP8" };

            pitchers.Sort();
   //         pitchers.Sort();

            System.Console.WriteLine("AFTER SORT");
            foreach (Player p in pitchers)
                Console.WriteLine(p.Name+", "+p.primaryPos);

            for (int i = 0; i < 6; i++)
            {
                Assert.AreEqual(expectedOrder[i], pitchers[i].Name);
            }
        }


        private List<Player> processTestData(String[,] testData)
        {
            List<Player> pitchers = new List<Player>();


            for (int i = 0; i < testData.GetLength(0); i++) { 
                Player player = new Player();
                player.Name = testData[i,0];
                player.Throws = testData[i,1];
                player.IsHitter = false;
                player.Bal = testData[i,3];
                player.IP = Int32.Parse(testData[i,7]);
                player.Hits = Int32.Parse(testData[i,8]);
                player.BB = Int32.Parse(testData[i, 9]);
                player.GS = Int32.Parse(testData[i,12]);
                player.SAVE = Int32.Parse(testData[i,13]);
                pitchers.Add(player);
            }
            pitchers.Add(addPositionPlayer());

            return pitchers;
        }


        private Player addPositionPlayer()
        {
            Player player = new Player();
            player.Name = "PositionPlayer";
            player.Throws ="R";
            player.IsHitter = true;
            player.IP = 475;
            player.Hits = 125;
            player.GS = 112;
            player.SAVE = 0;
            return player;
        }


    }
}
