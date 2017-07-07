using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using somReporter.team;

namespace somReporter
{
    public class DataStore
    {
        private List<Team> teams = new List<Team>();
        private List<Player> players = new List<Player>();

        public DataStore() { }

        public void addTeam( Team team ) {
            teams.Add(team);
        }

        public void addPlayerUsage( Player player ) {
            players.Add(player);
        } 

        public List<Team> Teams() {
            return teams;
        }

        public List<Player> Players()
        {
            return players;
        }

        public Team getTeam(String abbrv) {
            foreach(Team team in teams ) {
                if (team.Abrv.Equals(abbrv))
                    return team;
            }
            return null;
        }

        public void reset()
        {
            teams.Clear();
            players.Clear();
        }
    }
}
