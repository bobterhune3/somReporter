using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace somReporter
{
    public class DataStore
    {
        private List<Team> teams = new List<Team>();

        public DataStore() { }

        public void addTeam( Team team ) {
            teams.Add(team);
        }

        public List<Team> Teams() {
            return teams;
        }

        public void reset()
        {
            teams.Clear();
        }
    }
}
