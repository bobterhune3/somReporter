﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void draftOrderTeamLine(int pickNum, Team team)
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
            Console.Out.WriteLine(String.Format("{0,-3} {1,-15} {2,-5} {3,-5} {4,-3} {5,-4} {6,-1}",
                                                        rank,
                                                        team.Name,
                                                        team.Wins,
                                                        team.Loses,
                                                        gamesBehind,
                                                        rank == 1 ? 0 : showGBDif(team),
                                                        showWildCardRankDif(team, rank)));
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
    }
}