using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using somReporter.team;

namespace LIneupUsageEstimator.storage
{
    [TestClass()]
    public class TestBasicPersistence
    {
        private const String TEST_TEAM_NAME = "MyTeam";

        [TestInitialize()]
        public void Initialize()
        {
            LineupPersistence.clearDatabase();
        }

        [TestCleanup()]
        public void Cleanup()
        {
           LineupPersistence.clearDatabase();
        }

        [TestMethod]
        public void testStoreAndLoadPersistenceObject()
        {
            int TEST_COUNT = 5;

            Dictionary<String,TeamLineup> teamLineups = LineupPersistence.loadDatabase();
            Assert.IsTrue(teamLineups.Count == 0);
            fillWithTestTeams(teamLineups, TEST_COUNT);
            LineupPersistence.saveDatabase(teamLineups);

            Dictionary<String,TeamLineup> lineup2 = LineupPersistence.loadDatabase();
            Assert.IsTrue(lineup2.Count() == TEST_COUNT);
            Assert.IsTrue(lineup2.ContainsKey(TEST_TEAM_NAME+"1"));
            TeamLineup lineup = teamLineups[TEST_TEAM_NAME + "1"];
            Assert.IsTrue(lineup.Lineups.Count == 2);

            Assert.AreEqual(18, lineup2[TEST_TEAM_NAME + "1"].playerByGRID.Count);
            Assert.AreEqual(18, lineup2[TEST_TEAM_NAME + "2"].playerByGRID.Count);
        }



        private void fillWithTestTeams(Dictionary<String, TeamLineup> lineups, int count )
        {
            for( int i=1; i<=count; i++ )
            {
                String teamName = "MyTeam" + i;
                TeamLineup lineup = new TeamLineup();

                LineupData lupLeft = new LineupData("L", new LineupBalanceItem(0, 9, "L"), new LineupBalanceItem(18, 9, "R"), 77, Guid.NewGuid());
                lineup.Lineups.Add(lupLeft);
                List<Player> players = new List<Player>();
                String[] names =  { "CatcherL1","FirstL1","SecondL1","ThirdL1","ShortstopL1","LeftL1","CenterL1","RightL1","DesignatedL1"};
                foreach(String name in names)
                    players.Add(createTestPlayer(name));

                LineupData lupRight = new LineupData("R", new LineupBalanceItem(0, 9, "L"), new LineupBalanceItem(18, 9, "R"), 888, Guid.NewGuid());
                lineup.Lineups.Add(lupRight);
                String[] namesR = { "CatcherR1", "FirstR1", "SecondR1", "ThirdR1", "ShortstopR1", "LeftR1", "CenterR1", "RightR1", "DesignatedR1" };
                foreach (String name in namesR)
                    players.Add(createTestPlayer(name));

                lineup.playerByGRID = players;

                lineups.Add(teamName, lineup);
            }
        }

        private Player createTestPlayer(String name)
        {
            Player player = new Player();
            player.Name = name;
            player.Throws = "S";
            return player;
        }
    }
}
