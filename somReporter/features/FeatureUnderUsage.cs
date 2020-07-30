using System;
using System.Collections.Generic;
using System.Linq;
using somReporter.output;
using somReporter.team;
using Microsoft.Isam.Esent.Collections.Generic;
using somReportUtils;

namespace somReporter.features
{
    class FeatureUnderUsage : IFeature
    {
        private ComparisonReport teamComparisonReport = null;

        public Report getReport()
        {
            throw new NotImplementedException();
        }

        public void initialize(ISOMReportFile teamReportFile)
        {

            Console.WriteLine("    Building Under Usage...");
            if (teamComparisonReport == null)
            {
                teamComparisonReport = (ComparisonReport)teamReportFile.FindReport("LEAGUE", "Comparison Report");

                teamComparisonReport.processReport(Program.LEAGUES[0].Length);
            }
        }

        public void setReportData(ComparisonReport report )
        {
            teamComparisonReport = report;
        }

        class UnderUsageStat
        {
            public float UsagePct;
            public Player Player;
        }

        public void process(IOutput output)
        {
            List<UnderUsageStat> underUsedHitters = new List<UnderUsageStat>();
            List<UnderUsageStat> underUsedPitchers = new List<UnderUsageStat>();

            List<Player> players = teamComparisonReport.getPlayers();

            output.underUsageHeader(players.Count);

            foreach (Player player in players)
            {
                int actual = player.Actual;
                int replay = player.Replay;
                float playPct = ((float)replay) / ((float)actual);

                if (playPct < .33)
                {
                    UnderUsageStat usage = new UnderUsageStat();
                    usage.Player = player;
                    usage.UsagePct = playPct;

                    if (player.IsHitter && actual > 299)
                    {
                        underUsedHitters.Add(usage);
                    }
                    else if (!player.IsHitter && actual > 100)
                    {
                        underUsedPitchers.Add(usage);
                    }
                }
            }

            List<UnderUsageStat> topHitters = underUsedHitters.OrderBy(x => x.UsagePct).ToList();
            List<UnderUsageStat> topPitchers = underUsedPitchers.OrderBy(x => x.UsagePct).ToList();

            for (int i = 1; i < 21; i++)
            {
                Player hitter = null;
                if (topHitters.Count >= i)
                    hitter = topHitters[i - 1].Player;
                else
                    hitter = new Player(true);

                Player pitcher = null;
                if (topPitchers.Count>= i)
                    pitcher = topPitchers[i - 1].Player;
                else
                    pitcher = new Player(true);

                output.underUsageReportItem(hitter, pitcher, i);
            }

            output.underUsageFooter();
        }

        public void setDateStore(PersistentDictionary<string, string> dictionary)
        {
            throw new NotImplementedException();
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
