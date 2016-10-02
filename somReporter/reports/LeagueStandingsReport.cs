using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace somReporter
{
    public class LeagueStandingsReport : Report
    {
        //        private static String REGEX_HEADER      = @"([\s\S]{0,15}) +([\S]+) +([\S]+) +([\S]+) +([\S]+) +([\s\S]+) ([\S]+) +([\S]+) ([\S]+) ([\S]+)";
        private const String REGEX_HEADER = @"([A-Z][A-Z] [a-zA-Z]+]{0,15}) +";
        private const String REGEX_TEAM_RECORD = @"[0-9]+ (.+?(?=[4]))4] +([0-9]+) +(.[0-9]+) +([(|1).0-9]+) +([\S]+)";

        private const String REPORT_TYPE = "LEAGUE STANDINGS";

        private String m_CurrentDivision = "";

        public enum REPORT_SCOPE { ALL, LEAGUE, DIVISION }

        public LeagueStandingsReport(String title) : base(title)
        { }

        public override String getReportType() { return REPORT_TYPE; }

        public override void processReport() {
            String currentLeague = "";
            foreach( String line in m_lines)
            {
                collectData(line);
            }
        }

        public void collectData(string line)
        {
            Regex regex = new Regex(REGEX_HEADER);
            Match headerMatch = regex.Match(line);
            if (headerMatch.Success)
            {
                m_CurrentDivision = headerMatch.Groups[1].Value.Trim();
            }
            else {
                regex = new Regex(REGEX_TEAM_RECORD);
                Match teamMatch = regex.Match(line);
                if (teamMatch.Success)
                {
                    String name = teamMatch.Groups[1].Value.Trim();
                    name = name.Substring(0, name.Length - 1).Trim();
                    if( name.Length > 7 )
                    {
                        String abv = name.Substring(name.Length - 3).Trim();
                        if( abv.Length == 3)
                        {
                            name = name.Substring(0, name.Length - 3);

                            Team team = new Team(m_CurrentDivision);
                            team.Name = name.Trim();
                            team.Abrv = abv;
                            team.Wins = Convert.ToInt32(teamMatch.Groups[2].Value.Trim());
                            team.Loses = Convert.ToInt32(teamMatch.Groups[3].Value.Trim());
                    //        team.Wpct = Convert.ToDouble(teamMatch.Groups[4].Value.Trim());
                            String gamesBehind = teamMatch.Groups[5].Value.Trim();
                            if(gamesBehind.StartsWith("-"))
                                team.Gb = 0;
                            else
                                team.Gb = Convert.ToDouble(gamesBehind);
                            DATABASE.addTeam(team);
                        }
                    }
                }
            }

        }

        private List<Team> getTeamsByScope(ReportScope scope )
        {
            List<Team> matches = new List<Team>();
            foreach (Team team in DATABASE.Teams())
            {
                if (scope.AllTeams)
                    matches.Add(team);
                if (scope.Division.Length > 0 && scope.League.Length > 0)

                {
                    if (team.Division.Equals(scope.Division) &&
                        team.League.Equals(scope.League))
                    {
                        matches.Add(team);
                    }
                }
                else if (scope.Division.Length > 0 &&
                   team.Division.Equals(scope.Division))
                {
                    matches.Add(team);
                }
                else if (scope.League.Length > 0 &&
                    team.League.Equals(scope.League))
                {
                    matches.Add(team);
                }
            }
            return matches;
        }

        public List<Team> getTeamsByWinPercentage(ReportScope scope)
        {
            List<Team> matches = getTeamsByScope(scope);

            matches.Sort(delegate (Team x, Team y)
            {
                if (scope.OrderAscending) {
                    int result = x.Wpct.CompareTo(y.Wpct);
                    if (result == 0)
                        return x.PythagoreanTheorem.CompareTo(y.PythagoreanTheorem);
                    else return result;
                }
                else {
                    int result = y.Wpct.CompareTo(x.Wpct);
                    if (result == 0)
                        return y.PythagoreanTheorem.CompareTo(x.PythagoreanTheorem);
                    else return result;
                }

            });

            return matches;
        }
/*
        matches.Sort(delegate (Team x, Team y)
            {

            });
*/
        public class ReportScope
        {
            public String Division = "";
            public String League = "";
            public Boolean AllTeams = false;
            public Boolean OrderAscending = false;

        }

        public List<Team> getTeamsByName(ReportScope scope)
        {
            List<Team> matches = getTeamsByScope(scope);

            matches.Sort(delegate (Team x, Team y)
            {
                if (scope.OrderAscending)
                {
                    if (x.Name == null && y.Name == null) return 0;
                    else if (x.Name == null) return -1;
                    else if (y.Name == null) return 1;
                    else return x.Name.ToUpper().CompareTo(y.Name.ToUpper());
                }
                else {
                    if (x.Name == null && y.Name == null) return 0;
                    else if (x.Name == null) return -1;
                    else if (y.Name == null) return 1;
                    else return y.Name.ToUpper().CompareTo(x.Name.ToUpper());
                }

            });

            return matches;
        }

    }
}
