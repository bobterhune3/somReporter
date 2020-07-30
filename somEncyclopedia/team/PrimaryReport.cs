using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using somReporter;
using somReporter.team;
using somReporter.util.somReporter;
using somReportUtils;


namespace somEncyclopedia.team
{
    public class PrimaryReport : TeamReport
    {
        private List<EncPlayer> listOfPlayers = new List<EncPlayer>();
        public static int TeamIDCounter = 1;
        private string teamName;

        public PrimaryReport(string title, string currentTeam) : base(title) {
            teamName = currentTeam;
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
                    collectData(teamKey, fixedLine, leagueNameLength);
                }
            }
        }

        private const String REGEX_USAGE_HITTER = @".[0-9][0-9][0-9] +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+)";
        private const String REGEX_USAGE_PITCHER = @"^[0-9].[0-9][0-9] +([0-9]+) +([0-9]+) +.[0-9]+ +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+.[0-9]) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+)";
        private bool m_bWorkingOnHitters = true;

        public void collectData(string teamName, string line, int leagueNameLength)
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
                    Regex regex = m_bWorkingOnHitters ? 
                                new Regex(REGEX_USAGE_HITTER) :
                                new Regex(REGEX_USAGE_PITCHER);
                    Match teamMatch = regex.Match(data);
                    if (teamMatch.Success)
                    {
                        if (m_bWorkingOnHitters) {
                            player.YR = Config.LEAGUE_YEAR;
                            player.G = Convert.ToInt32(teamMatch.Groups[1].Value.Trim());
                            player.AB =Convert.ToInt32(teamMatch.Groups[2].Value.Trim());
                            player.R  =   Convert.ToInt32(teamMatch.Groups[3].Value.Trim());
                            player.H  =   Convert.ToInt32(teamMatch.Groups[4].Value.Trim());
                            player.T2B=   Convert.ToInt32(teamMatch.Groups[5].Value.Trim());
                            player.T3B=   Convert.ToInt32(teamMatch.Groups[6].Value.Trim());
                            player.HR =   Convert.ToInt32(teamMatch.Groups[7].Value.Trim());
                            player.RBI=   Convert.ToInt32(teamMatch.Groups[8].Value.Trim());
                            player.BB =   Convert.ToInt32(teamMatch.Groups[9].Value.Trim());
                            player.K =   Convert.ToInt32(teamMatch.Groups[10].Value.Trim());
                            player.HP =   Convert.ToInt32(teamMatch.Groups[11].Value.Trim());
                            player.SH =   Convert.ToInt32(teamMatch.Groups[12].Value.Trim());
                            player.GDP =   Convert.ToInt32(teamMatch.Groups[13].Value.Trim());
                            player.SB =   Convert.ToInt32(teamMatch.Groups[14].Value.Trim());
                            player.CS =   Convert.ToInt32(teamMatch.Groups[15].Value.Trim());
                            player.E  =   Convert.ToInt32(teamMatch.Groups[16].Value.Trim());
                            player.Pos = 9;
                        }
                        else
                        {
                            player.YR = Config.LEAGUE_YEAR;
                            player.W = Convert.ToInt32(teamMatch.Groups[1].Value.Trim());
                            player.L    =Convert.ToInt32(teamMatch.Groups[2].Value.Trim());
                            player.G    =Convert.ToInt32(teamMatch.Groups[3].Value.Trim());
                            player.GS   =Convert.ToInt32(teamMatch.Groups[4].Value.Trim());
                            player.CG   =Convert.ToInt32(teamMatch.Groups[5].Value.Trim());
                            player.SH   =Convert.ToInt32(teamMatch.Groups[6].Value.Trim());
                            player.SV   =Convert.ToInt32(teamMatch.Groups[7].Value.Trim());
                            String ip = teamMatch.Groups[8].Value.Trim();
                            int dot = ip.IndexOf(".");
                            player.IP = Convert.ToInt32(ip.Substring(0, dot));
                            player.Thirds = Convert.ToInt32(ip.Substring(dot+1));
                            player.H    =Convert.ToInt32(teamMatch.Groups[9].Value.Trim());
                            player.R    =Convert.ToInt32(teamMatch.Groups[10].Value.Trim());
                            player.ER   =Convert.ToInt32(teamMatch.Groups[11].Value.Trim());
                            player.HR   =Convert.ToInt32(teamMatch.Groups[12].Value.Trim());
                            player.BB   =Convert.ToInt32(teamMatch.Groups[13].Value.Trim());
                            player.K   =Convert.ToInt32(teamMatch.Groups[14].Value.Trim());
                            player.Pos = 1;
                        }

                        Team team = Report.DATABASE.getTeam(TeamUtils.prettyTeamNoDiceName(teamName));
                        if( team == null) {
                            team = new Team(TeamIDCounter++, TeamUtils.prettyTeamNoDiceName(teamName), leagueNameLength);
                            team.Abrv = TeamUtils.prettyTeamNoDiceName(teamName);
                        }
                        player.Team = team;
                        listOfPlayers.Add(player);
                    }
                }
            }
        }
    }
}
