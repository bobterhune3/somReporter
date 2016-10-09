using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace somReporter
{
    class NewspaperStyleReport : Report
    {
        String whosHotData = "NO DATA";
        public NewspaperStyleReport(string title) : base(title) {
        }

        public override void processReport()
        {
            bool inFirstSection = true;
            List<String> data = new List<String>();
            foreach (String line in m_lines)
            {
                if (line.Length > 0)
                {
                    string cleanLine = line.Replace("[4]", "");
                    cleanLine = cleanLine.Replace("[0]", "");
                    if (cleanLine.StartsWith("WHO'S HOT")) {
                        data.Add("<b>"+ cleanLine + "</b><br/>");
                        inFirstSection = false;
                    }
                    else if (cleanLine.StartsWith("WHO'S NOT")) {
                        data.Add("<br/><b>" + cleanLine + "</b><br/>");
                    }
                    else if (cleanLine.StartsWith("INJURY REPORT"))
                    {
                        data.Add("<br/><b>" + cleanLine + "</b><br/>");
                    }
                    else if (!inFirstSection)
                    {
                        data.Add(cleanLine + "<br/>");
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach(string s in data) {
                sb.Append(s);
            }
            whosHotData = sb.ToString();
        }
           
        public string getWhosHotData() {
            return whosHotData;
        } 
    }
}

