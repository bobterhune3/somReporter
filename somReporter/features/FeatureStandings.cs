using somReporter.output;
using somReporter.reports;
using somReporter.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Isam.Esent.Collections.Generic;

namespace somReporter.features
{
    class FeatureStandings : IFeature
    {
        private LeagueStandingsReport leagueStandingsReport;

        public Report getReport()
        {
            return leagueStandingsReport;
        }

        public void process(IOutput output)
        {
            if (!Config.SHOW_STANDINGS)
                return;

            Console.WriteLine("Process Standings...");
            foreach (string league in Program.LEAGUES)
            {
                if (Config.RANK_STATS_BY_DIVISION)
                {
                    foreach (string division in Program.DIVISIONS)
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

            foreach (string league in Program.LEAGUES)
            {
                foreach (string division in Program.DIVISIONS)
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

            foreach (string league in Program.LEAGUES)
            {
                foreach (string division in Program.DIVISIONS)
                {
                    List<Team> div = teams[league][division];
                    processDivision(output, league + division, div);
                }
            }

            if (Config.HAS_WILDCARD)
            {
                Console.WriteLine("Process Wild Card...");
                processWildCardStandings(output);
            }

        }


        public void initialize(SOMReportFile leagueReportFile) { 
            Console.WriteLine("    Building Standings...");
            leagueStandingsReport = (LeagueStandingsReport)leagueReportFile.FindReport("LEAGUE STANDINGS FOR");
            leagueStandingsReport.processReport();
        }


        public void buildCharts()
        {
            if (!Config.SHOW_STANDINGS)
                return;

            TeamWinPctHistory teamWinPctHistory = new TeamWinPctHistory();
            bool firstTimeLoad = !teamWinPctHistory.loadWinPctFile(@"wpct.csv");

            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.OrderAscending = true;
            scope.AllTeams = true;

            // Only create and save if additional games added.
            List<Team> teams = ((LeagueStandingsReport)getReport()).getTeamsByWinPercentage(scope);
            if (teamWinPctHistory.addCurrentSeason(teams))
            {
                if (!firstTimeLoad)
                {
                    // Create Division Charts
                    foreach (string league in Program.LEAGUES)
                    {
                        foreach (string division in Program.DIVISIONS)
                            buildDivisionChart(league, division);
                    }
                }

                teamWinPctHistory.csvSaveTeamParser(@"wpct.csv");
            }

            ((DraftOrderFeatureBase)Program.featureDraftOrder).buildDraftOrderChart();


            if (Config.HAS_WILDCARD)
            {
                List<Team> teamsAL = getWildCardTeams("AL");
                buildWildcardChart(teamsAL, "AL");

                List<Team> teamsNL = getWildCardTeams("NL");
                buildWildcardChart(teamsNL, "NL");
            }
        }


        private void buildDivisionChart(String league, String division)
        {
            List<Team> teams = getStandings(league, division);
            LineGraph lg = new LineGraph();
            lg.setGraphData("Trend Report for " + league + " " + division, teams, false);
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
            lg.setGraphData("Trend Report for " + league + " Wild Cards", chartedTeams, false);
            lg.save(String.Format("winpct_{0}wildcard.html", league));
        }

        private void processDivision(IOutput output, string division, List<Team> teams)
        {
            output.divisionStandingsHeader(division);
            output.divisionStandingsTableHeader();

            int rank = 1;
            foreach (Team team in teams)
            {
                int nextTeamLosses = -1;
                team.DivisionPositionCurrent = rank;
                if( rank == 1 )
                {
                    team.SecondPlaceTeamLosses = teams[1].Loses;
                }
                output.divisionStandingsTeamLine(rank++, team);
            }
            output.endOfTable();
        }

        private List<Team> getStandings(String league, String division)
        {
            LeagueStandingsReport.ReportScope scope = new LeagueStandingsReport.ReportScope();
            scope.OrderAscending = false;
            scope.Division = division;
            scope.League = Program.LEAGUES[0].Length == 0 ? "X" : league;
            return leagueStandingsReport.getTeamsByWinPercentage(scope);
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


        private void sortLeagueByWinningPct(List<Team> teams)
        {
            teams.Sort(delegate (Team x, Team y)
            {
                return y.Wpct.CompareTo(x.Wpct);
            });
        }

        private void writeOutLeagueWildcards(IOutput output, List<Team> teams)
        {
            Team secondPlaceTeam = teams.ElementAt(1);
            WriteOutLeadingTeamForWildCard(output, teams.ElementAt(0), secondPlaceTeam);
            WriteOutTeamForWildCard(output, 2, secondPlaceTeam, 999);

            for (int i = 2; i < teams.Count; i++)
            {
                Team team = teams.ElementAt(i);
                double gb = team.calculateGamesBehind(secondPlaceTeam);
                if (gb < 11.5)
                    WriteOutTeamForWildCard(output, i + 1, team, gb * -1.0);
            }
        }


        private void WriteOutLeadingTeamForWildCard(IOutput output, Team team, Team secondPlaceTeam)
        {
            double gb = secondPlaceTeam.calculateGamesBehind(team);
            WriteOutTeamForWildCard(output, 1, team, gb);
        }

        private void WriteOutTeamForWildCard(IOutput output, int rank, Team team, double wcGamesBehind)
        {
            team.WildCardPositionCurrent = rank;

            String gamesBehind = "---";
            if (wcGamesBehind != 999)
            {
                gamesBehind = String.Format("{0}", wcGamesBehind);
            }

            output.wildCardTeamLine(rank, team, gamesBehind);
        }

        public void processWildCardStandings(IOutput output)
        {
            output.spacer();

            List<Team> teamsAL = getWildCardTeams("AL");
            List<Team> teamsNL = getWildCardTeams("NL");

            output.wildCardHeader("AL");
            output.wildCardTableHeader();

            writeOutLeagueWildcards(output, teamsAL);
            output.endOfTable();

            output.spacer();

            output.wildCardHeader("NL");
            output.wildCardTableHeader();

            writeOutLeagueWildcards(output, teamsNL);
            output.endOfTable();
        }

        public void setDateStore(PersistentDictionary<string, string> dictionary)
        {
            throw new NotImplementedException();
        }
    }

}
