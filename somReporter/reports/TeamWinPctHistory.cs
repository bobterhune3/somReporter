using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using somReporter.team;

namespace somReporter.reports
{
    class TeamWinPctHistory : Report
    {
        public TeamWinPctHistory() : base("Win Pct History")
        { }

        //    private List<Team> teams = new List<Team>();

        public void loadWinPctFile(string fileName)
        {
            string[] rawData = File.ReadAllLines(fileName);
            foreach (string rawLine in rawData)
            {
                csvLoadTeamParser(rawLine);
      //          teams.Add(team);
            }
            return;
        }

        private void csvLoadTeamParser(string csv, char separator = ',')
        {
            Team team = null;
            List<double> parsed = new List<double>();
            string[] temp = csv.Split(separator);
            int counter = 0;
            string data = string.Empty;
            while (counter < temp.Length)
            {
                data = temp[counter].Trim();
                if (data.Trim().StartsWith("\""))
                {
                    bool isLast = false;
                    while (!isLast && counter < temp.Length)
                    {
                        data += separator.ToString() + temp[counter + 1];
                        counter++;
                        isLast = (temp[counter].Trim().EndsWith("\""));
                    }
                }
                if (counter == 0)
                    team = getTeamByAbbreviation(data);
                else
                    parsed.Add(float.Parse(data));
                counter++;
            }

            if (team != null)
                team.WinPctHistoryData = parsed;
        }

        private void csvSaveTeamParser(string csv, char separator = ',')
        {
            List<String> lines = new List<String>();
            List<Team> teams = DATABASE.Teams();

            foreach (Team team in teams)
            {
                string abrv = team.Abrv;
                string data2 = String.Join(",", team.WinPctHistoryData.Select(x => x.ToString()).ToArray());
                string data = abrv + "," + data2;
                lines.Add(data);
            }
            File.WriteAllLines(csv, lines);
        }
    }
}
