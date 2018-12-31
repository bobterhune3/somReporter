using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using LIneupUsageEstimator;
using LineupEngine;

namespace somReporter.team
{
    [TestClass]
    public class TestTeamReportBalanceFinder
    {

        private List<LineupBalanceItem> balanceItems;
        private const String IN_DIVISION =  "InDiv";
        private const String OUT_DIVISION = "OutDiv";

        [TestInitialize()]
        public void Initialize()
        {
            balanceItems = LineupTools.buildDefaultLineupTypes();
            RecordIndex.resetIndex(RecordIndex.INDEX.TestTeamId);
            RecordIndex.resetIndex(RecordIndex.INDEX.TestLineupDataId);
        }

        [TestCleanup()]
        public void Cleanup() { }
       
        [TestMethod]
        public void test_SP_Straight_Calculation()
        {
            Dictionary<Team, List<Player>> pitcherDataByTeam = new Dictionary<Team, List<Player>>();
            setupTestTeams(pitcherDataByTeam);

            Dictionary<String, List<Player>> pitcherDataByBalance = SOMTeamReportFile.organizePitcherByBalance(pitcherDataByTeam);
            SOMTeamReportFile teamReportFile = new SOMTeamReportFile(pitcherDataByBalance, pitcherDataByTeam);

            IUsageCalculator calculator = CalculatorFactory.getCalculator(CalculatorFactory.CalculatorType.SP_BASIC, teamReportFile, buildTeam("TEST1", IN_DIVISION));
            List<Dictionary<int, int>> results = calculator.calculate();

            verifyBasicLineupStructure(results);

            // Test L-1R
            runLineupTest(results, createTestTeamLineup("L", "1R","1R"), 11);

            // Test L 9L->9R
            runLineupTest(results, createTestTeamLineup("L", "9L", "9R"), 152);

            // Test R 9R->9R
            runLineupTest(results, createTestTeamLineup("R", "9L", "9R"), 473);

            // Test R E 
            runLineupTest(results, createTestTeamLineup("R", "E", "E"), 61);

            // Test R 6R-9R 
            runLineupTest(results, createTestTeamLineup("R", "6R", "9R"), 24);

            // Test L 6R-8R 
            runLineupTest(results, createTestTeamLineup("L", "6R", "8R"), 0);
        }

        [TestMethod]
        public void test_SP_Straight_Based_On_Original_Spreadsheet()
        {
            Dictionary<Team, List<Player>> pitcherDataByTeam = new Dictionary<Team, List<Player>>();
            setupTestTeams(pitcherDataByTeam);

            Dictionary<String, List<Player>> pitcherDataByBalance = SOMTeamReportFile.organizePitcherByBalance(pitcherDataByTeam);
            SOMTeamReportFile teamReportFile = new SOMTeamReportFile(pitcherDataByBalance, pitcherDataByTeam);

            IUsageCalculator calculator = CalculatorFactory.getCalculator(CalculatorFactory.CalculatorType.SP_BASIC, teamReportFile, buildTeam("TEST1", IN_DIVISION));
            List<Dictionary<int, int>> results = calculator.calculate();

            verifyBasicLineupStructure(results);

            int idx = 0;
            for (int i = 8; i >= 0; i--)
            {
                int[] expectedLLeft = { 0,0,0,0,0,0,19,10,33 };
                int[] expectedLRight = { 7,0,13,0,10,0,7,95,94};

                // Test L-1R
                runLineupTest(results, createTestTeamLineup("L",(i+1) + "L", (i + 1) + "L"), expectedLLeft[idx]);
                runLineupTest(results, createTestTeamLineup("R", (i + 1) + "L", (i + 1) + "L"), expectedLRight[idx]);
                idx++;
            }

            int expectedELeft = 16;
            int expectedERight = 61;
            runLineupTest(results, createTestTeamLineup("L", "E", "E"), expectedELeft);
            runLineupTest(results, createTestTeamLineup("R", "E", "E"), expectedERight);

            for ( int i=0; i<=8; i++ )
            {
                int[] expectedRLeft = {11,25,11,0,0,0,0,0,27};
                int[] expectedRRight = {29,56,34,43,0,18,0,6,0};

                // Test L-1R
                runLineupTest(results, createTestTeamLineup("L", (i + 1) + "R", (i + 1) + "R"), expectedRLeft[i]);
                runLineupTest(results, createTestTeamLineup("R", (i + 1) + "R", (i + 1) + "R"), expectedRRight[i]);
            }
        }

        [TestMethod]
        public void test_Balance_Calculation_Multiple_Divisions()
        {
            Dictionary<Team, List<Player>> pitcherDataByTeam = new Dictionary<Team, List<Player>>();
            setupTestTeams(pitcherDataByTeam);

            Dictionary<String, List<Player>> pitcherDataByBalance = SOMTeamReportFile.organizePitcherByBalance(pitcherDataByTeam);
            SOMTeamReportFile teamReportFile = new SOMTeamReportFile(pitcherDataByBalance, pitcherDataByTeam);

            IUsageCalculator calculator = CalculatorFactory.getCalculator(CalculatorFactory.CalculatorType.SP_SCHEDULE, teamReportFile, buildTeam("TEST1", IN_DIVISION));
            calculator.setOptions(CalculatorOptions.OPTION_IN_DIVISION_GAMES, 26);
            calculator.setOptions(CalculatorOptions.OPTION_OUT_DIVISION_GAMES, 14);

            List<Dictionary<int, int>> results = calculator.calculate();

            verifyBasicLineupStructure(results);

            // Test L-1R
            runLineupTest(results, createTestTeamLineup("L", "1R", "1R"), 11);

            // Test L 9L->9R
            runLineupTest(results, createTestTeamLineup("L", "9L", "9R"), 158);

            // Test R 9R->9R
            runLineupTest(results, createTestTeamLineup("R", "9L", "9R"), 483);

            // Test R E 
            runLineupTest(results, createTestTeamLineup("R", "E", "E"), 62);

            // Test R 6R-9R 
            runLineupTest(results, createTestTeamLineup("R", "6R", "9R"), 25);
        }

        [TestMethod]
        public void test_Balance_Calculation_Multiple_Divisions_On_Original_Spreadsheet()
        {
            Dictionary<Team, List<Player>> pitcherDataByTeam = new Dictionary<Team, List<Player>>();
            setupTestTeams(pitcherDataByTeam);

            Dictionary<String, List<Player>> pitcherDataByBalance = SOMTeamReportFile.organizePitcherByBalance(pitcherDataByTeam);
            SOMTeamReportFile teamReportFile = new SOMTeamReportFile(pitcherDataByBalance, pitcherDataByTeam);

            IUsageCalculator calculator = CalculatorFactory.getCalculator(CalculatorFactory.CalculatorType.SP_SCHEDULE, teamReportFile, buildTeam("TEST1", IN_DIVISION));
            calculator.setOptions(CalculatorOptions.OPTION_IN_DIVISION_GAMES, 26);
            calculator.setOptions(CalculatorOptions.OPTION_OUT_DIVISION_GAMES, 14);

            List<Dictionary<int, int>> results = calculator.calculate();

            verifyBasicLineupStructure(results);

            int idx = 0;
            for (int i = 8; i >= 0; i--)
            {
                int[] expectedLLeft = { 0, 0, 0, 0, 0, 0, 20, 11, 34 };
                int[] expectedLRight = { 8, 0, 14, 0, 11, 0, 8, 96, 94 };

                // Test L-1R
                runLineupTest(results, createTestTeamLineup("L", (i + 1) + "L", (i + 1) + "L"), expectedLLeft[idx]);
                runLineupTest(results, createTestTeamLineup("R", (i + 1) + "L", (i + 1) + "L"), expectedLRight[idx]);
                idx++;
            }

            int expectedELeft = 17;
            int expectedERight = 62;
            runLineupTest(results, createTestTeamLineup("L", "E", "E"), expectedELeft);
            runLineupTest(results, createTestTeamLineup("R", "E", "E"), expectedERight);

            for (int i = 0; i <= 8; i++)
            {
                int[] expectedRLeft = { 11, 25, 12, 0, 0, 0, 0, 0, 28 };
                int[] expectedRRight = { 29, 57, 34, 45, 0, 19, 0, 6, 0 };

                // Test L-1R
                runLineupTest(results, createTestTeamLineup("L", (i + 1) + "R", (i + 1) + "R"), expectedRLeft[i]);
                runLineupTest(results, createTestTeamLineup("R", (i + 1) + "R", (i + 1) + "R"), expectedRRight[i]);
            }
        }


        [TestMethod]
        public void test_Balance_Calculation_All_Pitchers_Multiple_Divisions_On_Original_Spreadsheet()
        {
            Dictionary<Team, List<Player>> pitcherDataByTeam = new Dictionary<Team, List<Player>>();
            setupTestTeams(pitcherDataByTeam);

            Dictionary<String, List<Player>> pitcherDataByBalance = SOMTeamReportFile.organizePitcherByBalance(pitcherDataByTeam);
            SOMTeamReportFile teamReportFile = new SOMTeamReportFile(pitcherDataByBalance, pitcherDataByTeam);

            IUsageCalculator calculator = CalculatorFactory.getCalculator(CalculatorFactory.CalculatorType.ALL_PITCHERS_AND_SCHEDULE, teamReportFile, buildTeam("TEST1", IN_DIVISION));
            calculator.setOptions(CalculatorOptions.OPTION_IN_DIVISION_GAMES, 26);
            calculator.setOptions(CalculatorOptions.OPTION_OUT_DIVISION_GAMES, 14);

            List<Dictionary<int, int>> results = calculator.calculate();

            verifyBasicLineupStructure(results);

            int idx = 0;
            for (int i = 8; i >= 0; i--)
            {
                int[] expectedLLeft = {  0, 2, 0, 2, 11, 0, 22, 11, 36 };
                int[] expectedLRight = {22, 0,11, 0,  3,23, 14, 87, 63 };

                // Test L-1R
                runLineupTest(results, createTestTeamLineup("L", (i + 1) + "L", (i + 1) + "L"), expectedLLeft[idx]);
                runLineupTest(results, createTestTeamLineup("R", (i + 1) + "L", (i + 1) + "L"), expectedLRight[idx]);
                idx++;
            }

            int expectedELeft = 0;
            int expectedERight = 57;
            runLineupTest(results, createTestTeamLineup("L", "E", "E"), expectedELeft);
            runLineupTest(results, createTestTeamLineup("R", "E", "E"), expectedERight);

            for (int i = 0; i <= 8; i++)
            {
                int[] expectedRLeft = { 2,31,0,0,0,0,0,0,29 };
                int[] expectedRRight = { 23,54,28,47,24,31,0,7,3 };

                // Test L-1R
                runLineupTest(results, createTestTeamLineup("L", (i + 1) + "R", (i + 1) + "R"), expectedRLeft[i]);
                runLineupTest(results, createTestTeamLineup("R", (i + 1) + "R", (i + 1) + "R"), expectedRRight[i]);
            }
        }


        [TestMethod]
        public void testCalculateColumn()
        {
            int ip_for_balance = 1220;
            //int total_ip = 13854;
//            int total_ip = 12151;
            int result1 = CalculateColumnUtil.calculateColumn( ip_for_balance, .65, 18915);
            int result2 = CalculateColumnUtil.calculateColumn(ip_for_balance, .65,18171);

            return;
        }
        //
        // Utility Test Methods
        //

        private void runLineupTest(List<Dictionary<int, int>> calculatedResults, LineupDataObj lineup, int expectedAB)
        {
            int estimated = lookupEstimatedAtBatForLineup(calculatedResults, lineup);
            Assert.AreEqual(expectedAB, estimated,
                "Inital Version estimates does not match for " + lineup);
        }

        private LineupDataObj createTestTeamLineup(String arm, String start, String end )
        {
            LineupDataObj lineup = new LineupDataObj(RecordIndex.getNextId(RecordIndex.INDEX.TestLineupDataId));
            lineup.PitcherArm = arm;
            // Converts 1R to the int 1 and letter 'R'
            if( start.Equals("E"))
                lineup.BalanceItemFrom = balanceItems[hackyLookupIndex(0, start)];
            else
                lineup.BalanceItemFrom = balanceItems[hackyLookupIndex(Int32.Parse(start.Substring(0, 1)), 
                    start.Substring(1, 1))];

            if (end.Equals("E"))
                lineup.BalanceItemTo = balanceItems[hackyLookupIndex(0, start)];
            else
                lineup.BalanceItemTo = balanceItems[hackyLookupIndex(Int32.Parse(end.Substring(0, 1)), 
                    end.Substring(1, 1))];
            return lineup;
        }

        private void verifyBasicLineupStructure(List<Dictionary<int, int>> results)
        {
            Assert.AreEqual(2, results.Count, "Should have results for Lefty and Righty");
            Assert.AreEqual(19, results[0].Count, "Should have 19 types for each hand (9L->9R)");
            Assert.AreEqual(19, results[1].Count, "Should have 19 types for each hand (9L->9R)");
        }
        
        private void setupTestTeams(Dictionary<Team, List<Player>> pitcherDataByTeam)
        {
            // Populate Data engine with test data
            setupTestData(pitcherDataByTeam, ABRV[0], IN_DIVISION, TEST_TEAM1);
            setupTestData(pitcherDataByTeam, ABRV[1], IN_DIVISION, TEST_TEAM2);
            setupTestData(pitcherDataByTeam, ABRV[2], IN_DIVISION, TEST_TEAM3);
            setupTestData(pitcherDataByTeam, ABRV[3], IN_DIVISION, TEST_TEAM4);
            setupTestData(pitcherDataByTeam, ABRV[4], OUT_DIVISION, TEST_TEAM1);
            setupTestData(pitcherDataByTeam, ABRV[5], OUT_DIVISION, TEST_TEAM2);
            setupTestData(pitcherDataByTeam, ABRV[6], OUT_DIVISION, TEST_TEAM3);
            setupTestData(pitcherDataByTeam, ABRV[7], OUT_DIVISION, TEST_TEAM4);
        }

        private void setupTestData(Dictionary<Team, List<Player>> playerListByTeam, String teamAbrv, String teamDivision, String[,] data)
        {
            Team team = buildTeam(teamAbrv, teamDivision);
            List<Player> players = buildListOfTestPitchers(team, data);
            playerListByTeam.Add(team, players);
        }
        
        private Team buildTeam(String teamAbrv, String division)
        {
            Team testTeam = new Team(RecordIndex.getNextId(RecordIndex.INDEX.TestTeamId), division, 0);
            testTeam.Abrv = teamAbrv;
            return testTeam;
        }

        private int lookupEstimatedAtBatForLineup(List<Dictionary<int, int>> results, LineupDataObj lineup)
        {
            int pitcherArmIndex = lineup.PitcherArm.Equals("L") ? 0 : 1;
            LineupData data = lineup.getLineupData();
            int estimatedAtBats = SOMTeamReportFile.calculateAtBatsByLineup(results[pitcherArmIndex], data);
            return estimatedAtBats;
        }

        private List<Player> buildListOfTestPitchers(Team team, String[,] testData)
        {
            List<Player> players = new List<Player>();

            for (int i = 0; i < testData.GetLength(0); i++)
            {
                Player player = new Player();
                player.Name = testData[i, 0];
                player.IP = Int32.Parse(testData[i, 7]);
                player.Bal = testData[i, 3];
                player.Hits = Int32.Parse(testData[i, 8]);
                player.BB = Int32.Parse(testData[i, 9]);
                player.GS = Int32.Parse(testData[i, 12]);
                player.SAVE = Int32.Parse(testData[i, 13]);
                player.Team = team;
                player.IsHitter = false;
                player.Throws = testData[i, 1];
                players.Add(player);
            }
            return players;
        }

        private String[] ABRV = { "TM1", "TM2", "TM3", "TM4", "TM5", "TM6", "TM7", "TM8" };

        //                                   NAME   ARM LEFT%     BAL  WON LOST     ERA     IP    HITS WALKS K'S     HR   GS   SV
        public static String[,] TEST_TEAM1 = {{ "StartP4", "R", "43", "2R", "3", "13", "5.34", "120", "125", "42", "97", "15", "18", "1" },
                                        { "StartP5", "R", "52", "1L", "8", "13", "5.29", "180", "209", "48", "145", "26", "32", "0" },
                                        { "StartP2", "R", "43", "2L", "8", "4", "4.39", "96", "103", "21", "92", "20", "17", "0" },
                                        { "StartP3", "R", "43", "2L", "13", "12", "4.74", "178", "180", "41", "194", "35", "30", "0" },
                                        { "StartP1", "R", "47", "E", "12", "4", "2.42", "182", "152", "44", "234", "15", "32", "0" },
                                        { "StartP6", "R", "42", "5L", "5", "6", "4.27", "65", "68", "19", "50", "9", "12", "0" },
                                        { "StartP7", "R", "46", "E", "5", "2", "5.80", "54", "51", "31", "69", "7", "4", "0" },
                                        { "StartP8", "R", "39", "1R", "2", "2", "4.17", "45", "41", "13", "51", "9", "8", "0" },
                                        { "Relief1", "R", "32", "3R", "2", "1", "5.49", "57", "64", "22", "57", "7", "0", "2" },
                                        { "Relief2", "R", "30", "4R", "1", "1", "4.35", "41", "50", "14", "33", "3", "0", "0" },
                                        { "Relief3", "R", "50", "9L", "8", "5", "2.68", "81", "50", "26", "100", "11", "0", "4" },
                                        { "NoCount", "R", "43", "2R", "2", "2", "4.38", "25", "21", "15", "25", "1", "0", "6" },
                                        { "Closer1", "L", "22","5L","2","0","2.81","51","34","10","62","5","0","24"},
                                        { "Relief4", "R", "41","3L","3","5","2.61","59","43","14","64","6","0","1" },
                                        { "Relief5", "R", "35", "4L", "2", "2", "5.22", "59", "67", "22", "56", "7", "0", "0" },
                                        { "Relief6", "R", "40", "5R", "0", "0", "2.44", "52", "37", "11", "51", "8", "0", "0" } };
        //SP = 920 (147= 16%), 80=8%

        public static String[,] TEST_TEAM2 = {{ "Relief6", "R","47","2R","6","5","4.43","45","41","17","53","5",       "0","6" },
                                        { "Relief3", "L","36","2R","4","3","2.89","65","44","26","65","10",      "0","1" },
                                        { "StartP1", "R","51","2R","8","15","4.69","148","136","77","120","20", "25","1" },
                                        { "StartP2", "R","41","2L","12","10","3.66","179","175","44","128","22","29","0" },
                                        { "StartP3", "L","25","E","8","8","6.14","103","114","42","72","19",    "20","0" },
                                        { "StartP4", "R","44","1L","9","10","5.58","129","133","53","105","28", "24","0" },
                                        { "StartP9", "R","57","1R","1","5","6.16","64","79","12","35","16",     "11","0" },
                                        { "LowUse1", "R","51","2R","2","2","4.68","25","20","12","22","4",       "0","0" },
                                        { "Relief5", "R","44","3R","8","4","4.45","83","78","34","80","9",       "0","2" },
                                        { "Relief4", "L","42","5L","3","3","4.12","55","51","18","63","10",      "0","0" },
                                        { "NoCount", "R","47","3R","5","4","7.85","57","73","32","52","10",      "8","0" },
                                        { "Closer1", "R","52","3R","5","4","4.23","55","40","26","65","3",       "0","39" },
                                        { "StartP8", "L","24","1R","2","5","4.15","69","64","31","76","12",     "12","0" },
                                        { "StartP6", "R","46","2R","5","6","4.28","103","94","44","145","14",   "19","0" },
                                        { "StartP5", "R","48","1L","9","9","3.49","157","148","61","146","17",  "28","0" },
                                        { "StartP7", "R","46","2L","3","7","3.12","89","64","32","98","11",     "15","0" },
                                        { "Relief1", "R","34","9R","2","3","3.22","50","39","8","56","7",        "0","0" },
                                        { "Relief2", "R","39","7L","3","6","3.55","66","48","24","62","11",      "0","0" }};
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