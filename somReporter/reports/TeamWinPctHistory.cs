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

        public bool loadWinPctFile(string fileName)
        {
            try {
            string[] rawData = File.ReadAllLines(fileName);
                foreach (string rawLine in rawData)
                {
                    csvLoadTeamParser(rawLine);
                }

            }
            catch( Exception ex) { return false; }
            return true;
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
                else if (counter == 1)
                    team.WinPctHistoryGameCount = Int32.Parse(data);
                else
                    parsed.Add(Math.Round(float.Parse(data),3));
                counter++;
            }

            if (team != null)
                team.WinPctHistoryData = parsed;
        }

        public void csvSaveTeamParser(string csv, char separator = ',')
        {
            List<String> lines = new List<String>();
            List<Team> teams = DATABASE.Teams();

            foreach (Team team in teams)
            {
                string abrv = team.Abrv;
                int games = team.Wins + team.Loses;
                string data2 = String.Join(",", team.WinPctHistoryData.Select(x => x.ToString()).ToArray());
                string data = abrv + "," + games + "," + data2;
                lines.Add(data);
            }
            File.WriteAllLines(csv, lines);
        }

        public bool addCurrentSeason( List<Team> teams)
        {
            bool updatesMade = false;
            foreach (Team team in teams)
            {
                int curGames = team.Wins + team.Loses;
                if (curGames > team.WinPctHistoryGameCount)
                {
                    team.addWinPctHistoryData(team.Wpct);
                    updatesMade = true;
                }
            }
            return updatesMade;
        }
    }
}
