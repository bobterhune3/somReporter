using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace somReporter.reports
{
    public class ScheduleReport : Report
    {
        protected Dictionary<int, List<String>> m_days = new Dictionary<int, List<String>>();

        public ScheduleReport(string title) : base(title)
        {
        }

        public override void processReport()
        {

        }
    }
}
