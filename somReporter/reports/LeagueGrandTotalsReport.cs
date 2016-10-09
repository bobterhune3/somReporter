using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace somReporter
{
    public class LeagueGrandTotalsReport : Report
    {
        private const String REPORT_TYPE = "LEAGUE GRAND TOTALS";
        private const String REGEX_HEADER = @"[TEAM]+]{0,15} +([\S]+)";

      //  https://regex101.com/

        private const String REGEX_TEAM_HIT_STATS = @"[0-9]+ (.+?(?=[4]))4] +(.[0-9]+) +(.[0-9]+) +(.[0-9]+) +(.[0-9]+) +(.[0-9]+) +(.[0-9]+) +(.[0-9]+) +(.[0-9]+) +(.[0-9]+) +(.[0-9]+) +(.[0-9]+)";
        private const String REGEX_TEAM_PIT_STATS = @"[0-9]+ (.+?(?=[4]))4] +([0-9].[0-9]+) +(.[0-9]+) +(.[0-9]+) +([0-9]+.[0-9]) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9]+) +([0-9])";

        public LeagueGrandTotalsReport(string title) : base(title)
        {
        }

        public override String getReportType() { return REPORT_TYPE; }

        private bool bProcessHitters = true;

        public override void processReport()
        {
            foreach (String line in m_lines)
            {
                if( line.Length > 0) {
                    string fixedLine = line.Replace("[0]", "");
                    collectData(fixedLine);
               }
            }
        }

        public void collectData(string line )
        {
            Regex regex = new Regex(REGEX_HEADER);
            Match headerMatch = regex.Match(line);

            if (headerMatch.Success)
            {
                String statType = headerMatch.Groups[1].Value.Trim();
                if (statType.Equals("AVG"))
                    bProcessHitters = true;
                else
                    bProcessHitters = false;
            }
            else
            {
                if (bProcessHitters)
                    processHitters(line);
                else
                    processPitchers(line);
            }
        }

        private void processPitchers(String line)
        {
            Regex regex = new Regex(REGEX_TEAM_PIT_STATS);
            Match teamMatch = regex.Match(line);
            if (teamMatch.Success)
            {
                String name = teamMatch.Groups[1].Value.Trim();
                name = name.Substring(0, name.Length - 1).Trim();
                if (name.Length > 4)
                {
                    Team team = getTeamDataByName(name);
                    team.ERA = Convert.ToDouble(teamMatch.Groups[2].Value.Trim());
                    //        team.Win = Convert.ToInt32(teamMatch.Groups[3].Value.Trim());
                    //        team.Lost = Convert.ToInt32(teamMatch.Groups[4].Value.Trim());
                    team.IP = Convert.ToDouble(teamMatch.Groups[5].Value.Trim());
                    //        team.Hits = Convert.ToInt32(teamMatch.Groups[6].Value.Trim());
                    team.RunsAllowed = Convert.ToInt32(teamMatch.Groups[7].Value.Trim());
                    //        team.EarnedRuns = Convert.ToInt32(teamMatch.Groups[8].Value.Trim());
                    team.HomeRuns = Convert.ToInt32(teamMatch.Groups[9].Value.Trim());
                    //        team.Walks = Convert.ToInt32(teamMatch.Groups[10].Value.Trim());
                    //        team.Strikeouts = Convert.ToInt32(teamMatch.Groups[10].Value.Trim());

                }
            }
        }

        private void processHitters(String line)
        {
            Regex regex = new Regex(REGEX_TEAM_HIT_STATS);
            Match teamMatch = regex.Match(line);
            if (teamMatch.Success)
            {
                String name = teamMatch.Groups[1].Value.Trim();
                name = name.Substring(0, name.Length - 1).Trim();
                if (name.Length > 4)
                {
                    Team team = getTeamDataByName(name);
                    team.Average = Convert.ToDouble(teamMatch.Groups[2].Value.Trim());
            //        team.AtBat = Convert.ToInt32(teamMatch.Groups[3].Value.Trim());
                    team.RunsScored = Convert.ToInt32(teamMatch.Groups[4].Value.Trim());
            //        team.Hits = Convert.ToInt32(teamMatch.Groups[5].Value.Trim());
            //        team.Doubles = Convert.ToInt32(teamMatch.Groups[6].Value.Trim());
            //        team.Triples = Convert.ToInt32(teamMatch.Groups[7].Value.Trim());
            //        team.HomeRun = Convert.ToInt32(teamMatch.Groups[8].Value.Trim());
            //        team.StolenBase = Convert.ToInt32(teamMatch.Groups[9].Value.Trim());
            //        team.CaughtSteal = Convert.ToInt32(teamMatch.Groups[10].Value.Trim());
                    
                }
            }
        }
    }
}