using Microsoft.Isam.Esent.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using somReporter.team;

namespace somReporter
{
    public class Report
    {
        public static DataStore DATABASE = new DataStore();
        private String m_Title;
        protected List<String> m_lines = new List<String>();
        
        public Report(String title)
        {
            m_Title = title;
        }

        public int getLineCount() { return m_lines.Count; }

        public String Name { get { return m_Title; } }

        internal void AddLine(string line)
        {
            m_lines.Add(line);
        }

        public virtual String getReportType() { return "UNKNOWN TYPE"; }


        public virtual void processReport()
        {
            throw new NotImplementedException();
        }

        public Team getTeamDataByName(string name)
        {
            foreach (Team team in DATABASE.Teams())
            {
                if (team.Name.Equals(name))
                    return team;
            }
            return null;
        }

        public static double RoundToSignificantDigits(double d, int digits)
        {
            if (d == 0.0)
            {
                return 0.0;
            }
            else
            {
                double leftSideNumbers = Math.Floor(Math.Log10(Math.Abs(d))) + 1;
                double scale = Math.Pow(10, leftSideNumbers);
                double result = scale * Math.Round(d / scale, digits, MidpointRounding.AwayFromZero);

                // Clean possible precision error.
                if ((int)leftSideNumbers >= digits)
                {
                    return Math.Round(result, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    return Math.Round(result, digits - (int)leftSideNumbers, MidpointRounding.AwayFromZero);
                }
            }
        }



        public List<Team> getTeamsByOwner(string owner)
        {
            List<Team> matches = new List<Team>();
            foreach (Team team in DATABASE.Teams())
            {
                if (team.Owner.Equals(owner))
                    matches.Add(team);
            }
            return matches;
        }


        public Team getTeamByAbbreviation(string abvr)
        {
            foreach (Team team in DATABASE.Teams())
            {
                if (team.Abrv.Equals(abvr))
                    return team;
            }
            return null;
        }

        public static PersistentDictionary<string, string> saveReportInformation(String reportName)
        {
            PersistentDictionary<string, string> dictionary = new PersistentDictionary<string, string>(reportName);

            List<Team> teams = Report.DATABASE.Teams();
            foreach (Team team in teams)
            {
                dictionary[team.Abrv] = team.buildStorageData();
            }

            List<Player> players = Report.DATABASE.Players();
            foreach( Player player in players)
            {
                String key = "Usage_" + player.Name + ":" + player.Team;
                dictionary[key] = player.buildStorageData();
            }

            return dictionary;
        }

        public void loadPreviousStorageInfo(PersistentDictionary<string, string>  dictionary)
        {
            Console.WriteLine("Read in Previous Save File");

            foreach (string teamAbrv in dictionary.Keys)
            {
                string data = dictionary[teamAbrv];

                Team team = this.getTeamByAbbreviation(teamAbrv);
                if (team != null ) {
                    Dictionary<string, string> teamData = loadStorageString(data);

                    team.WinsPrevious = Int32.Parse(teamData["Wins"]);
                    team.LosesPrevious = Int32.Parse(teamData["Loses"]);
                    team.GbPrevious = Double.Parse(teamData["GB"]);
                    team.DivisionPositionPrevious = Int32.Parse(teamData["DIVPos"]);
                    team.DraftPickPositionPrevious = Int32.Parse(teamData["DPickPos"]);
                    team.WildCardPositionPrevious = Int32.Parse(teamData["WCardPos"]);
                }
            }
        }

        public Dictionary<string, string> loadStorageString(String s)
        {
            var data = new Dictionary<string, string>();
            foreach (var row in s.Split('|'))
                data.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));

            return data;
        }

    }
}
