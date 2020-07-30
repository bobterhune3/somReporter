using System;
using System.Collections.Generic;
using System.Linq;
using somReporter.output;
using somReporter.team;
using Microsoft.Isam.Esent.Collections.Generic;
using somReportUtils;
using somEncyclopedia.team;

namespace somReporter.features
{
    class FeatureSecondary : IFeature
    {
        private List<SecondaryReport> allSecondaryReports = new List<SecondaryReport>();
        //    private PersistentDictionary<string, string> database;

        public List<SecondaryReport> getReports()
        {
            return allSecondaryReports;
        }

        public void initialize(ISOMReportFile teamReportFile)
        {
            Console.WriteLine("    Building Secondary Player Statistics...");
            Dictionary<string, List<Report>> allReports = teamReportFile.getAllReports();
            foreach (string team in allReports.Keys)
            {
                SecondaryReport teamSecondaryReport = (SecondaryReport)teamReportFile.FindReport(team, "Secondary Player Statistics For");
                if (teamSecondaryReport == null)
                {
                    System.Console.WriteLine("Are you sure you selected 'ALL REPORTS' for the Team Reports?");
                    throw new Exception("Unable to find Secondary Player Statistics Report in the Team Report File");
                }
                teamSecondaryReport.processReport(3);

                allSecondaryReports.Add(teamSecondaryReport);
            }
        }

        public void setDateStore(PersistentDictionary<string, string> dictionary) {
         //   this.database = dictionary;
        }

        public void process(IOutput output) {
            /*
            List<Player> players = teamComparisonReport.getPlayers();
         
            output.usageHeader(players.Count);
            int counter = 1;

            Team currentTeam = null;
            foreach (Player player in players)
            {
                if (currentTeam == null || !currentTeam.Equals(player.Team))
                    counter = 1;
                currentTeam = player.Team;
                int previousReplay = checkForPreviousStorageInfo(player);
                if (previousReplay > 0)
                    player.PreviousReplay = previousReplay;
                if (output.usageReportItem(player, counter)) {
                    Report.DATABASE.addPlayerUsage(player);
                    counter++;
                }
            }

            output.usageFooter();
            */
        }

        public int checkForPreviousStorageInfo( Player player)
        {
            return 0;
        }

        public Report getReport()
        {
            throw new NotImplementedException();
        }
    }
}
