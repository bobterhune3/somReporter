using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        }



        private void fillWithTestTeams(Dictionary<String, TeamLineup> lineups, int count )
        {
            for( int i=1; i<=count; i++ )
            {
                String teamName = "MyTeam" + i;
                TeamLineup lineup = new TeamLineup();

                LineupData lupLeft = new LineupData("L", new LineupBalanceItem(0, 9, "L"), new LineupBalanceItem(18, 9, "R"), 77, Guid.NewGuid());
                lineup.Lineups.Add(lupLeft);

                LineupData lupRight = new LineupData("R", new LineupBalanceItem(0, 9, "L"), new LineupBalanceItem(18, 9, "R"), 888, Guid.NewGuid());
                lineup.Lineups.Add(lupRight);

                lineups.Add(teamName, lineup);
            }
        }
    }
}
