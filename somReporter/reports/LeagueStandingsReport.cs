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
        private static String REGEX_HEADER      = @"([\s\S\]{0,15}) +([\S]+) +([\S]+) +([\S]+) +([\S]+) +([\s\S]+) ([\S]+) +([\S]+) ([\S]+) ([\S]+)";
        private static String REGEX_TEAM_RECORD = @"[0-9]+ ([\S]+) ([\S]+) +([0-9]+) +([0-9]+) +([(|1).0-9]+) +([\S]+) +([\S]+) +([0-9]+) +([0-9]+) +([(|1).0-9]";

        private String REPORT_TYPE = "LEAGUE STANDINGS";

        public LeagueStandingsReport(String title) : base(title)
        { }

        public override String getReportType() { return REPORT_TYPE; }

        public override void processReport() {
            String currentLeague = "";
            foreach( String line in m_lines)
            {
                if (isLeagueLine(line) ) {
          //          currentLeague = value;
                }
            }
        }

        private bool isLeagueLine(string line)
        {
            Regex regex = new Regex(REGEX_HEADER);
            Match match = regex.Match(line);
            if (match.Success)
            {
                field1 = match.Groups[1].Value.Trim();
                field2 = match.Groups[2].Value.Trim();
                field3 = match.Groups[3].Value.Trim();
                field4 = match.Groups[4].Value.Trim();
                field5 = match.Groups[5].Value.Trim();
                field6 = match.Groups[6].Value.Trim();
                field7 = match.Groups[7].Value.Trim();
                field8 = match.Groups[8].Value.Trim();
                field9 = match.Groups[9].Value.Trim();

                return true;
            }
            else
                return false;
            if (match.Groups.Count != 3)
                throw new Exception("Expecting ALL REPORTS to run, first report should be titled 'LEAGUE STANDINGS FOR'");

        }
    }
}
