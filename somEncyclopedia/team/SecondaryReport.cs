using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using somReporter;
using somReporter.team;
using somReportUtils;


namespace somEncyclopedia.team
{
    public class SecondaryReport : TeamReport
    {
        private List<EncPlayer> listOfPlayers = new List<EncPlayer>();
        private string teamName = "";

        public SecondaryReport(string title, string teamName) : base(title) {
            teamName = teamName;
        }
        
        public List<EncPlayer> getPlayers() {
            return listOfPlayers;
        }

        public override void processReport(int leagueNameLength)
        {
            foreach( String teamKey in this.m_Teamlines.Keys ) {
                List<String> teamLines = m_Teamlines[teamKey];
                foreach (String line in teamLines)
                {
                    string fixedLine = line.Replace("[0]", "");
                    collectData(fixedLine, leagueNameLength);
                }
            }
        }

        private const String REGEX_USAGE_HITTER = @"[0-9]+ +[0-9]+ +[0-9]+ +[0-9]+ +([0-9]+) +([0-9]+) +[0-9]+ +([0-9]+) +[1.0-]+ +[0-9]+ +([0-9]+)-([ 0-9]+) +([0-9.-]+[0-9]+|[-]+) +([0-9]+) +([0-9]+) +([0-9]+)";
        private const String REGEX_USAGE_PITCHER = @"[0-9]?[0-9]?.[0-9]+[ ]+[0-9]?[0-9]?.[0-9]+[ ]+[0-9]+[ ]+[0-9]+[ ]+[0-9]+[ ]+[0-9]+[ ]+[0-9]+[ ]+[0-9]+[ ]+([0-9]+)+[ ]+([0-9]+).+";
        private bool m_bWorkingOnHitters = true;

        public void collectData(string line, int leagueNameLength)
        {
            if (line.Contains("BAVG"))
                m_bWorkingOnHitters = true;
            else if (line.Contains("ERA"))
                m_bWorkingOnHitters = false;
            else {
                int idx = line.IndexOf("[4]");
                if (idx == -1)
                    return;
                String playerName = line.Substring(0, idx).Trim();
                if(playerName.Length > 0 )
                {
                    EncPlayer player = new EncPlayer();
                    player.IsHitter = m_bWorkingOnHitters;
                    int idxDot = playerName.IndexOf('.');
                    if (idxDot == -1)
                        return;
                    player.Name = playerName.Substring(idxDot+1);
                    player.FName = playerName.Substring(0, idxDot);
                    String data = line.Substring(idx + 4).Trim();
                    Regex  regex = m_bWorkingOnHitters ? 
                                        new Regex(REGEX_USAGE_HITTER) :
                                        new Regex(REGEX_USAGE_PITCHER);

                    Match teamMatch = regex.Match(line);
                    if (teamMatch.Success)
                    {
                        if (m_bWorkingOnHitters) { 
                            player.IBB = Convert.ToInt32(teamMatch.Groups[1].Value.Trim());
                            player.SF  = Convert.ToInt32(teamMatch.Groups[2].Value.Trim());
                            player.SAt = Convert.ToInt32(teamMatch.Groups[3].Value.Trim());

                            player.Stk = Convert.ToInt32(teamMatch.Groups[4].Value.Trim());
                            player.LStk = Convert.ToInt32(teamMatch.Groups[5].Value.Trim());


                            player.PAB = Convert.ToInt32(teamMatch.Groups[7].Value.Trim());
                            player.PH  = Convert.ToInt32(teamMatch.Groups[8].Value.Trim());
                            player.PHR = Convert.ToInt32(teamMatch.Groups[9].Value.Trim());
                            player.CI = 0;
                        }
                        else
                        {

                        }

                        Team team = PrimaryReport.DATABASE.getTeam(TeamUtils.prettyTeamNoDiceName(teamName));
                        if( team == null) {
                            team = new Team(PrimaryReport.TeamIDCounter++, "teamName", leagueNameLength);
                            team.Abrv = TeamUtils.prettyTeamNoDiceName(teamName);
                        }
                        player.Team = team;
                    }
                    listOfPlayers.Add(player);
                }
            }
        }
    }
}
