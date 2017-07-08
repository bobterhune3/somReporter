using Microsoft.Isam.Esent.Collections.Generic;
using somReporter.output;
using somReporter.team;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using somReporter.reports;
using somReporter.util;
using somReporter.features;

namespace somReporter
{
    public class Program
    {
        public static string[] LEAGUES = { "" };
        //      private readonly string[] LEAGUES = { "AL", "NL" };
        public static string[] DIVISIONS = { "Federal", "American", "National" };
        public static string[] DIVISION_DRAFT_ORDER = { "National", "American", "Federal" };
        //      private readonly string[] DIVISIONS = { "East", "West" };

        public static IFeature featureStandings = null;
        public static IFeature featureDraftOrder = null;
        public static IFeature featureRecordBook = null;
        public static IFeature featureUsage = null;
        public static IFeature featureInjuries = new FeatureInjuries();

        public static Config cfg = new Config("config.properties");
        private LeagueGrandTotalsReport leaguePrimaryStatReport;
        private LineScoreReport lineScoreReport;
        private NewspaperStyleReport newspaperStyleReport;

        private SOMReportFile leagueReportFile;
        private SOMReportFile teamReportFile;

        private IOutput output;

        public Program() {
        //    output = new ConsoleOutput();
     //       output = new HTMLOutput();
            output = new BootstrapHTMLOutput();
        }

        public void cleanup() {
            output.setOutputFooter();
        }

        public IOutput outputStream() {
            return output;
        }                                    

        static void Main(string[] args)
        {
            Program program = new Program();

            int daysPlayed = 0;
            Console.Write("Number of Days Played? ");
            String daysPlayedText = Console.ReadLine();
            if (daysPlayedText.Length > 0)
                daysPlayed = int.Parse(daysPlayedText);

            Console.WriteLine("Intializing...");
            program.initialize(daysPlayed);


            string fileName = program.lookupPreviousSaveFile();
            PersistentDictionary<string, string> prevDictionaryFile = new PersistentDictionary<string, string>(fileName); ;
            if ( fileName.Length > 0 ) {
                program.loadPreviousStorageInfo(prevDictionaryFile);
            }

            featureStandings = FeatureFactory.loadFeature(FeatureFactory.FEATURE.STANDINGS);
            featureStandings.process(program.outputStream());

            Console.WriteLine("Process Who Hot...");
            program.showWhosHot();

            Console.WriteLine("Process Draft Order...");
            featureDraftOrder = FeatureFactory.loadFeature(FeatureFactory.FEATURE.DRAFT_ORDER);
            featureDraftOrder.process(program.outputStream());

            Console.WriteLine("Process Record Book...");
            featureRecordBook = FeatureFactory.loadFeature(FeatureFactory.FEATURE.RECORD_BOOK);
            featureRecordBook.process(program.outputStream());

            Console.WriteLine("Process Player Usage ...");
            featureUsage.setDateStore(prevDictionaryFile);
            featureUsage.process(program.outputStream());

            Console.WriteLine("Create Win Pct History Charts ...");
            ((FeatureStandings)featureStandings).buildCharts();

            Console.WriteLine("Press ESC to stop or S to save");
            ConsoleKey key;
            do
            {
                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.S)
                {
                    Console.WriteLine("Saving Data..");
                    program.saveReportInformation();
                    break;
                }
            } while (key != ConsoleKey.Escape);


            program.cleanup();
        }

        private void showWhosHot()
        {
            output.ShowWhosHotData(newspaperStyleReport.getWhosHotData());
            ((FeatureInjuries)featureInjuries).setData(newspaperStyleReport.getInjuryData());
            featureInjuries.process(output);
         }

        public String lookupPreviousSaveFile()
        {
            string[] directories = Directory.GetDirectories(Directory.GetCurrentDirectory(), leagueReportFile.SeasonTitle+"*");
            if (directories.Length == 0) {
                // If there is no database, reset the wpct history file
                if (File.Exists("wpct.csv"))
                    File.Delete("wpct.csv");
                return "";
            }

            int highestValue = 0;
            foreach (string dir in directories)
            {
                string value = dir.Substring(dir.IndexOf('-')+1);
                if (value.Length > 0)
                {
                    long theValue = 0;
                    Int64.TryParse(value, out theValue);
                    if (theValue > highestValue)
                        highestValue = (int)theValue;
                }
            }

            if (highestValue == 0)
                return "";

            string fileName = String.Format("{0}-{1}", leagueReportFile.SeasonTitle, highestValue);
            return fileName;
        }

        public void loadPreviousStorageInfo(PersistentDictionary<string, string>  prevDictionaryFile)
        {
            this.leaguePrimaryStatReport.loadPreviousStorageInfo(prevDictionaryFile);
        }

        public void initialize(int daysPlayed) {
            Report.DATABASE.reset();

            Console.WriteLine("  Loading League Report File ...");
            leagueReportFile = new SOMReportFile(Config.getConfigurationFile("ALL_REPORTS.PRT"));
            leagueReportFile.parseLeagueFile();


            IFeature feature = FeatureFactory.loadFeature(FeatureFactory.FEATURE.STANDINGS);
            feature.initialize(leagueReportFile);

            Console.WriteLine("    Building Grand Totals...");
            leaguePrimaryStatReport = (LeagueGrandTotalsReport)leagueReportFile.FindReport("LEAGUE GRAND TOTALS (primary report) FOR");
            leaguePrimaryStatReport.processReport();

            Console.WriteLine("    Building League Report ...");
            lineScoreReport = (LineScoreReport)leagueReportFile.FindReport("INJURY/MINOR LEAGUE REPORT FOR");
            lineScoreReport.processReport();

            Console.WriteLine("    Building Awards...");
            newspaperStyleReport = (NewspaperStyleReport)leagueReportFile.FindReport("AWARDS VOTING FOR");
            newspaperStyleReport.processReport();

            Console.WriteLine("    Building Record Book...");
            feature = FeatureFactory.loadFeature(FeatureFactory.FEATURE.RECORD_BOOK);
            feature.initialize(leagueReportFile);

            output.setOutputHeader(leagueReportFile.SeasonTitle, daysPlayed);

            Console.WriteLine("  Loading Team Report File...");
            teamReportFile = new SOMReportFile(Config.getConfigurationFile("TEAM_ALL_REPORTS.PRT"));
            teamReportFile.parseTeamFile();

            featureUsage = FeatureFactory.loadFeature(FeatureFactory.FEATURE.USAGE);
            featureUsage.initialize(teamReportFile);
        }

        public void saveReportInformation()
        {
            Report.saveReportInformation(buildReportDBName());
        }

        private String buildReportDBName()
        {
            return String.Format("{0}-{1}", leagueReportFile.SeasonTitle, Team.TOTAL_GAMES);
        }

        private void WriteOutLeadingTeamForWildCard(Team team, Team secondPlaceTeam)
        {
            double gb = secondPlaceTeam.calculateGamesBehind(team);
            WriteOutTeamForWildCard(1, team, gb);
        }

        private void WriteOutTeamForWildCard(int rank, Team team, double wcGamesBehind)
        {
            team.WildCardPositionCurrent = rank;

            String gamesBehind = "---";
            if (wcGamesBehind != 999) {
                gamesBehind = String.Format("{0}",wcGamesBehind);
            }

            output.wildCardTeamLine(rank, team, gamesBehind);
        }

    }
}
