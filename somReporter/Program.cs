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
                            WriteOutTeam(pickNum, tbTeam);
                        }
                        tieBreakerList.Clear();
                        prevTeam = team;
                    }
                    else {
                        pickNum++;
                        WriteOutTeam(pickNum, prevTeam);
                        prevTeam = team;
                    }

                }

            }
            pickNum++;
            WriteOutTeam(pickNum, prevTeam);
        }

        private void WriteOutTeam(int pickNum, Team team ) { 
            Console.Out.WriteLine(String.Format("{0,-3} {1,-15} {2,-5} {3,-5} {4,-5} {5}",
                                                        pickNum,
                                                        team.Name,
                                                        team.Wins,
                                                        team.Loses,
                                                        team.Wpct,
                                                        team.PythagoreanTheorem));
         }
    }
}
