using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using somReporter.output;
using somReporter.team;
using somReporter.util;

namespace somReporter.features
{
    class FeatureUsage : IFeature
    {
        private ComparisonReport teamComparisonReport;

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
                if (output.usageReportItem(player, counter))
                    counter++;
            }

            output.usageFooter();
        }
    }
}
