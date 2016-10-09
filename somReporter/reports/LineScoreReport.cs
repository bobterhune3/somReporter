using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace somReporter
{
    public class LineScoreReport : Report
    {
        private const String REGEX_LINE_SCOREA = @".+?(?=[4])4]([A-Z][A-Z][A-Z]) +(.[0-9]+).+?(?=[0])0]([A-Z][A-Z][A-Z]) +(.[0-9]+)";
        private const String REGEX_LINE_SCOREB = @".+?(?=[0])0]([A-Z][A-Z][A-Z]) +(.[0-9]+).+?(?=[4])4]([A-Z][A-Z][A-Z]) +(.[0-9]+)";

        public LineScoreReport(string title) : base(title) {
        }

        public override void processReport() {
            bool inFirstSection = true;

            foreach (String line in m_lines)
            {
                if (line.Length > 0) {
                    if(!inFirstSection) {
                        collectData(line);
                    }
                    else if (line.Contains("-----------------"))
                        inFirstSection = false;
                }
            }
        }

        public void collectData(string line)
        {
            Regex regex = new Regex(REGEX_LINE_SCOREA);
            Match match = regex.Match(line);
            if (!match.Success) {
                regex = new Regex(REGEX_LINE_SCOREB);
                match = regex.Match(line);
            }

            if (match.Success)
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
}
