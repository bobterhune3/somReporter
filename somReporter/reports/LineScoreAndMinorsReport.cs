using somReportUtils;
using System;
using System.Text.RegularExpressions;

namespace somReporter
{
    public class LineScoreAndMinorsReport : Report
    {
        private const String REGEX_LINE_SCOREA = @".+?(?=[4])4]([A-Z][A-Z][A-Z]) +(.[0-9]+).+?(?=[0])0]([A-Z][A-Z][A-Z]) +(.[0-9]+)";
        private const String REGEX_LINE_SCOREB = @".+?(?=[0])0]([A-Z][A-Z][A-Z]) +(.[0-9]+).+?(?=[4])4]([A-Z][A-Z][A-Z]) +(.[0-9]+)";
        private string[] IN_MINORS = new String[] { "(M)" };

        public LineScoreAndMinorsReport(string title) : base(title) {
        }

        public override void processReport(int n) {
            bool inFirstSection = true;
            Team currentTeam = null;
            foreach (String line in m_lines)
            {
                if (line.Length > 0) {
                    if(!inFirstSection) {
                        collectLineScoreData(line);
                    }
                    else if (line.Contains("-----------------"))
                        inFirstSection = false;
                    else
                    {
                        currentTeam = collectInMinorsData(line, currentTeam);
                    }
                }
            }
        }

        public void collectLineScoreData(string line)
        {
            Regex regex = new Regex(REGEX_LINE_SCOREA);
            Match matchLineScore = regex.Match(line);
            if (!matchLineScore.Success) {
                regex = new Regex(REGEX_LINE_SCOREB);
                matchLineScore = regex.Match(line);
            }

            if (matchLineScore.Success)
            {
                processLineScore(matchLineScore);
            }
        }


        public Team collectInMinorsData(string line, Team currentTeam)
        {
            if (line.Equals("(M) = In Minors   [4](J#) = Number Of Days Injured"))
                return currentTeam;

            // Strip off the year.
            string sabv = TeamUtils.prettyTeamNoDiceName(line.Substring(5));
            if (!sabv.Equals("UNK")) {
                Team teamAbv = getTeamByAbbreviation(sabv);
                if (teamAbv != null)
                {
                    return teamAbv;
                }
            }

            int idx = line.IndexOf("[4]");
            if (idx > 0)
            {
                line = line.Substring(0, idx).Trim();
            }

            String[] inMinors = line.Split(IN_MINORS, StringSplitOptions.RemoveEmptyEntries);
            currentTeam.setPlayersInMinors(inMinors);
            return currentTeam;
        }

        private void processLineScore(Match match)
        {
            String teama_abv = match.Groups[1].Value.Trim();
            int teama_score = Convert.ToInt32(match.Groups[2].Value);

            String teamb_abv = match.Groups[3].Value.Trim();
            int teamb_score = Convert.ToInt32(match.Groups[4].Value);

            Team teama = this.getTeamByAbbreviation(teama_abv);
            Team teamb = this.getTeamByAbbreviation(teamb_abv);

            teama.addLineScore(teama_score, teamb_abv, teamb_score, false);
            teamb.addLineScore(teamb_score, teama_abv, teama_score, true);
        }
    }
}
