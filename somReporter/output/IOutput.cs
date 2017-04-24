using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using somReporter.team;

namespace somReporter.output
{
    public interface IOutput
    {
        void draftOrderHeader();
        void draftOrderTableHeader();
        void draftOrderTeamLine(int pickNum, int dicPick, Team team);
        void wildCardHeader(string league);
        void wildCardTableHeader();
        void wildCardTeamLine(int rank, Team team, string gamesBehind);
        void spacer();
        void setOutputHeader(string title);
        void setOutputFooter();
        void endOfTable();
        void divisionStandingsHeader(string division);
        void divisionStandingsTableHeader();
        void divisionStandingsTeamLine(int rank, Team team);
        void ShowWhosHotData(string v);
        void ShowInjuryData(string v);
        void recordBookHeader(bool teamRecord);
        void recordBookItem(SOMRecord rec, int counter, bool teamRecord);
        void usageHeader(int playerCount);
        void usageFooter();
        bool usageReportItem(Player player, int counter);
    }
}
