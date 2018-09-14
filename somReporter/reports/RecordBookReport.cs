using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace somReporter
{
    public class RecordBookReport : Report
    {
        private static string REGEX_TEAM_RECORD1   = "^([0-9][/][ 0-9][0-9]) +([0-9]+) +([A-Z][A-Z][A-Z]) +[0-9]+ +[0-9]+ +[0-9]+ +([A-Z][A-Z][A-Z]) +";
        private static string REGEX_TEAM_RECORD2   = "^([0-9][/][ 0-9][0-9]) +([0-9]+) +([A-Z][A-Z][A-Z]) +?([A-Z][A-Z][A-Z]).+?([A-Z][A-Z][A-Z])";
        private static string REGEX_INDIVIDUAL_REC = "^([0-9][/][ 0-9][0-9]) +([0-9]+) +([a-zA-Z0-9- .\\[\\]]+)";

        private List<SOMRecord> team_records = new List<SOMRecord>();
        private List<SOMRecord> player_records = new List<SOMRecord>();
        public RecordBookReport(string title) : base(title) {
        }

        public List<SOMRecord> getTeamRecords() {
            return team_records;
        }

        public List<SOMRecord> getPlayerRecords() {
            return player_records;
        }

        public override void processReport(int n)
        {
            string currentTitle = "";
            foreach (String line in m_lines)
            {
                if (line.Trim().Length == 0)
                    continue;

                if(isHeader(line))
                    currentTitle = line;
                else {
                    string fixLine = line.Replace("[4]", "");
                    fixLine = fixLine.Replace("[0]", "");
                    fixLine.Trim();


                    Regex regex = new Regex(REGEX_TEAM_RECORD1);
                    Match match = regex.Match(fixLine);
                    if (match.Success)
                    {
                        //Group 1.    0 - 4 `6 / 30`
                        //Group 2.    6 - 8 `21`
                        //Group 3.    9 - 12    `CRM`
                        //Group 4.    17 - 20   `CRM`
                        //Group 5.    31 - 34   `SLB`
                        String date = match.Groups[1].Value;
                        int record = Convert.ToInt32(match.Groups[2].Value);

                        if (doWeCareAboutThisTeamRecord(currentTitle, record)) {

                            String team_rec1 = match.Groups[3].Value.Trim();
                            String team_opp = match.Groups[4].Value.Trim();

                            team_records.Add(new SOMRecord(currentTitle, record, team_rec1, team_opp));
                        }
                    }
                    else {
                        regex = new Regex(REGEX_TEAM_RECORD2);
                        match = regex.Match(fixLine);
                        if (match.Success)
                        {
                            //Group 1.    0 - 4 `6 / 30`
                            //Group 2.    6 - 8 `21`
                            //Group 3.    9 - 12    `CRM`
                            //Group 4.    17 - 20   `CRM`
                            //Group 5.    31 - 34   `SLB`
                            String date = match.Groups[1].Value;
                            int record = Convert.ToInt32(match.Groups[2].Value);

                            if (doWeCareAboutThisTeamRecord(currentTitle, record))
                            {

                                String team_rec1 = match.Groups[3].Value.Trim();
                                //    String team_rec2 = match.Groups[4].Value.Trim();
                                String team_opp = match.Groups[5].Value.Trim();

                                team_records.Add(new SOMRecord(currentTitle, record, team_rec1, team_opp));
                            }
                        }
                        else {
                            regex = new Regex(REGEX_INDIVIDUAL_REC);
                            match = regex.Match(fixLine);
                            if (match.Success)
                            {
                                //                            Group 1.    0 - 4 `9 / 6`
                                //                           Group 2.    6 - 7 `4`
                                //                         Group 3.    8 - 44    `B Harper of CHJ vs.PHM in 5 at - bats`
                                String date = match.Groups[1].Value;
                                int record = Convert.ToInt32(match.Groups[2].Value);
                                if (doWeCareAboutThisPlayerRecord(currentTitle, record)) { 
                                    String description = match.Groups[3].Value.Trim();
                                    player_records.Add(new SOMRecord(currentTitle, record, description));
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool doWeCareAboutThisPlayerRecord(string currentTitle, int record)
        {
            if (currentTitle.ToLower().StartsWith("at bats")) {
                return record > 9;
            }
            else if(currentTitle.ToLower().StartsWith("hits")) {
                return record > 5;
            }
            else if (currentTitle.ToLower().StartsWith("home runs allowed")) {
                return record > 5;
            }
            else if (currentTitle.ToLower().StartsWith("home runs")) {
                return record > 4;
            }
            else if (currentTitle.ToLower().StartsWith("runs scored")) {
                return record > 4;
            }
            else if (currentTitle.ToLower().StartsWith("rbis")) {
                return record > 7;
            }
            else if (currentTitle.ToLower().StartsWith("stolen bases")) {
                return record > 4;
            }
            else if (currentTitle.ToLower().StartsWith("least hits allowed")) {
                return record == 0;
            }
            else if (currentTitle.ToLower().StartsWith("most hits allowed")) {
                return record > 16;
            }
            else if (currentTitle.ToLower().StartsWith("strikeouts")) {
                return record > 17;
            }

            return false;
        }

        private bool doWeCareAboutThisTeamRecord(string currentTitle, int record)
        {
            if(currentTitle.ToLower().StartsWith("longest games")) {
                return record > 17;
            }
            else if(currentTitle.ToLower().StartsWith("most runs - 1 team")) {
                return record > 19;
            }
            else if (currentTitle.ToLower().StartsWith("most hits - 1 team")) {
                return record > 25;
            }
            return false;
        }


        public bool isHeader(string line) {
            
            if (line.Trim().Length == 0)
                return false;

            Match match = new Regex("^[0-9].+").Match(line);
            return !match.Success;
        }
    }

    public class SOMRecord
    {
        private string label = "";
        private int record = 0;
        private string team = "";
        private string opponent = "";
        private string description = "";


        public SOMRecord(string label, int record, string team, string opponent)
        {
            this.label = label;
            this.record = record;
            this.team = team;
            this.opponent = opponent;
        }

        public SOMRecord(string label, int record, string description)
        {
            this.label = label;
            this.record = record;
            this.description = description;
        }

        public string Label { get { return this.label; } }
        public int RecordValue { get { return this.record; } }
        public string Team { get { return this.team; } }
        public string Opponent { get { return this.opponent; } }
        public string Description { get { return this.description; } }
    }
}



