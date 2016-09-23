using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Team getTeamDataByCode(string code)
        {
            foreach (Team team in DATABASE.Teams())
            {
                if (team.Abrv.Equals(code))
                    return team;
            }
            return null;
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

    }
}
