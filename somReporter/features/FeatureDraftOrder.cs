using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using somReporter.output;

namespace somReporter.features
{
    abstract class DraftOrderFeatureBase : IFeature
    {
        public abstract Report getReport();
        public abstract void initialize(SOMReportFile file);
        public abstract void process(IOutput output);
        public Team[] actualDraftPicks;

        protected void WriteOutTeamForDraftPicks(IOutput output, int pickNum, int divPick, Team team)
        {
            output.draftOrderTeamLine(pickNum, divPick, team);
            team.DraftPickPositionCurrent = pickNum;
        }


        public void buildDraftOrderChart()
        {
            LineGraph lg = new LineGraph();
            List<Team> teams = actualDraftPicks.OfType<Team>().ToList(); // this isn't going to be fast.
            lg.setGraphData("Trend Report for Draft Order", teams, true);
            lg.save("winpct_draftorder.html");
        }
    }

    class FeatureDraftOrderTierd : DraftOrderFeatureBase
    {


        public override Report getReport()
        {
            throw new NotImplementedException();
        }

        public override void initialize(SOMReportFile file)
        {
            throw new NotImplementedException();
        }

        public override void process(IOutput output)
        {
            output.draftOrderHeader();

            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.OrderAscending = true;
            scope.League = "X";

            // Get Team Draft order by division
            Dictionary<String, List<Team>> draftOrder = new Dictionary<string, List<Team>>();
            foreach (string division in Program.DIVISIONS)
            {
                scope.Division = division;
                draftOrder[division] = ((LeagueStandingsReport)Program.featureStandings.getReport()).getTeamsByWinPercentage(scope);
            }

            output.draftOrderTableHeader();

            int pickNum = 0;
            List<Team> tieBreakerList = new List<Team>();
            Team prevTeam = null;
            Team lastTeam = null;
            Team[] picks = new Team[18];

            foreach (String division in Program.DIVISION_DRAFT_ORDER)
            {
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

            actualDraftPicks = new Team[18];

            // Re-organize the picks, Paren is by division draft order
            //    1-4 - Draft (1-4)
            //    5.A6  (6)    12.A2 (14)
            //    6.N2  (8)    13.F5 (11)
            //    7.A5  (5)    14.A1 (13)
            //    8.N1  (7)    15.F4 (15)
            //    9.A4  (9)    16.F3 (16)
            //    10.A3 (10)   17.F2 (17)
            //    11.F6 (12)   18.F1 (18)
            for (int i = 0; i < picks.Length; i++)
            {
                if (i < 4 || i > 13)
                {
                    actualDraftPicks[i] = picks[i];
                    continue;
                }
                switch (i)
                {
                    case 4: actualDraftPicks[5] = picks[i]; break;
                    case 5: actualDraftPicks[7] = picks[i]; break;
                    case 6: actualDraftPicks[4] = picks[i]; break;
                    case 7: actualDraftPicks[6] = picks[i]; break;
                    case 8: actualDraftPicks[8] = picks[i]; break;
                    case 9: actualDraftPicks[9] = picks[i]; break;
                    case 10: actualDraftPicks[11] = picks[i]; break;
                    case 11: actualDraftPicks[13] = picks[i]; break;
                    case 12: actualDraftPicks[10] = picks[i]; break;
                    case 13: actualDraftPicks[12] = picks[i]; break;
                }
            }

            for (int i = 0; i < actualDraftPicks.Length; i++)
            {
                WriteOutTeamForDraftPicks(output, i, 1, actualDraftPicks[i]);
            }
            output.endOfTable();
        }
    }

    class FeatureDraftOrderStraight : DraftOrderFeatureBase
    {
        public override Report getReport()
        {
            throw new NotImplementedException();
        }

        public override void initialize(SOMReportFile file)
        {
            throw new NotImplementedException();
        }

        public override void process(IOutput output)
        {
            output.draftOrderHeader();

            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.OrderAscending = true;
            scope.AllTeams = true;

            output.draftOrderTableHeader();


            List<Team> teams = ((LeagueStandingsReport)Program.featureStandings.getReport()).
                                    getTeamsByWinPercentage(scope);
            int pickNum = 0;
            List<Team> tieBreakerList = new List<Team>();
            Team prevTeam = null;

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
                            WriteOutTeamForDraftPicks(output, pickNum, 0, tbTeam);
                        }
                        tieBreakerList.Clear();
                        prevTeam = team;
                    }
                    else
                    {
                        pickNum++;
                        WriteOutTeamForDraftPicks(output, pickNum, 0, prevTeam);
                        prevTeam = team;
                    }
                }
            }
            pickNum++;
            WriteOutTeamForDraftPicks(output, pickNum, 0, prevTeam);
            output.endOfTable();
       }
    }
}
