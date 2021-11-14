using System;
using System.Collections.Generic;
using System.Linq;
using somReporter.output;
using somReporter.team;
using somReporter.util.somReporter;
using Microsoft.Isam.Esent.Collections.Generic;
using somReportUtils;

namespace somReporter.features
{
    class FeatureUsage : IFeature
    {
        public ComparisonReport teamComparisonReport;
        private PersistentDictionary<string, string> database;
        private PrimaryStatsReport teamPrimaryStatsReport = null;
        public Report getReport()
        {
            throw new NotImplementedException();
        }

        public void initialize(ISOMReportFile teamReportFile)
        {
            Console.WriteLine("    Building Comparison...");
            Console.WriteLine("      Showing Moral=" + Config.SHOW_MORAL + ", Showing Warnings=" + Config.SHOW_WARNING);

            teamPrimaryStatsReport = (PrimaryStatsReport)teamReportFile.FindReport("LEAGUE", "Primary Player Statistics For");
            teamPrimaryStatsReport.processReport(Program.LEAGUES[0].Length);
            List<Player> replayPlayers = teamPrimaryStatsReport.getPlayers();


            teamComparisonReport = (ComparisonReport)teamReportFile.FindReport("LEAGUE","Comparison Report");
            if (teamComparisonReport == null) {
                System.Console.WriteLine("Are you sure you selected 'ALL REPORTS' for the Team Reports?");
                throw new Exception("Unable to find Comparison Report in the Team Report File");
            }
            teamComparisonReport.setPlayerActualData(replayPlayers);
            teamComparisonReport.processReport(Program.LEAGUES[0].Length);
        }

        public void setDateStore(PersistentDictionary<string, string> dictionary) {
            this.database = dictionary;
        }

        public void process(IOutput output) {
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
                bool inMinors = teamPrimaryStatsReport.getTeamByAbbreviation(TeamUtils.prettyTeamNoDiceName(currentTeam.Abrv)).isPlayerInMinors(player);
                if (output.usageReportItem(player, counter, inMinors)) {
                    Report.DATABASE.addPlayerUsage(player);
                    counter++;
                }
            }

            output.usageFooter();
        }

        public int checkForPreviousStorageInfo( Player player)
        {
            foreach (string key in database.Keys)
            {
                if( key.Equals("Usage_"+player.Name+":"+player.Team)) {
                    Dictionary<string, string> playerData = loadStorageString(database[key]);
                    try
                    {
                        int storedValue = int.Parse(playerData["PreviousReplay"]);
                        return storedValue;
                    }
                    catch(Exception)
                    {
                        return 0;
                    }
                }
            }
            return 0;
        }


        private Dictionary<string, string> loadStorageString(String s)
        {
            var data = new Dictionary<string, string>();
            foreach (var row in s.Split('|'))
                data.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));

            return data;
        }
    }
}
