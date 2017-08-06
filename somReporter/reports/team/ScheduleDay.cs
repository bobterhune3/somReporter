using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace somReporter.reports.team
{
    public class ScheduleDay
    {
        private DateTime date;
        public int dayNumber;
        Dictionary<String, List<String>> teamSchedules = new Dictionary<String, List<String>>();

        //private XXXX List of team games
        public void addGames(String line)
        {
            if (line.Length == 0 || !line.Contains("-"))
                return;

            line = line.Replace('*', ' ');
            string[] games = line.Split(' ');
            foreach(String game in games )
            {
                if (game.Length > 0 && line.Contains('-'))
                {
                    string[] teams = game.Split('-');
                    addAGame(teams[1], teams[0], true);
                    addAGame(teams[0], teams[1], false);
                }
            }
        }

        private void addAGame(String team1, String team2, Boolean teamOneHome)
        {
            List<String> teamGames;
            if (!teamSchedules.ContainsKey(team1))
            {
                teamGames = new List<String>();
                teamSchedules.Add(team1, teamGames);
            }
            else
                teamGames = teamSchedules[team1];

            if (teamOneHome)
                teamGames.Add("vs " + team2);
            else
                teamGames.Add("at " + team2);
        }

        public String getTeamsGameToday(String team)
        {
            if( !teamSchedules.ContainsKey(team))
            {
                return "day off<br>";
            }
            List<String> games = teamSchedules[team];
            StringBuilder sb = new StringBuilder();
            foreach(String game in games)
            {
                if (sb.Length > 0)
                    sb.Append("<br>");
                sb.Append(game);
            }
            sb.Append("<br>");
            return sb.ToString();
        }
    }
}
