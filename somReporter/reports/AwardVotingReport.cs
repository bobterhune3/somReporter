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
        String injuryData = "NO DATA";

        public NewspaperStyleReport(string title) : base(title) {
        }

        public override void processReport()
        {
            bool inFirstSection = true;
            List<String> dataWhoHot = new List<String>();
            List<String> dataInjury = new List<String>();
            Boolean bDoingWhosHot = true;
            foreach (String line in m_lines)
            {
                if (line.Length > 0)
                {
                    string cleanLine = line.Replace("[4]", "");
                    cleanLine = cleanLine.Replace("[0]", "");
                    if (cleanLine.StartsWith("WHO'S HOT")) {
                        dataWhoHot.Add("<b>"+ cleanLine + "</b><br/>");
                        inFirstSection = false;
                        bDoingWhosHot = true;
                    }
                    else if (cleanLine.StartsWith("WHO'S NOT")) {
                        dataWhoHot.Add("<br/><b>" + cleanLine + "</b><br/>");
                        bDoingWhosHot = true;
                    }
                    else if (cleanLine.StartsWith("INJURY REPORT"))
                    {
              //          dataInjury.Add(cleanLine + "\r\n");
                        bDoingWhosHot = false;
                    }
                    else if (!inFirstSection)
                    {
                        if (bDoingWhosHot)
                            dataWhoHot.Add(cleanLine + "<br/>");
                        else
                            dataInjury.Add(cleanLine + "\r\n");

                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach(string s in dataWhoHot) {
                sb.Append(s);
            }
            whosHotData = sb.ToString();

            StringBuilder sbInjury = new StringBuilder();
            foreach (string s in dataInjury)
            {
                sbInjury.Append(s);
            }
            injuryData = sbInjury.ToString();
        }
           
        public string getWhosHotData() {
            return whosHotData;
        } 

        public string getInjuryData() {
            return injuryData;
        }
    }
}

