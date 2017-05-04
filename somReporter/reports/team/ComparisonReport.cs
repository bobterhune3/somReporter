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
        private const String REGEX_USAGE_PITCHER = @"[0-9]+.[0-9]+[ ]+[0-9]+.[0-9]+[ ]+[0-9]+[ ]+[0-9]+[ ]+[0-9]+[ ]+[0-9]+[ ]+[0-9]+[ ]+[0-9]+[ ]+([0-9]+)+[ ]+([0-9]+).+";
        private const String REGEX_USAGE_PITCHER_NOT_USED = @"[.0-9].[0-9]+ +--- +[0-9]+ +[0-9]+ +[0-9]+ +[0-9]+ +[0-9]+ +[0-9]+ +([0-9]+) +([0-9]+) +[\S]+";
        private bool m_bWorkingOnHitters = true;

        public void collectData(string team, string line)
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
                            Hitters <= 101 actual at bats are allowed at 150%    (ie ab * 1.5)
                            Hitters > 101 actual at bats are actual at bats + 50  (ie ab + 50)

                            Pitchers >= 100 innings is innings + 30  
                            Pitchers < 100 is 110% of actual   (99 * 1.1) = +29
                          */

                        if (m_bWorkingOnHitters) { 
                            player.Actual = Convert.ToInt32(teamMatch.Groups[1].Value.Trim());
                            if (player.Actual < 25)
                                return;
                            player.Replay = Convert.ToInt32(teamMatch.Groups[2].Value.Trim());
                            if(player.Actual >=  500) {
                                player.TargetUsage = Convert.ToInt32(((float)player.Actual) * 1.1f);
                            }
                            else if( player.Actual > 100 )  {
                                player.TargetUsage = Convert.ToInt32(((float)player.Actual) * 1.5f);
                            }
                            else {
                                player.TargetUsage = player.Actual + 50;
                            }
                         
                        }
                        else
                        {
                            player.Actual = Convert.ToInt32(teamMatch.Groups[1].Value.Trim());
                            if (player.Actual < 25)
                                return;
                            player.Replay = Convert.ToInt32(teamMatch.Groups[2].Value.Trim());
                            if (player.Actual <= 60)
                            {
                                player.TargetUsage = Convert.ToInt32(player.Actual * 1.5);
                            }
                            else
                            {
                                player.TargetUsage   = player.Actual + 30;
                            }
                        }
                        player.Team = team;
                    }
                    listOfPlayers.Add(player);
                }
            }
        }
    }
}
