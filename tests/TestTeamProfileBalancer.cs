using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using LineupEngine;

namespace somReporter.team
{
    [TestClass]
    public class TestTeamProfilePBalancer
    {
        private List<LineupBalanceItem> balanceItems;

        [TestInitialize()]
        public void Initialize()
        {
            balanceItems = LineupTools.buildDefaultLineupTypes();
            RecordIndex.resetIndex(RecordIndex.INDEX.TestTeamId);
        }

        [TestCleanup()]
        public void Cleanup() { }
       
        [TestMethod]
        public void testProfiler_normal_lineup()
        {
            List<Player> pitchers = buildListOfTestPitchers(TEST_TEAM_NORMAL);
            TeamPitchingProfile profile = TeamPitchingProfile.generatedLikelyUsage(pitchers);

            Assert.AreEqual(8, profile.Starters().Count, "Count Starting Pitcher");
            Assert.AreEqual(6, profile.Relievers().Count, "Count Starting Pitcher");
            Assert.AreEqual(1, countClosers(profile), "Count Starting Pitcher");
        }

        [TestMethod]
        public void testProfiler_full_test_lineup()
        {
            List<Player> pitchers = buildListOfTestPitchers(TEST_FULL_TEST_SQUAD);
            TeamPitchingProfile profile = TeamPitchingProfile.generatedLikelyUsage(pitchers);

            Assert.AreEqual(7, profile.Starters().Count, "Count Starting Pitcher");
            int closerCount = countClosers(profile);
     //       Assert.AreEqual(pitchers.Count- profile.Starters().Count- closerCount, 
     //           profile.Relievers().Count, "Count Relief Pitchers");
            Assert.AreEqual(1, closerCount, "Count Closers");
        }

        //
        // Utility Test Methods
        //
        private int countClosers(TeamPitchingProfile profile)
        {
            if (profile.Closer() == null) return 0;
            return 1;
        }

        private List<Player> buildListOfTestPitchers(String[,] testData)
        {
            List<Player> players = new List<Player>();

            for (int i = 0; i < testData.GetLength(0); i++)
            {
                Player player = new Player();
                player.Name = testData[i, 0];
                player.IP = Int32.Parse(testData[i, 3]);
                player.Bal = testData[i, 2];
                player.Hits = Int32.Parse(testData[i, 4]);
                player.BB = Int32.Parse(testData[i, 5]);
                player.GS = Int32.Parse(testData[i, 6]);
                player.SAVE = Int32.Parse(testData[i, 7]);
                player.Team = new Team(RecordIndex.getNextId(RecordIndex.INDEX.TestTeamId),"ABV", 3);
                player.IsHitter = false;
                player.Throws = testData[i, 1];
                players.Add(player);
            }
            players.Sort();
            return players;
        }


        //                                                   0     1     2      3      4    5     6   7
        //                                                NAME   ARM   BAL     IP   HITS WALKS   GS   SV
        public static String[,] TEST_TEAM_NORMAL = {{ "StartP4", "R", "2R", "120", "125", "42", "18", "1" },
                                                    { "StartP5", "R", "1L", "180", "209", "48", "32", "0" },
                                                    { "StartP2", "R", "2L",  "96", "103", "21", "17", "0" },
                                                    { "StartP3", "R", "2L", "178", "180", "41", "30", "0" },
                                                    { "StartP1", "R",  "E", "182", "152", "44", "32", "0" },
                                                    { "StartP6", "R", "5L",  "65",  "68", "19", "12", "0" },
                                                    { "StartP7", "R",  "E",  "54",  "51", "31",  "4", "0" },
                                                    { "StartP8", "R", "1R",  "45",  "41", "13",  "8", "0" },
                                                    { "Relief1", "R", "3R",  "57",  "64", "22",  "0", "2" },
                                                    { "Relief2", "R", "4R",  "41",  "50", "14",  "0", "0" },
                                                    { "Relief3", "R", "9L",  "81",  "50", "26",  "0", "4" },
                                                    { "NoCount", "R", "2R",  "25",  "21", "15",  "0", "6" },
                                                    { "Closer1", "L", "5L",  "34",  "10", "62",  "0","24"},
                                                    { "Relief4", "R", "3L",  "43",  "14", "64",  "0", "1" },
                                                    { "Relief5", "R", "4L",  "59",  "67", "22",  "0", "0" },
                                                    { "Relief6", "R", "5R",  "52",  "37", "11",  "0", "0" } };

        public static String[,] TEST_FULL_TEST_SQUAD= {
                                                          { "SPFullTime2",   "R",  "E","175", "155", "20", "27", "0" },
                                                          { "SPFullTime1",   "R",  "E","200", "155", "20", "28", "0" },
                                                          { "ReliefLow",     "R",  "E", "25", "25", "20",  "0",  "0" },
                                                          { "SPFullTime5",   "R",  "E","75",   "55", "20", "10", "0" },
                                                          { "SPFullTime6",   "R",  "E","50",   "30", "20", "9", "0" },
                                                          { "SPFullTimeBad", "R",  "E","150", "200", "20", "20", "0" },
                                                          { "SPFullTimeYuck","R",  "E","150", "250", "20", "20", "0" },
                                                          { "SPPartTimeAce", "R",  "E","100",  "60", "10", "15", "0" },
                                                          { "SPPartTime3",   "R",  "E", "75",  "78", "20", "12", "0" },
                                                          { "SPPartTime2",   "R",  "E", "75",  "75", "20", "12", "0" },
                                                          { "SPPartBad",     "R",  "E", "75",  "90", "20", "12", "0" },
                                                          { "SPLowIP",       "R",  "E", "25",  "25", "20", "8",  "0" },
                                                          { "Closer1",       "R",  "E", "75",  "50", "20", "0",  "25" },
                                                          { "SPFullTime3",   "R",  "E","175", "160", "20", "26", "0" },
                                                          { "Closer2",       "R",  "E", "75",  "75", "20", "0",  "24" },
                                                          { "CloserLow",     "R",  "E", "20", "20", "20",  "0",  "9" },
                                                          { "ReliefAce",     "R",  "E","100", "75", "20",  "2",  "3" },
                                                          { "ReliefHigh",    "R",  "E","100", "80", "20",  "0",  "0" },
                                                          { "ReliefMed",     "R",  "E", "75", "55", "20",  "0",  "0" },
                                                          { "SPFullTime4",   "R",  "E","110", "80", "20",  "15", "0" },
                                                          { "ReliefNorm",    "R",  "E", "50", "30", "20",  "0",  "0" },
                                                          { "SPFullTimeAce", "R",  "E","200", "152", "20", "32", "0" },
                                                          { "ReliefNorm2",   "R",  "E", "50", "40", "20",  "0",  "0" },
                                                          { "SPPartTime1",   "R",  "E", "75",  "72", "20", "12", "0" },
                                                          { "ReliefBad",     "R",  "E", "50", "75", "20",  "0",  "0" },
                                                          { "ReliefYuck",    "R",  "E", "40", "75", "20",  "0",  "0" }
        };

        //SP = 1098 ( 176= 16%) 96=8%

        private String[,] TEST_TEAM3 = {{ "StartP5","L","26","3L","6","6","4.81","86","88","26","81","16",    "15","0" },
                                        { "Relief1","R","41","3L","3","6","2.95","64","56","29","64","9",      "0","0" },
                                        { "Relief2","R","36","5R","7","3","3.88","70","57","28","83","7",      "0","1" },
                                        { "StartP6","R","47","3L","2","4","5.05","46","52","11","30","8",      "9","0" },
                                        { "Relief7","L","42","6L","1","0","4.89","35","37","10","28","7",      "0","0" },
                                        { "Relief6","R","44","1L","3","6","3.99","59","52","18","46","4",      "0","2" },
                                        { "StartP4","R","43","1L","3","11","5.43","124","157","49","75","18", "23","0" },
                                        { "DoNoUse","L","25","3L","1","1","6.08","40","59","11","32","8",      "8","0" },
                                        { "Relief8","R","39","2L","4","2","4.22","64","57","23","78","7",      "0","1" },
                                        { "StartP1","L","24","9R","6","15","5.52","174","200","67","148","27","31","0" },
                                        { "Relief","R","52","3R","5","3","5.01","74","88","20","68","16",    "13","0" },
                                        { "StartP2","L","16","1L","17","8","2.90","214","165","43","308","24","32","0" },
                                        { "Relief3","L","36","8L","4","1","3.77","45","35","25","58","8",      "0","0" },
                                        { "StartP3","R","43","E","9","11","5.31","166","165","84","148","22", "30","0" },
                                        { "Relief4","R","39","5R","1","0","3.20","45","50","13","27","4",      "0","0" },
                                        { "StartP7","R","44","7L","7","5","5.31","83","103","22","66","11",    "8","1" },
                                        { "Relief5","R","50","8R","2","3","4.15","35","39","8","26","5",       "4","0" }};
        //SP = 956 (153=16%) 83=8%

        public static String[,] TEST_TEAM4 = {{ "Relief4","R","40","2L","1","1","5.00","27","27","15","26","7",       "0","0" },
                                        { "StartP1","R","48","4R","10","13","4.79","154","169","59","143","20","28","0" },
                                        { "Closer1","R","51","4L","1","4","1.78","76","48","40","126","6",      "0","39" },
                                        { "StartP2","R","39","4R","5","6","4.39","131","123","31","109","22",  "19","1" },
                                        { "StartP3","L","20","2R","15","5","2.89","162","116","71","218","23", "28","0" },
                                        { "Relief3","R","51","1R","3","4","3.40","48","37","20","76","3",       "0","11" },
                                        { "StartP7","R","47","1R","6","3","4.52","78","73","28","69","15",     "14","0" },
                                        { "StartP","L","41","2R","1","1","4.81","39","39","22","43","5",       "0","1" },
                                        { "StartP8","R","42","2L","5","6","4.68","85","81","53","86","12",     "11","1" },
                                        { "Relief2","R","50","1L","1","2","2.97","30","29","3","34","0",        "7","0" },
                                        { "StartP9","R","35","9L","3","2","3.88","46","50","22","35","9",       "8","0" },
                                        { "Relief5","R","42","6R","3","4","5.45","74","84","33","84","8",       "0","0" },
                                        { "StartP4","R","51","6R","7","8","4.57","114","88","54","139","18",   "21","0" },
                                        { "StartP6","R","50","3R","4","7","4.26","89","90","24","52","13",     "15","0" },
                                        { "Relief1","R","42","5R","3","2","4.19","43","41","14","46","10",      "0","1" },
                                        { "StartP5","L","21","3R","3","8","7.39","71","74","40","63","23",     "14","0" },
                                        { "StartP","L","30","2L","0","3","6.93","62","81","31","57","12",      "4","0" }};
        //SP= 1022(163=16%)   89=8%

        private int hackyLookupIndex(int balance, String balanceArm)
        {
            if (balanceArm.Equals("L"))
            {
                return 9 - balance;
            }
            else if (balanceArm.Equals("R"))
            {
                return 9 + balance;

            }
            else return 9; //Even
        }
    }
}