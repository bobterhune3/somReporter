using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using somReporter.team;

namespace somReporter.output
{
    class ConsoleOutput : IOutput
    {
        public void draftOrderHeader()
        {
            Console.Out.WriteLine("===============");
            Console.Out.WriteLine("= DRAFT ORDER =");
            Console.Out.WriteLine("===============");
            spacer();
        }

        public void draftOrderTableHeader()
        {
            Console.Out.WriteLine(String.Format("{0,-3} {1,-15} {2,-5} {3,-5} {4,-5} {5}",
                                                "#",
                                                "TEAM",
                                                "WINS",
                                                "LOSES",
                                                "WPCT",
                                                "PYTG"));
            Console.Out.WriteLine("==========================================");
        }

        public void draftOrderTeamLine(int pickNum, int n, Team team)
        {
            Console.Out.WriteLine(String.Format("{0,-3} {1,-15} {2,-5} {3,-5} {4,-5} {5,-3} {6,1}",
                                                        pickNum,
                                                        team.Name,
                                                        team.Wins,
                                                        team.Loses,
                                                        team.Wpct,
                                                        team.PythagoreanTheorem,
                                                        showDraftPickRankDif(team, pickNum)));
        }

        public void wildCardHeader(string league)
        {
            spacer();
            Console.Out.WriteLine("===========================");
            Console.Out.WriteLine("= "+ league +" WILD CARD STANDINGS =");
            Console.Out.WriteLine("===========================");
            spacer();
        }

        public void wildCardTableHeader()
        {
            Console.Out.WriteLine(String.Format("{0,-3} {1,-15} {2,-5} {3,-5} {4,-3} {5,-7}",
                                                "#",
                                                "TEAM",
                                                "WINS",
                                                "LOSES",
                                                "GB",
                                                "GBDIF"));
            Console.Out.WriteLine("===========================================");
            spacer();
        }

        public void wildCardTeamLine(int rank, Team team, string gamesBehind)
        {
            Console.Out.WriteLine(String.Format("{0,-3} {1,-15} {2,-5} {3,-5} {4,-3} {5,-1}",
                                                        rank,
                                                        team.Name,
                                                        team.Wins,
                                                        team.Loses,
                                                        gamesBehind,
                                                   //     rank == 1 ? 0 : showGBDif(team),
                                                        showWildCardRankDif(team, rank)));
        }


        public void divisionStandingsHeader(string division)
        {
            Console.Out.WriteLine("==========================");
            Console.Out.WriteLine("= " + division + " STANDINGS");
            Console.Out.WriteLine("==========================");
            spacer();
        }

        public void divisionStandingsTableHeader()
        {
            Console.Out.WriteLine(String.Format("{0,-3} {1,-15} {2,-5} {3,-5} {4,-3}",
                                                "#",
                                                "TEAM",
                                                "WINS",
                                                "LOSES",
                                                "GB"));
            Console.Out.WriteLine("===========================================");
            spacer();
        }

        public void divisionStandingsTeamLine(int rank, Team team)
        {
            Console.Out.WriteLine(String.Format("{0,-3} {1,-15} {2,-5} {3,-5} {4,-3}",
                                                        rank,
                                                        team.Name,
                                                        team.Wins,
                                                        team.Loses,
                                                        team.Gb));
        }

        public void spacer()
        {
            Console.Out.WriteLine(" ");
        }

        private string showWildCardRankDif(Team team, int rank)
        {
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

        private double showGBDif(Team team)
        {
            double diff = team.Gb - team.GbPrevious;
            return diff;
        }

        public void setOutputHeader(string title) { }
        public void setOutputFooter() { }
        public void endOfTable() { }

        public void ShowWhosHotData(string v)
        {
        }

        public void recordBookHeader(bool v)
        {
        }

        public void recordBookItem(SOMRecord rec, int counter, bool teamRecord)
        {
        }

        public void usageHeader(int playerCount)
        {
            throw new NotImplementedException();
        }

        public void usageFooter()
        {
            throw new NotImplementedException();
        }
        public bool usageReportItem(Player player, int counter)
        {
            throw new NotImplementedException();
        }

        public void ShowInjuryData(string v)
        {
            throw new NotImplementedException();
        }
    }
}
