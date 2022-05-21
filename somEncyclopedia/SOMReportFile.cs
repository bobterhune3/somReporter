using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using somEncyclopedia.team;
using somReporter;
using somReporter.team;
using somReportUtils;

namespace somEncyclopedia
{
    public class SOMReportFile : ISOMReportFile
    {
        private String m_fileName = "";
        private String m_Season = "";
        private String m_LeagueName = "";
        private String m_TeamName = "";
        private String m_SeasonTitle = "";
        private Dictionary<string,List<Report>> allReports = new Dictionary<string, List<Report>>();

        public string Season
        {
            get {return m_Season; }
        }

        public string LeagueName
        {
            get { return m_LeagueName; }
        }

        public string TeamName
        {
            get { return m_TeamName; }
        }

        public string SeasonTitle
        {
            get { return m_SeasonTitle; }
        }

        public Dictionary<string, List<Report>> getAllReports()
        {
           return allReports;
        }

        public Report FindReport(String team, String name)
        {
            List<Report> reports = allReports[team];
            foreach (Report report in reports)
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

        public void parseLeagueFile() {
             List<String> lines = readReportLines(true);
            organizeByReportType("LEAGUE", lines);
        }

        public void parseTeamFile()
        {
            List<String> lines = readReportLines(false);
            organizeByTeamAndReportType(lines);
        }

        private void organizeByTeamAndReportType(List<string> lines)
        {
            Report currentReport = null;
            String reportTitle = "";
            String currentTeam = "";
            bool skipThisReport = false;
            foreach (String line in lines)
            {
                Regex regex = new Regex(@"(.*) [0-9]+ (.*) [0-9]+ [\S]+");
                Match match = regex.Match(line);
                if (match.Success && line.Contains(" For "))
                {
                    reportTitle = match.Groups[1].Value.Trim();
                    int x = reportTitle.IndexOf(" For ");
                    if (x > 0)
                        reportTitle = reportTitle.Substring(0, x);
                    try
                    {
                        currentTeam = match.Groups[2].Value.Substring(0, 13).Trim();
                    }
                    catch (Exception) { }

                    if (currentReport != null)
                    {
                        currentReport = loadReport(reportTitle, currentTeam);
                        skipThisReport = !(currentReport is TeamReport);
                        if (!skipThisReport && FindReport(currentTeam, reportTitle) == null)
                            addReport(currentTeam,currentReport);
                    }
                    else
                    {
                        currentReport = loadReport(reportTitle, currentTeam);
                        skipThisReport = !(currentReport is TeamReport);
                        if (!skipThisReport && FindReport(currentTeam, reportTitle) == null)
                            addReport(currentTeam,currentReport);
                    }
                }
                else
                {
                    if (!skipThisReport)
                        ((TeamReport)currentReport).AddLine(currentTeam, line);
                }

                if (isNewReport(reportTitle, currentTeam))
                    addReport(currentTeam, currentReport);
            }
        }

        private void addReport(string team, Report report)
        {
            if (!getAllReports().ContainsKey(team))
                getAllReports().Add(team, new List<Report>());
            List<Report> reports = getAllReports()[team];
            if(reports == null )
            {
                reports = new List<Report>();
            }
            reports.Add(report);
        }

        private Report loadReport(string reportTitle, string currentTeam)
        {
            
            if (reportTitle.StartsWith("Primary Player Statistics For"))
                return new PrimaryReport(reportTitle, currentTeam);
            else if (reportTitle.StartsWith("Secondary Player Statistics For"))
                return new SecondaryReport(reportTitle, currentTeam);
            
            return new Report(reportTitle);
        }

        private void organizeByReportType(String team, List<string> lines)
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

                        if (isNewReport(reportTitle, team))
                        {
                            currentReport = loadReport(reportTitle, team);
                            addReport(team, currentReport);
                        }
                    }
                    else
                    {
                        currentReport = loadReport(reportTitle, team);
                        addReport(team, currentReport);
                    }
                }
                else {
                    currentReport.AddLine(line);
                }

            }
            if (isNewReport(reportTitle, team))
                addReport(team, currentReport);
        }

        private bool isNewReport(string reportTitle, string team)
        {
            if (!getAllReports().ContainsKey(team))
                return true;
            List<Report> reports = getAllReports()[team];
            foreach ( Report report in reports )
            {
                if (report.Name.Equals(reportTitle))
                    return false;

            }
            return true;
        }

        public List<String> readReportLines(Boolean bParseLeagueFile) {
            List<String> lines = new List<String>();
            System.IO.StreamReader file = null;
            try { 
                string line;

                // Read the file and display it line by line.
                file = new System.IO.StreamReader(m_fileName);
                while((line = file.ReadLine()) != null)
                {
                    line = cleanUpLine(line);
                    if( line.Trim().Length == 0)
                        continue;
                    if (lines.Count == 0 && bParseLeagueFile)
                        parseLeagueFirstLineOfReport(line);
                    else if (lines.Count == 0 ) {
                        if (line.Trim().Length == 0)
                            continue;
                        parseTeamFirstLineOfReport(line);
                    }
                    lines.Add(line);
                }
            }
            finally {
                if(file != null )
                  file.Close();
            }
            return lines;
         }

        public List<String> readFileLinesOnly(Boolean cleanup)
        {
            List<String> lines = new List<String>();
            System.IO.StreamReader file = null;
            try
            {
                string line;

                file = new System.IO.StreamReader(m_fileName);
                while ((line = file.ReadLine()) != null)
                {
                    if(cleanup)
                        line = cleanUpLine(line);
                    if (line.Trim().Length == 0)
                        continue;
                    lines.Add(line);
                }
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
            return lines;
        }

        public void parseLeagueFirstLineOfReport(string line)
        {
            Regex regex = new Regex(@"\.* ([0-9]+) (.*)");
            Match match = regex.Match(line);
            if(match.Groups.Count != 3)
                throw new Exception("Expecting ALL REPORTS to run, first report should be titled 'LEAGUE STANDINGS FOR'");
            m_SeasonTitle = match.Value.Trim();
            m_Season = match.Groups[1].Value.Trim();
            m_LeagueName = match.Groups[2].Value.Trim();
        }

        
      public void parseTeamFirstLineOfReport(string line)
        {
            Regex regex = new Regex(@"\.* ([0-9]+) (.*)");
            Match match = regex.Match(line);
            if (match.Groups.Count != 3)
                throw new Exception("Expecting ALL REPORTS to run, first report should be titled 'LEAGUE STANDINGS FOR'");
            m_SeasonTitle = match.Value.Trim();
            m_Season = match.Groups[1].Value.Trim();
            m_TeamName = match.Groups[2].Value.Substring(0, 13).Trim();
        }

        public string cleanUpLine(string line)
        {
            //       line = line.Replace("[0]", "");
            line = line.Replace("[1]", "");
            line = line.Replace("[2]", "");
            line = line.Replace("[3]", "");
            //   line = line.Replace("[4]", "");
            return line.Trim();
        }
    }
}
