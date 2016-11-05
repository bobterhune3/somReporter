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

            Console.WriteLine("Process Wild Card...");
            program.processWildCardStandings();

            Console.WriteLine("Process Draft Order...");
            program.processDraftOrder();

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
                            WriteOutTeamForDraftPicks(pickNum, tbTeam);
                        }
                        tieBreakerList.Clear();
                        prevTeam = team;
                    }
                    else {
                        pickNum++;
                        WriteOutTeamForDraftPicks(pickNum, prevTeam);
                        prevTeam = team;
                    }
                }
            }
            pickNum++;
            WriteOutTeamForDraftPicks(pickNum, prevTeam);
            output.endOfTable();
        }

        public void processStandings() {

            leagueStandingsReport.calculateHighLowTeamStats("AL", Team.CATEGORY.BATTING_AVERAGE);
            leagueStandingsReport.calculateHighLowTeamStats("NL", Team.CATEGORY.BATTING_AVERAGE);
            leagueStandingsReport.calculateHighLowTeamStats("AL", Team.CATEGORY.HOME_RUNS);
            leagueStandingsReport.calculateHighLowTeamStats("NL", Team.CATEGORY.HOME_RUNS);
            leagueStandingsReport.calculateHighLowTeamStats("AL", Team.CATEGORY.EARNED_RUNS_AVG);
            leagueStandingsReport.calculateHighLowTeamStats("NL", Team.CATEGORY.EARNED_RUNS_AVG);

            List<Team> teamsALEast = getStandings("AL", "East");
            List<Team> teamsALWest = getStandings("AL", "West");
            List<Team> teamsNLEast = getStandings("NL", "East");
            List<Team> teamsNLWest = getStandings("NL", "West");


            processDivision("AL EAST", teamsALEast);
            processDivision("AL WEST", teamsALWest);
            processDivision("NL EAST", teamsNLEast);
            processDivision("NL WEST", teamsNLWest);
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


        public void processWildCardStandings() {
            output.spacer();

            List<Team> teamsALEast = getStandings("AL", "East");
            List<Team> teamsALWest = getStandings("AL", "West");
            List<Team> teamsNLEast = getStandings("NL", "East");
            List<Team> teamsNLWest = getStandings("NL", "West");

            //Remove Winners of each Divisino
            teamsALEast.RemoveAt(0);
            teamsALWest.RemoveAt(0);
            teamsNLWest.RemoveAt(0);
            teamsNLEast.RemoveAt(0);

            List<Team> teamsAL = new List<Team>();
            List<Team> teamsNL = new List<Team>();
            teamsAL.AddRange(teamsALEast);
            teamsAL.AddRange(teamsALWest);
            teamsNL.AddRange(teamsNLWest);
            teamsNL.AddRange(teamsNLEast);

            sortLeagueByWinningPct(teamsAL);
            sortLeagueByWinningPct(teamsNL);

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
            scope.League = league;
            return leagueStandingsReport.getTeamsByWinPercentage(scope); 
        }

        private void WriteOutTeamForDraftPicks(int pickNum, Team team ) {
            output.draftOrderTeamLine(pickNum, team);
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
        //    if( teamWinPctHistory.addCurrentSeason(teams))
            {
           //     if(!firstTimeLoad)
                { 
                    // Create Division Charts
                    buildDivisionChart("AL", "East");
                    buildDivisionChart("AL", "West");
                    buildDivisionChart("NL", "East");
                    buildDivisionChart("NL", "West");
                }

           //     teamWinPctHistory.csvSaveTeamParser(@"wpct.csv");
            }
        }

        private void buildDivisionChart(String league, String division) {
            List<Team> teams= getStandings(league, division);
            LineGraph lg = new LineGraph();
            lg.setGraphData("Trend Report for "+ league +" " + division, teams);
            lg.save(String.Format("winpct_{0}{1}.html", league, division.ToUpper()));
        }
    }
}
