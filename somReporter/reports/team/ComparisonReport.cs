using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace somReporter.team
{
    public class ComparisonReport : TeamReport
    {
        private List<Player> listOfPlayers = new List<Player>();

        public ComparisonReport(string title) : base(title) {
        }
        
        public List<Player> getPlayers() {
            return listOfPlayers;
        }

        public override void processReport()
        {
            foreach( String teamKey in this.m_Teamlines.Keys ) {
                List<String> teamLines = m_Teamlines[teamKey];
                foreach (String line in teamLines)
                {
                    string fixedLine = line.Replace("[0]", "");
                    collectData(teamKey, fixedLine);
                }
            }
        }

        private const String REGEX_USAGE_HITTER = @".[0-9]+ +.[0-9]+ +([0-9]+) +([0-9]+) +[\S]+";
        // private const String REGEX_USAGE_PITCHER = @"[ .0-9].[0-9]+ +[ 0-9].[0-9]+ +[0-9]+ +[0-9]+ +[0-9]+ +[0-9]+ +[0-9]+ +[0-9]+ +([0-9]+) +([0-9]+) +[\S]+";
        private const String REGEX_USAGE_PITCHER = @"[0-9]?[0-9]?.[0-9]+[ ]+[0-9]?[0-9]?.[0-9]+[ ]+[0-9]+[ ]+[0-9]+[ ]+[0-9]+[ ]+[0-9]+[ ]+[0-9]+[ ]+[0-9]+[ ]+([0-9]+)+[ ]+([0-9]+).+";
        private const String REGEX_USAGE_PITCHER_NOT_USED = @"[.0-9].[0-9]+ +--- +[0-9]+ +[0-9]+ +[0-9]+ +[0-9]+ +[0-9]+ +[0-9]+ +([0-9]+) +([0-9]+) +[\S]+";
        private bool m_bWorkingOnHitters = true;

        public void collectData(string teamName, string line)
        {
            if (line.StartsWith("--AVERAGE"))
                m_bWorkingOnHitters = true;
            else if (line.StartsWith("----ERA"))
                m_bWorkingOnHitters = false;
            else if (line.StartsWith("NAME") || line.Trim().Length == 0)
                return;
            else {
                int idx = line.IndexOf("[4]");
                String playerName = line.Substring(0, idx).Trim();
                if(playerName.Length > 0 )
                {
                    Player player = new Player();
                    player.IsHitter = m_bWorkingOnHitters;
                    player.Name = playerName;
                    String data = line.Substring(idx + 4).Trim();
                    Regex regex;
                    if ( data.Contains("---") && !m_bWorkingOnHitters) {
                        regex = new Regex(REGEX_USAGE_PITCHER_NOT_USED);
                    }
                    else {
                         regex = m_bWorkingOnHitters ? 
                                        new Regex(REGEX_USAGE_HITTER) :
                                        new Regex(REGEX_USAGE_PITCHER);
                    }
                    Match teamMatch = regex.Match(line);
                    if (teamMatch.Success)
                    {
                        /*
                            Hitters < 120 actual at bats are allowed at 150%      (ie ab * 1.5)
                            Hitters > 120 actual at bats are actual at bats + 60  (ie ab + 60)
                            Hitters > 600 actual at bats are allowed at 110%      (ie ab * 1.1)

                            Pitchers < 60 is 150% of actual   (59 * 1.5) = +29
                            Pitchers > 118 innings is innings + 30  
                            Pitchers > 199 is 115% of actual (199 * 1.15) = +29
                          */

                        if (m_bWorkingOnHitters) { 
                            player.Actual = Convert.ToInt32(teamMatch.Groups[1].Value.Trim());
                            if (player.Actual < 25)
                                return;
                            player.Replay = Convert.ToInt32(teamMatch.Groups[2].Value.Trim());

                            player.TargetUsage = getPlayerTargetUsage(player.Actual);

                        }
                        else
                        {
                            player.Actual = Convert.ToInt32(teamMatch.Groups[1].Value.Trim());
                            if (player.Actual < 25)
                                return;
                            player.Replay = Convert.ToInt32(teamMatch.Groups[2].Value.Trim());

                            player.TargetUsage = getPitcherTargetUsage(player.Actual);
                        }

                        Team team = Report.DATABASE.getTeam(Team.prettyTeamName(teamName));
                        if( team == null) {
                            team = new Team("XX");
                            team.Abrv = teamName;
                        }
                        player.Team = team;
                    }
                    listOfPlayers.Add(player);
                }
            }
        }

        /*
            Hitters < 120 actual at bats are allowed at 150%      (ie ab * 1.5)
            Hitters > 120 actual at bats are actual at bats + 60  (ie ab + 60)
            Hitters > 600 actual at bats are allowed at 110%      (ie ab * 1.1)
          */
        public static int getPlayerTargetUsage(int actual)
        {
            if (actual < 120)
            {
                return (int)Math.Round(((float)actual) * 1.5f);
            }
            else if (actual > 600)
            {
                return (int)Math.Round(((float)actual) * 1.1f);
            }
            else
            {
                return actual + 60;
            }
        }

        /*
            Pitchers < 60 is 150% of actual   (59 * 1.5) = +29
            Pitchers > 118 innings is innings + 30  
            Pitchers > 199 is 115% of actual (199 * 1.15) = +29
          */
        public static int getPitcherTargetUsage(int actual)
        {
            if (actual < 60)
            {
               return (int)Math.Round(((float)actual) * 1.5f);
            }
            else if (actual > 199)
            {
                return (int)Math.Round(((float)actual) * 1.15f);
            }
            else
            {
                return actual + 30;
            }
        }
    }
}
