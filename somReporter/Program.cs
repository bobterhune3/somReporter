using Microsoft.Isam.Esent.Collections.Generic;
using somReporter.output;
using somReporter.team;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using somReporter.reports;
using somReporter.util;
using somReporter.features;

namespace somReporter
{
    public class Program
    {
        public static string[] LEAGUES = { "" };
        //      private readonly string[] LEAGUES = { "AL", "NL" };
        public static string[] DIVISIONS = { "Federal", "American", "National" };
        public static string[] DIVISION_DRAFT_ORDER = { "National", "American", "Federal" };
        //      private readonly string[] DIVISIONS = { "East", "West" };

        public static IFeature featureStandings = null;

        private LeagueGrandTotalsReport leaguePrimaryStatReport;
        private LineScoreReport lineScoreReport;
        private NewspaperStyleReport newspaperStyleReport;
        private RecordBookReport recordBookReport;
        private ComparisonReport teamComparisonReport;

        private SOMReportFile leagueReportFile;
        private SOMReportFile teamReportFile;

        private IOutput output;

        public Program() {
        //    output = new ConsoleOutput();
            output = new HTMLOutput();
        }

        public void cleanup() {
            output.setOutputFooter();
        }

        public IOutput outputStream() {
            return output;
        }                                    

        static void Main(string[] args)
        {
            Program program = new Program();
            Config cfg = new Config("config.properties");

            Console.WriteLine("Intializing...");
            program.initialize();
      
            string fileName = program.lookupPreviousSaveFile();
            if( fileName.Length > 0 ) {
                PersistentDictionary<string, string> prevDictionaryFile = 
                    new PersistentDictionary<string, string>(fileName);
                program.loadPreviousStorageInfo(prevDictionaryFile);
            }

            featureStandings = FeatureFactory.loadFeature(FeatureFactory.FEATURE.STANDINGS);
            featureStandings.process(program.outputStream());

            Console.WriteLine("Process Who Hot...");
            program.showWhosHot();


            Console.WriteLine("Process Draft Order...");
            if (Config.STRAIGHT_DRAFT_ORDER)
                program.processDraftOrder();
            else
                program.processTierdDraftOrder();

            Console.WriteLine("Process Record Book...");
            program.processRecordBook();

            Console.WriteLine("Process Player Usage ...");
            program.processPlayerUsage();

            Console.WriteLine("Create Win Pct History Charts ...");
            ((FeatureStandings)featureStandings).buildCharts();

            Console.WriteLine("Press ESC to stop or S to save");
            ConsoleKey key;
            do
            {
                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.S)
                {
                    Console.WriteLine("Saving Data..");
                    program.saveReportInformation();
                    break;
                }
            } while (key != ConsoleKey.Escape);


            program.cleanup();
        }

        private void showWhosHot()
        {
            output.ShowWhosHotData(newspaperStyleReport.getWhosHotData());
        }

        public String lookupPreviousSaveFile()
        {
            string[] directories = Directory.GetDirectories(Directory.GetCurrentDirectory(), leagueReportFile.SeasonTitle+"*");
            if (directories.Length == 0)
                return "";

            int highestValue = 0;
            foreach (string dir in directories)
            {
                string value = dir.Substring(dir.IndexOf('-')+1);
                if( value.Length > 1)
                {
                    long theValue = 0;
                    Int64.TryParse(value, out theValue);
                    if (theValue > highestValue)
                        highestValue = (int)theValue;
                }
            }

            if (highestValue == 0)
                return "";

            string fileName = String.Format("{0}-{1}", leagueReportFile.SeasonTitle, highestValue);
            return fileName;
        }

        public void loadPreviousStorageInfo(PersistentDictionary<string, string>  prevDictionaryFile)
        {
            this.leaguePrimaryStatReport.loadPreviousStorageInfo(prevDictionaryFile);
        }

        public void initialize() {
            Report.DATABASE.reset();

            Console.WriteLine("  Loading League Report File ...");
            leagueReportFile = new SOMReportFile(Config.getConfigurationFile("ALL_REPORTS.PRT"));
            leagueReportFile.parseLeagueFile();


            IFeature feature = FeatureFactory.loadFeature(FeatureFactory.FEATURE.STANDINGS);
            feature.initialize(leagueReportFile);

            Console.WriteLine("    Building Grand Totals...");
            leaguePrimaryStatReport = (LeagueGrandTotalsReport)leagueReportFile.FindReport("LEAGUE GRAND TOTALS (primary report) FOR");
            leaguePrimaryStatReport.processReport();

            Console.WriteLine("    Building League Report ...");
            lineScoreReport = (LineScoreReport)leagueReportFile.FindReport("INJURY/MINOR LEAGUE REPORT FOR");
            lineScoreReport.processReport();

            Console.WriteLine("    Building Awards...");
            newspaperStyleReport = (NewspaperStyleReport)leagueReportFile.FindReport("AWARDS VOTING FOR");
            newspaperStyleReport.processReport();

            Console.WriteLine("    Building Record Book...");
            recordBookReport = (RecordBookReport)leagueReportFile.FindReport("RECORD BOOK FOR FOR");
            recordBookReport.processReport();

            output.setOutputHeader(leagueReportFile.SeasonTitle);

            Console.WriteLine("  Loading Team Report File...");
            teamReportFile = new SOMReportFile(Config.getConfigurationFile("TEAM_ALL_REPORTS.PRT"));
            teamReportFile.parseTeamFile();

            Console.WriteLine("    Building Comparison...");
            Console.WriteLine("      Showing Moral="+ Config.SHOW_MORAL+", Showing Warnings="+ Config.SHOW_WARNING);

            teamComparisonReport = (ComparisonReport)teamReportFile.FindReport("Comparison Report");
            teamComparisonReport.processReport();
        }

        public void saveReportInformation()
        {
            Report.saveReportInformation(buildReportDBName());
        }

        private String buildReportDBName()
        {
            return String.Format("{0}-{1}", leagueReportFile.SeasonTitle, Team.TOTAL_GAMES);
        }

        public void processDraftOrder()
        {
            output.draftOrderHeader();

            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.OrderAscending = true;
            scope.AllTeams = true;

            output.draftOrderTableHeader();


            List<Team> teams = ((LeagueStandingsReport)featureStandings.getReport()).
                                    getTeamsByWinPercentage(scope);
            int pickNum = 0;
            List<Team> tieBreakerList = new List<Team>();
            Team prevTeam = null;

            foreach (Team team in teams)
            {
                if (prevTeam == null) {
                    prevTeam = team;
                }
                else if(team.Wpct == prevTeam.Wpct)
                {
                    if(tieBreakerList.Count == 0)
                        tieBreakerList.Add(prevTeam);
                    tieBreakerList.Add(team);
                }
                else
                {
                    if (tieBreakerList.Count > 0)
                    {
                        foreach( Team tbTeam in tieBreakerList)
                        {
                            pickNum++;
                            WriteOutTeamForDraftPicks(pickNum, 0, tbTeam);
                        }
                        tieBreakerList.Clear();
                        prevTeam = team;
                    }
                    else {
                        pickNum++;
                        WriteOutTeamForDraftPicks(pickNum, 0, prevTeam);
                        prevTeam = team;
                    }
                }
            }
            pickNum++;
            WriteOutTeamForDraftPicks(pickNum, 0, prevTeam);
            output.endOfTable();
        }

        public void processTierdDraftOrder() {
            output.draftOrderHeader();

            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.OrderAscending = true;
            scope.League = "X";

            // Get Team Draft order by division
            Dictionary<String, List<Team>> draftOrder = new Dictionary<string, List<Team>>();
            foreach (string division in DIVISIONS) {
                scope.Division = division;
                draftOrder[division] = ((LeagueStandingsReport)featureStandings.getReport()).getTeamsByWinPercentage(scope);
            }

            output.draftOrderTableHeader();

            int pickNum = 0;
            List<Team> tieBreakerList = new List<Team>();
            Team prevTeam = null;
            Team lastTeam = null;
            Team[] picks = new Team[18];

            foreach ( String division in Program.DIVISION_DRAFT_ORDER) {
                List<Team> teams = draftOrder[division];
                
                foreach (Team team in teams)
                {
                    if (prevTeam == null)
                    {
                        prevTeam = team;
                    }
                    else if (team.Wpct == prevTeam.Wpct)
                    {
                        if (tieBreakerList.Count == 0)
                            tieBreakerList.Add(prevTeam);
                        tieBreakerList.Add(team);
                    }
                    else
                    {
                        if (tieBreakerList.Count > 0)
                        {
                            foreach (Team tbTeam in tieBreakerList)
                            {
                            //    pickNum++;
                                picks[pickNum++] = tbTeam;
       //                         WriteOutTeamForDraftPicks(pickNum, tbTeam);
                            }
                            tieBreakerList.Clear();
                            prevTeam = team;
                        }
                        else
                        {
                       //     pickNum++;
                            picks[pickNum++] = prevTeam;
    //                        WriteOutTeamForDraftPicks(pickNum, prevTeam);
                            prevTeam = team;
                        }
                    }
                }
            }

            if (tieBreakerList.Count > 0)
            {
                foreach (Team tbTeam in tieBreakerList)
                {
                    //    pickNum++;
                    picks[pickNum++] = tbTeam;
                    //                         WriteOutTeamForDraftPicks(pickNum, tbTeam);
                }
                tieBreakerList.Clear();

            }
            else
            {
                picks[pickNum++] = prevTeam;
                
            }

            Team[] actualPicks = new Team[18];

            // Re-organize the picks, Paren is by division draft order
            //    1-4 - Draft (1-4)
            //    5.A6  (6)    12.A2 (14)
            //    6.N2  (8)    13.F5 (11)
            //    7.A5  (5)    14.A1 (13)
            //    8.N1  (7)    15.F4 (15)
            //    9.A4  (9)    16.F3 (16)
            //    10.A3 (10)   17.F2 (17)
            //    11.F6 (12)   18.F1 (18)
            for (int i=0; i< picks.Length; i++) {
                if (i < 4 || i > 13) {
                    actualPicks[i] = picks[i];
                    continue;
                }
                switch(i) {
                    case 4: actualPicks[5] = picks[i]; break;
                    case 5: actualPicks[7] = picks[i]; break;
                    case 6: actualPicks[4] = picks[i]; break;
                    case 7: actualPicks[6] = picks[i]; break;
                    case 8: actualPicks[8] = picks[i]; break;
                    case 9: actualPicks[9] = picks[i]; break;
                    case 10: actualPicks[11] = picks[i]; break;
                    case 11: actualPicks[13] = picks[i]; break;
                    case 12: actualPicks[10] = picks[i]; break;
                    case 13: actualPicks[12] = picks[i]; break;
                }
            }

            for (int i = 1; i < actualPicks.Length; i++)
            {
                WriteOutTeamForDraftPicks(i, 1, actualPicks[i]);
            }
            output.endOfTable();
        }

 

 
 


        private void processRecordBook()
        {
            int counter = 1;
            List<SOMRecord> teamRecords = ((RecordBookReport)recordBookReport).getTeamRecords();
            if(teamRecords.Count > 0) {
                output.recordBookHeader(true);
                foreach( SOMRecord rec in teamRecords) {
                    output.recordBookItem(rec, counter++, true);
                }
            }

            counter = 1;
            List<SOMRecord> playerRecords = ((RecordBookReport)recordBookReport).getPlayerRecords();
            if (playerRecords.Count > 0)
            {
                output.recordBookHeader(false);
                foreach (SOMRecord rec in playerRecords)  {
                    output.recordBookItem(rec, counter++, false);
                }
            }
            output.spacer();
        }

 
        private void WriteOutTeamForDraftPicks(int pickNum, int divPick, Team team ) {
            output.draftOrderTeamLine(pickNum, divPick, team);
            team.DraftPickPositionCurrent = pickNum;
         }

        private void WriteOutLeadingTeamForWildCard(Team team, Team secondPlaceTeam)
        {
            double gb = secondPlaceTeam.calculateGamesBehind(team);
            WriteOutTeamForWildCard(1, team, gb);
        }

        private void WriteOutTeamForWildCard(int rank, Team team, double wcGamesBehind)
        {
            team.WildCardPositionCurrent = rank;

            String gamesBehind = "---";
            if (wcGamesBehind != 999) {
                gamesBehind = String.Format("{0}",wcGamesBehind);
            }

            output.wildCardTeamLine(rank, team, gamesBehind);
        }

        public void processPlayerUsage()
        {
            output.usageHeader();
            int counter = 1;
            List<Player> players = teamComparisonReport.getPlayers();
            string currentTeam = "";
            foreach (Player player in players)
            {
                if( !currentTeam.Equals(player.Team))
                    counter = 1;
                currentTeam = player.Team;
                if (output.usageReportItem(player, counter))
                    counter++;
            }
        }
    }
}
