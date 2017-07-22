using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using somReporter.output;
using somReporter.team;
using somReporter.util;
using Microsoft.Isam.Esent.Collections.Generic;

namespace somReporter.features
{
    class FeatureUsage : IFeature
    {
        private ComparisonReport teamComparisonReport;
        private PersistentDictionary<string, string> database;

        public Report getReport()
        {
            throw new NotImplementedException();
        }

        public void initialize(SOMReportFile teamReportFile)
        {
            Console.WriteLine("    Building Comparison...");
            Console.WriteLine("      Showing Moral=" + Config.SHOW_MORAL + ", Showing Warnings=" + Config.SHOW_WARNING);

            teamComparisonReport = (ComparisonReport)teamReportFile.FindReport("Comparison Report");
            teamComparisonReport.processReport();
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
                if (output.usageReportItem(player, counter)) {
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
                        int actual = int.Parse(playerData["PreviousReplay"]);
                        return actual;
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
