using Microsoft.Isam.Esent.Collections.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace somReporter
{
    public class Program
    {
        private LeagueStandingsReport leagueStandingsReport;
        private LeagueGrandTotalsReport leaguePrimaryStatReport;
        private SOMReportFile file;


        static void Main(string[] args)
        {
            
            Program program = new Program();
            program.initialize();

            string fileName = program.lookupPreviousSaveFile();
            if( fileName.Length > 0 ) {
                PersistentDictionary<string, string> prevDictionaryFile = 
                    new PersistentDictionary<string, string>(fileName);
                program.loadPreviousStorageInfo(prevDictionaryFile);
            }

            program.processDraftOrder();

            program.processWildCardStandings();


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
        }

        public String lookupPreviousSaveFile()
        {
            string[] directories = Directory.GetDirectories(Directory.GetCurrentDirectory(), file.SeasonTitle+"*");
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

            string fileName = String.Format("{0}-{1}", file.SeasonTitle, highestValue);
            return fileName;

        }

        public void loadPreviousStorageInfo(PersistentDictionary<string, string>  prevDictionaryFile)
        {
            this.leaguePrimaryStatReport.loadPreviousStorageInfo(prevDictionaryFile);
        }

        public void initialize() {
            Report.DATABASE.reset();
            file = new SOMReportFile("ALL_REPORTS.PRT");
            file.parseFile();
            leagueStandingsReport = (LeagueStandingsReport)file.FindReport("LEAGUE STANDINGS FOR");
            leagueStandingsReport.processReport();

            leaguePrimaryStatReport = (LeagueGrandTotalsReport)file.FindReport("LEAGUE GRAND TOTALS (primary report) FOR");
            leaguePrimaryStatReport.processReport();
        }

        public void saveReportInformation()
        {
            Report.saveReportInformation(buildReportDBName());
        }


        private String buildReportDBName()
        {
            return String.Format("{0}-{1}", file.SeasonTitle, Team.TOTAL_GAMES);

        }

        public void processDraftOrder()
        {
            Console.Out.WriteLine("===============");
            Console.Out.WriteLine("= DRAFT ORDER =");
            Console.Out.WriteLine("===============");

            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.OrderAscending = true;
            scope.AllTeams = true;


            List<Team> teams = leagueStandingsReport.getTeamsByWinPercentage(scope);

            Console.Out.WriteLine(String.Format("{0,-3} {1,-15} {2,-5} {3,-5} {4,-5} {5}",
                                                "#",
                                                "TEAM",
                                                "WINS",
                                                "LOSES",
                                                "WPCT",
                                                "PYTG"));
            Console.Out.WriteLine("==========================================");

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
        }

        public void processWildCardStandings() {
            List<Team> teamsALEast = getStandings("AL", "East");
            List<Team> teamsALWest = getStandings("AL", "West");
            List<Team> teamsNLEast = getStandings("NE", "East");
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

            Console.Out.WriteLine(" ");

            Console.Out.WriteLine(String.Format("{0,-3} {1,-15} {2,-5} {3,-5} {4,-3} {5,-7}",
                                                "#",
                                                "TEAM",
                                                "WINS",
                                                "LOSES",
                                                "GB",
                                                "GBDIF"));
            Console.Out.WriteLine("===========================================");

            writeOutLeagueWildcards("AL Wildcard", teamsAL);

            Console.Out.WriteLine(" ");

            Console.Out.WriteLine(String.Format("{0,-3} {1,-15} {2,-5} {3,-5} {4,-3} {5,-7}",
                                    "#",
                                    "TEAM",
                                    "WINS",
                                    "LOSES",
                                    "GB",
                                    "GBDIF"));
            Console.Out.WriteLine("===========================================");

            writeOutLeagueWildcards("NL Wildcard", teamsNL);

        }

        private void writeOutLeagueWildcards( String title, List<Team> teams ) {
            int pickNum = 1;
            Console.Out.WriteLine(title);


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
            Console.Out.WriteLine(String.Format("{0,-3} {1,-15} {2,-5} {3,-5} {4,-5} {5,-3} {6,1}",
                                                        pickNum,
                                                        team.Name,
                                                        team.Wins,
                                                        team.Loses,
                                                        team.Wpct,
                                                        team.PythagoreanTheorem,
                                                        showDraftPickRankDif(team, pickNum)));
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

            Console.Out.WriteLine(String.Format("{0,-3} {1,-15} {2,-5} {3,-5} {4,-3} {5,-4} {6,-1}",
                                                        rank,
                                                        team.Name,
                                                        team.Wins,
                                                        team.Loses,
                                                        gamesBehind,
                                                        rank == 1 ? 0 : showGBDif(team),
                                                        showWildCardRankDif(team, rank)));                                                   
        }

        private double showGBDif(Team team) {
            double diff = team.Gb - team.GbPrevious;
            return diff;
        }
        private string showWildCardRankDif(Team team, int rank) {
            if (team.WildCardPositionPrevious > rank)
                return "-";
            else if (team.WildCardPositionPrevious < rank)
                return "+";
            else return "=";
        }
        private string showDraftPickRankDif(Team team, int rank)
        {
            if (team.DraftPickPositionPrevious > rank)
                return "-";
            else if (team.DraftPickPositionPrevious < rank)
                return "+";
            else return "=";
        }
    }
}
