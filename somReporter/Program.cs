using Microsoft.Isam.Esent.Collections.Generic;
using somReporter.output;
using somReporter.team;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using somReporter.reports;

namespace somReporter
{
    public class Program
    {
        public static string[] LEAGUES = { "" };
        //      private readonly string[] LEAGUES = { "AL", "NL" };
        public static string[] DIVISIONS = { "Federal", "American", "National" };
        public static string[] DIVISION_DRAFT_ORDER = { "National", "American", "Federal" };
        //      private readonly string[] DIVISIONS = { "East", "West" };
        private static bool HAS_WILDCARD = false;
        private static bool RANK_STATS_BY_DIVISION = true;
        private static bool STRAIGHT_DRAFT_ORDER = false;

        private LeagueStandingsReport leagueStandingsReport;
        private LeagueGrandTotalsReport leaguePrimaryStatReport;
        private LineScoreReport lineScoreReport;
        private NewspaperStyleReport newspaperStyleReport;
        private RecordBookReport recordBookReport;
        private ComparisonReport teamComparisonReport;

        private SOMReportFile leagueReportFile;
        private SOMReportFile teamReportFile;

        public static bool SHOW_WARNING = false;
        public static bool SHOW_MORAL = false;

        IOutput output;
        public Program() {
        //    output = new ConsoleOutput();
            output = new HTMLOutput();
        }

        public void cleanup() {
            output.setOutputFooter();
        }


        static void Main(string[] args)
        {
            Program program = new Program();

            Console.WriteLine("Intializing...");
            program.initialize();
       
            string fileName = program.lookupPreviousSaveFile();
            if( fileName.Length > 0 ) {
                PersistentDictionary<string, string> prevDictionaryFile = 
                    new PersistentDictionary<string, string>(fileName);
                program.loadPreviousStorageInfo(prevDictionaryFile);
            }

            Console.WriteLine("Process Standings...");
            program.processStandings();

            Console.WriteLine("Process Who Hot...");
            program.showWhosHot();

            if(HAS_WILDCARD) {
                Console.WriteLine("Process Wild Card...");
                program.processWildCardStandings();
            }

            Console.WriteLine("Process Draft Order...");
            if (Program.STRAIGHT_DRAFT_ORDER)
                program.processDraftOrder();
            else
                program.processTierdDraftOrder();

            Console.WriteLine("Process Record Book...");
            program.processRecordBook();

            Console.WriteLine("Process Player Usage ...");
            program.processPlayerUsage();

            Console.WriteLine("Create Win Pct History Charts ...");
            program.buildCharts();

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
            leagueReportFile = new SOMReportFile("ALL_REPORTS.PRT");
            leagueReportFile.parseLeagueFile();

            Console.WriteLine("    Building Standings...");
            leagueStandingsReport = (LeagueStandingsReport)leagueReportFile.FindReport("LEAGUE STANDINGS FOR");
            leagueStandingsReport.processReport();

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
            teamReportFile = new SOMReportFile("TEAM_ALL_REPORTS.PRT");
            teamReportFile.parseTeamFile();

            Console.WriteLine("    Building Comparison...");
            Console.WriteLine("      Showing Moral="+Program.SHOW_MORAL+", Showing Warnings="+Program.SHOW_WARNING);

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

            List<Team> teams = leagueStandingsReport.getTeamsByWinPercentage(scope);
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
                draftOrder[division] = leagueStandingsReport.getTeamsByWinPercentage(scope);
            }

            output.draftOrderTableHeader();

            int pickNum = 0;
            List<Team> tieBreakerList = new List<Team>();
            Team prevTeam = null;

            Team[] picks = new Team[19];

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
                                pickNum++;
                                picks[pickNum] = tbTeam;
       //                         WriteOutTeamForDraftPicks(pickNum, tbTeam);
                            }
                            tieBreakerList.Clear();
                            prevTeam = team;
                        }
                        else
                        {
                            pickNum++;
                            picks[pickNum] = prevTeam;
    //                        WriteOutTeamForDraftPicks(pickNum, prevTeam);
                            prevTeam = team;
                        }
                    }
                }
            }
            pickNum++;
            picks[pickNum] = prevTeam;
            Team[] actualPicks = new Team[19];

            // Re-organize the picks, Paren is by division draft order
            //    1-4 - Draft (1-4)
            //    5.A6  (6)    12.A2 (14)
            //    6.N2  (8)    13.F5 (11)
            //    7.A5  (5)    14.A1 (13)
            //    8.N1  (7)    15.F4 (15)
            //    9.A4  (9)    16.F3 (16)
            //    10.A3 (10)   17.F2 (17)
            //    11.F6 (12)   18.F1 (18)
            for (int i=1; i< picks.Length; i++) {
                if (i < 5 || i > 14) {
                    actualPicks[i] = picks[i];
                    continue;
                }
                switch(i) {
                    case 5: actualPicks[6] = picks[i]; break;
                    case 6: actualPicks[8] = picks[i]; break;
                    case 7: actualPicks[5] = picks[i]; break;
                    case 8: actualPicks[7] = picks[i]; break;
                    case 9: actualPicks[9] = picks[i]; break;
                    case 10: actualPicks[10] = picks[i]; break;
                    case 11: actualPicks[12] = picks[i]; break;
                    case 12: actualPicks[14] = picks[i]; break;
                    case 13: actualPicks[11] = picks[i]; break;
                    case 14: actualPicks[13] = picks[i]; break;
                }
            }

            for (int i = 1; i < actualPicks.Length; i++)
            {
                WriteOutTeamForDraftPicks(i, 1, actualPicks[i]);
            }
            output.endOfTable();
        }

        public void processStandings() {

            foreach (string league in LEAGUES)
            {
                if(RANK_STATS_BY_DIVISION) { 
                    foreach (string division in DIVISIONS)
                    {
                        leagueStandingsReport.calculateHighLowTeamStats(league, division, Team.CATEGORY.BATTING_AVERAGE);
                        leagueStandingsReport.calculateHighLowTeamStats(league, division, Team.CATEGORY.HOME_RUNS);
                        leagueStandingsReport.calculateHighLowTeamStats(league, division, Team.CATEGORY.EARNED_RUNS_AVG);

                    }
                }
                else
                {
                    leagueStandingsReport.calculateHighLowTeamStats(league, "", Team.CATEGORY.BATTING_AVERAGE);
                    leagueStandingsReport.calculateHighLowTeamStats(league, "", Team.CATEGORY.HOME_RUNS);
                    leagueStandingsReport.calculateHighLowTeamStats(league, "", Team.CATEGORY.EARNED_RUNS_AVG);
                }
            }

            Dictionary<String, Dictionary<String, List<Team>>> teams = new Dictionary<String, Dictionary<String, List<Team>>>();

            foreach (string league in LEAGUES)
            {
                foreach (string division in DIVISIONS)
                {
                    Dictionary<String, List<Team>> divs = null;
                    if (teams.Count > 0)
                        divs = teams[league];
                    else
                        teams[""] = new Dictionary<string, List<Team>>();

                    if (divs == null)
                    {
                        divs = new Dictionary<String, List<Team>>();
                        teams[league] = divs;
                    }

                    divs[division] = getStandings(league, division);
                }
            }

            foreach (string league in LEAGUES)
            {
                foreach (string division in DIVISIONS)
                {
                    List<Team> div = teams[league][division];
                    processDivision(league+division, div);
                }
            }
        }

        private void processDivision( string division, List<Team> teams) {
            output.divisionStandingsHeader(division);
            output.divisionStandingsTableHeader();

            int rank = 1;
            foreach(Team team in teams) {
                team.DivisionPositionCurrent = rank;
                output.divisionStandingsTeamLine(rank++, team);
            }
            output.endOfTable();
        }

        private List<Team> getWildCardTeams(string league)
        {
            List<Team> teams = new List<Team>();

            List<Team> teamsEast = getStandings(league, "East");
            List<Team> teamsWest = getStandings(league, "West");

            teamsEast.RemoveAt(0);
            teamsWest.RemoveAt(0);

            teams.AddRange(teamsEast);
            teams.AddRange(teamsWest);

            sortLeagueByWinningPct(teams);

            return teams;
        }


        public void processWildCardStandings() {
            output.spacer();

            List<Team> teamsAL = getWildCardTeams("AL");
            List<Team> teamsNL = getWildCardTeams("NL");

            output.wildCardHeader("AL");
            output.wildCardTableHeader();

            writeOutLeagueWildcards(teamsAL);
            output.endOfTable();

            output.spacer();

            output.wildCardHeader("NL");
            output.wildCardTableHeader();

            writeOutLeagueWildcards(teamsNL);
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


        private void writeOutLeagueWildcards( List<Team> teams ) {
             Team secondPlaceTeam = teams.ElementAt(1);
            WriteOutLeadingTeamForWildCard(teams.ElementAt(0), secondPlaceTeam);
            WriteOutTeamForWildCard(2, secondPlaceTeam, 999);

            for( int i=2; i<teams.Count; i++)
            {
                Team team = teams.ElementAt(i);
                double gb = team.calculateGamesBehind(secondPlaceTeam);
                if( gb < 11.5 )
                   WriteOutTeamForWildCard(i+1, team, gb*-1.0);
            }
        }

        private void sortLeagueByWinningPct(List<Team> teams) { 
            teams.Sort(delegate (Team x, Team y)
            {
                return y.Wpct.CompareTo(x.Wpct);
            });
        }

        private List<Team> getStandings( String league, String division) {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.OrderAscending = false;
            scope.Division = division;
            scope.League = Program.LEAGUES[0].Length == 0 ? "X" : league;
            return leagueStandingsReport.getTeamsByWinPercentage(scope); 
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

        public void buildCharts()
        {
            TeamWinPctHistory teamWinPctHistory = new TeamWinPctHistory();
            bool firstTimeLoad = !teamWinPctHistory.loadWinPctFile(@"wpct.csv");

            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.OrderAscending = true;
            scope.AllTeams = true;

            // Only create and save if additional games added.
            List<Team> teams = leagueStandingsReport.getTeamsByWinPercentage(scope);
            if( teamWinPctHistory.addCurrentSeason(teams))
            {
                if(!firstTimeLoad)
                {
                    // Create Division Charts
                    foreach (string league in LEAGUES)
                    {
                        foreach (string division in DIVISIONS)
                            buildDivisionChart(league, division);
                    }
                }

                teamWinPctHistory.csvSaveTeamParser(@"wpct.csv");
            }

            buildDraftOrderChart(teams);

            if(HAS_WILDCARD) {
                List<Team> teamsAL = getWildCardTeams("AL");
                buildWildcardChart(teamsAL, "AL");           
                
                List<Team> teamsNL = getWildCardTeams("NL");
                buildWildcardChart(teamsNL, "NL");
            }
        }

        private void buildDivisionChart(String league, String division) {
            List<Team> teams= getStandings(league, division);
            LineGraph lg = new LineGraph();
            lg.setGraphData("Trend Report for "+ league +" " + division, teams, false);
            lg.save(String.Format("winpct_{0}{1}.html", league, division.ToUpper()));
        }

        private void buildWildcardChart(List<Team> teams, String league)
        {
            LineGraph lg = new LineGraph();
            Team secondPlaceTeam = teams.ElementAt(1);
            List<Team> chartedTeams = new List<Team>();
            chartedTeams.Add(teams[0]);
            chartedTeams.Add(teams[1]);

            for (int i = 2; i < teams.Count; i++)
            {
                Team team = teams.ElementAt(i);
                double gb = team.calculateGamesBehind(secondPlaceTeam);
                if (gb < 11.5)
                    chartedTeams.Add(team);
            }
            lg.setGraphData("Trend Report for "+ league + " Wild Cards", chartedTeams, false);
            lg.save(String.Format("winpct_{0}wildcard.html", league));
        }

        private void buildDraftOrderChart(List<Team> teams)
        {
            LineGraph lg = new LineGraph();
            lg.setGraphData("Trend Report for Draft Order", teams, true);
            lg.save("winpct_draftorder.html");
        }

    }
}
