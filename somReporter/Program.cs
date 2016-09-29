using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace somReporter
{
    public class Program
    {
        static void Main(string[] args)
        {
            
            Program program = new Program();
            program.initialize();
            program.processDraftOrder();

            program.processWildCardStandings();

            Console.WriteLine("Press ESC to stop");
            do
            {
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }

        LeagueStandingsReport leagueStandingsReport;
        LeagueGrandTotalsReport leaguePrimaryStatReport;

        public void initialize() {
            Report.DATABASE.reset();
            SOMReportFile file = new SOMReportFile("ALL_REPORTS.PRT");
            file.parseFile();
            leagueStandingsReport = (LeagueStandingsReport)file.FindReport("LEAGUE STANDINGS FOR");
            leagueStandingsReport.processReport();

            leaguePrimaryStatReport = (LeagueGrandTotalsReport)file.FindReport("LEAGUE GRAND TOTALS (primary report) FOR");
            leaguePrimaryStatReport.processReport();
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

            writeOutLeagueWildcards("AL Wildcard", teamsAL);
            writeOutLeagueWildcards("NL Wildcard", teamsNL);
        }

        private void writeOutLeagueWildcards( String title, List<Team> teams ) {
            int pickNum = 1;
            Console.Out.WriteLine(title);

            WriteOutTeamForWildCard(1, teams.ElementAt(0), 999);
            Team secondPlaceTeam = teams.ElementAt(1);
            WriteOutTeamForWildCard(2, secondPlaceTeam, 999);

            for( int i=2; i<teams.Count; i++)
            {
                Team team = teams.ElementAt(i);
                double gb = team.calculateGamesBehind(secondPlaceTeam);
                if( gb < 11.5 )
                   WriteOutTeamForWildCard(i+1, team, gb);
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
            Console.Out.WriteLine(String.Format("{0,-3} {1,-15} {2,-5} {3,-5} {4,-5} {5}",
                                                        pickNum,
                                                        team.Name,
                                                        team.Wins,
                                                        team.Loses,
                                                        team.Wpct,
                                                        team.PythagoreanTheorem));
         }

        private void WriteOutTeamForWildCard(int rank, Team team, double wcGamesBehind)
        {
            String gamesBehind = "---";
            if (wcGamesBehind != 999) {
                gamesBehind = String.Format("{0}",wcGamesBehind);
            }
            
            Console.Out.WriteLine(String.Format("{0,-3} {1,-15} {2,-5} {3,-5} {4,-3}",
                                                        rank,
                                                        team.Name,
                                                        team.Wins,
                                                        team.Loses,
                                                        gamesBehind));
        }
    }
}
