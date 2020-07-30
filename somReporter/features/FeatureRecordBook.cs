using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Isam.Esent.Collections.Generic;
using somReporter.output;
using somReportUtils;
using somReportUtils.output;

namespace somReporter.features
{
    class FeatureRecordBook : IFeature
    {
        public RecordBookReport recordBookReport;

        public Report getReport()
        {
            throw new NotImplementedException();
        }

        public void initialize(ISOMReportFile leagueReportFile)
        {
            recordBookReport = (RecordBookReport)leagueReportFile.FindReport("LEAGUE", "RECORD BOOK FOR FOR");
            recordBookReport.processReport(Program.LEAGUES[0].Length);
        }

        public void process(IOutput output)
        {
            int counter = 1;
            List<SOMRecord> teamRecords = ((RecordBookReport)recordBookReport).getTeamRecords();
            if (teamRecords.Count > 0)
            {
                output.recordBookHeader(true);
                foreach (SOMRecord rec in teamRecords)
                {
                    output.recordBookItem(rec, counter++, true);
                }
            }
            output.endOfTable();

            counter = 1;
            List<SOMRecord> playerRecords = ((RecordBookReport)recordBookReport).getPlayerRecords();
            if (playerRecords.Count > 0)
            {
                output.recordBookHeader(false);
                foreach (SOMRecord rec in playerRecords)
                {
                    output.recordBookItem(rec, counter++, false);
                }
            }
            output.spacer();
            output.endOfTable();
        }

        public void setDateStore(PersistentDictionary<string, string> dictionary)
        {
            throw new NotImplementedException();
        }
    }
}
