using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Isam.Esent.Collections.Generic;
using somReporter.output;
using somReportUtils;

namespace somReporter.features
{
    abstract class DraftOrderFeatureBase : IFeature
    {
        public abstract Report getReport();
        public abstract void initialize(ISOMReportFile file);
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

        public void setDateStore(PersistentDictionary<string, string> dictionary)
        {
            throw new NotImplementedException();
        }
    }

    class FeatureDraftOrderTierd : DraftOrderFeatureBase
    {


        public override Report getReport()
        {
            throw new NotImplementedException();
        }

        public override void initialize(ISOMReportFile file)
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
            Team[] picks = new Team[24];

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

            actualDraftPicks = new Team[24];

            // Re-organize the picks, Paren is by division draft order
            //    1-4 - Draft (1-4)
            // 5.N4 (5)    12.A5  (12)    =========
            // 6.N3 (6)    13.A4  (13)    19.F6 (19)   
            // ========                   20.F5 (20)
            // 7.A8 (9)    14.A3  (14)    21.F4 (21)
            // 8.N2 (7)    15.F8  (17)    22.F3 (22)
            // 9.A7 (10)   16.A2  (15)    23.F2 (23)
            // 10.N1 (8)   17.F7  (18)    24.F1 (24)
            // 11.A6 (11)  18.A1  (16)
            for (int i = 0; i < picks.Length; i++)
            {
                if (i < 6 )
                {  // 1 to 5 (DET, STL< SD< BOS< TEX, BAL)
                    actualDraftPicks[i] = picks[i];
                    continue;
                }
                else if( i >= 19)
                {
                    actualDraftPicks[i] = picks[i];
                    continue;
                }
                switch (i)
                {
                    // case is +1,   actualDraft is -1
                    case 6: actualDraftPicks[7] = picks[i]; break;   // A8 - MIN
                    case 7: actualDraftPicks[9] = picks[i]; break;  // N2 - CLE
                    case 8: actualDraftPicks[6] = picks[i]; break;   // A7 - HOU
                    case 9: actualDraftPicks[8] = picks[i]; break;   // N1 - TOR
                    case 10: actualDraftPicks[10] = picks[i]; break;  // A6 - WAS
                    case 11: actualDraftPicks[11] = picks[i]; break; // A5 - OAK
                    case 12: actualDraftPicks[12] = picks[i]; break; // A4 = CHI
                    case 13: actualDraftPicks[13] = picks[i]; break; // A3 = TB
                    case 16: actualDraftPicks[14] = picks[i]; break; // F8 = ATL
                    case 14: actualDraftPicks[15] = picks[i]; break; // A2 = SF
                    case 17: actualDraftPicks[16] = picks[i]; break; // F7 = CIN
                    case 15: actualDraftPicks[17] = picks[i]; break;  // A1 =MIL
                    case 18: actualDraftPicks[18] = picks[i]; break;
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

        public override void initialize(ISOMReportFile file)
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
