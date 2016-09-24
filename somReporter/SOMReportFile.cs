using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace somReporter
{
    public class SOMReportFile 
    {
        private String m_fileName = "";
        private String m_Season = "";
        private String m_LeagueName = "";
        private String m_SeasonTitle = "";
        private List<Report> reports = new List<Report>();

        public string Season
        {
            get {return m_Season; }
        }

        public string LeagueName
        {
            get { return m_LeagueName; }
        }

        public string SeasonTitle
        {
            get { return m_SeasonTitle; }
        }

        public List<Report> Reports
        {
            get {return reports; }
        }

        public Report FindReport(String name)
        {
            foreach(Report report in reports)
            {
                if (report.Name.Equals(name))
                    return report;
            }
            return null;
        }


        public SOMReportFile( String reportPath ) {
            m_fileName = reportPath;
            if (!File.Exists(m_fileName))
                throw new Exception("File " + reportPath + " cannot be found");

        }

        public void parseFile() {
             List<String> lines = readReportLines();
            organizeByReportType(lines);
        }

        private void organizeByReportType(List<string> lines)
        {
            Report currentReport = null;
            String reportTitle = "";
            foreach ( String line in lines )
            {
                Regex regex = new Regex(@"(.*) "+ m_SeasonTitle);
                Match match = regex.Match(line);
                if (match.Success)
                {
                    reportTitle = match.Groups[1].Value.Trim();

                    if (currentReport != null)
                    {

                        if (isNewReport(reportTitle))
                        {
                            currentReport = loadReport(reportTitle);
                            Reports.Add(currentReport);
                        }
                    }
                    else
                    {
                        currentReport = loadReport(reportTitle);
                        Reports.Add(currentReport);
                    }
                }
                else {
                    currentReport.AddLine(line);
                }

            }
            if (isNewReport(reportTitle))
                reports.Add(currentReport);
        }

        private Report loadReport(string reportTitle)
        {
            if (reportTitle.StartsWith("LEAGUE STANDINGS"))
                return new LeagueStandingsReport(reportTitle);
            if (reportTitle.StartsWith("LEAGUE GRAND TOTALS (primary report) FOR"))
                return new LeagueGrandTotalsReport(reportTitle);
            else
                return new Report(reportTitle);
        }

        private bool isNewReport(string reportTitle)
        {
            foreach( Report report in reports )
            {
                if (report.Name.Equals(reportTitle))
                    return false;

            }
            return true;
        }

        public List<String> readReportLines() {
            List<String> lines = new List<String>();
            System.IO.StreamReader file = null;
            try { 
                string line;

                // Read the file and display it line by line.
                file =
                       new System.IO.StreamReader(m_fileName);
                while((line = file.ReadLine()) != null)
                {
                    line = cleanUpLine(line);
                    if (lines.Count == 0)
                        parseFirstLineOfReport(line);
                    lines.Add(line);
                }
            }
            finally {
                if(file != null )
                  file.Close();
            }
            return lines;
         }

        public void parseFirstLineOfReport(string line)
        {

            Regex regex = new Regex(@"\.* ([0-9]+) (.*)");
            Match match = regex.Match(line);
            if(match.Groups.Count != 3)
                throw new Exception("Expecting ALL REPORTS to run, first report should be titled 'LEAGUE STANDINGS FOR'");
            m_SeasonTitle = match.Value.Trim();
            m_Season = match.Groups[1].Value.Trim();
            m_LeagueName = match.Groups[2].Value.Trim();
        }

        public string cleanUpLine(string line)
        {
            line = line.Replace("[0]", "");
            line = line.Replace("[1]", "");
            line = line.Replace("[2]", "");
            line = line.Replace("[3]", "");
         //   line = line.Replace("[4]", "");
            return line.Trim();
        }
    }
}
