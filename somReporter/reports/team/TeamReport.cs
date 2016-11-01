using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace somReporter.team
{
    abstract public class TeamReport : Report
    {
        protected Dictionary<String, List<String>> m_Teamlines = new Dictionary<String, List<String>>();

        public TeamReport(string title) : base(title) {
        }

        public override void processReport()
        {
        }

        public void AddLine(string team, string line)
        {
            List<String> lines;

            if (!m_Teamlines.ContainsKey(team)) { 
                lines = new List<String>();
                m_Teamlines.Add(team, lines);
            }

            lines = m_Teamlines[team];
            lines.Add(line);
        }
    }
}
