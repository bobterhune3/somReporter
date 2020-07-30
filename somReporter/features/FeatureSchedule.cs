using System;
using Microsoft.Isam.Esent.Collections.Generic;
using somReporter.output;
using System.Collections.Generic;
using somReporter.reports.team;
using System.Text.RegularExpressions;
using System.Text;
using somReportUtils;

namespace somReporter.features
{
    public class FeatureSchedule : IFeature
    {
        private List<ScheduleDay> schedule = new List<ScheduleDay>();
        private const String REG_EX_FOR_DAY_NUMBER = "[0-9 ][0-9]/[0-9 ][0-9]/[0-9][0-9][0-9][0-9] +([0-9]+)";
        public Report getReport()
        {
            throw new NotImplementedException();
        }

        public void initialize(ISOMReportFile file)
        {
            List<String> lines = file.readFileLinesOnly(false);

            ScheduleDay day = null;
            String header = "";
            String games = "";
            foreach ( String line in lines ) {
                if (line.StartsWith("["))
                    continue;
                else
                {
                    header = line.Substring(0, 15).Trim();
                    if(header.StartsWith("Schedule for "))
                    {
                        continue;
                    }
                    if(header.Length == 0)
                    {
                        //This is a continuation of the previous day
                        games = line.Substring(16).Trim();
                        day.addGames(games);
                    }
                    else
                    {
                        if( day != null )
                            schedule.Add(day);
                        // New Day.
                        day = new ScheduleDay();
                        day.dayNumber = getDayNumber(line);
                        // Store the previous day
                        games = line.Substring(15).Trim();
                        day.addGames(games);

                     }
                }
            }
        }

        private int getDayNumber(String line)
        {
            Regex regex = new Regex(REG_EX_FOR_DAY_NUMBER);
            Match gameDayMatch = regex.Match(line);
            if (gameDayMatch.Success)
            {
                return Int32.Parse(gameDayMatch.Groups[1].Value.Trim());
            }
            return 0;
        }

        public String getTeamSchedulesForDays( string teamAbbrv, int startDayNumber, int numberOfDays)
        {
            StringBuilder sb = new StringBuilder();
            foreach( ScheduleDay day in schedule)
            {
                if( day.dayNumber >= startDayNumber  && day.dayNumber < startDayNumber+numberOfDays )
                {
                    sb.Append(day.getTeamsGameToday(teamAbbrv));
                }
            }
            return sb.ToString();
        }

        public void process(IOutput output)
        {
            throw new NotImplementedException();
        }

        public void setDateStore(PersistentDictionary<string, string> dictionary)
        {
            throw new NotImplementedException();
        }
    }
}
