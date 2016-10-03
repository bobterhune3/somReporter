using Microsoft.Isam.Esent.Collections.Generic;
using somReporter.output;
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
            program.initialize();
       
            string fileName = program.lookupPreviousSaveFile();
            if( fileName.Length > 0 ) {
                PersistentDictionary<string, string> prevDictionaryFile = 
                    new PersistentDictionary<string, string>(fileName);
                program.loadPreviousStorageInfo(prevDictionaryFile);
            }

            program.processWildCardStandings();

            program.processDraftOrder();

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

            output.setOutputHeader(file.SeasonTitle);
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

        public void processWildCardStandings() {
            output.spacer();

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
    }
}
