using somReporter.util;
using somReporter.util.somReporter;
using System;
using System.Collections.Generic;
using System.Text;
using somReporter.team;
using somReporter.features;
using somReportUtils.output;

namespace somReporter.output
{
    class BootstrapHTMLOutput : IOutput
    {
        List<String> lines = new List<String>();
        private const string S_HTML_EXTRA_TEXT = "<div class=\"alert alert-info\" role=\"alert\"><a class=\"alert-link\">{0}</a></div>";
        private FeatureSchedule featureSchedule = null;
        private static String REPLACE_LEAGUE_NAME = "#{LEAGUE_NAME}";

        public BootstrapHTMLOutput()
        {
            featureSchedule = (FeatureSchedule)FeatureFactory.loadFeature(FeatureFactory.FEATURE.SCHEDULE);
        }

        public void setOutputHeader(string title, int daysPlayed)
        {
            String daysPlayedText = String.Format("{0} days played", daysPlayed);


            System.IO.StreamReader file = null;
            try
            {
                string line;

                // Read the file and display it line by line.
                file = new System.IO.StreamReader(".\\resources\\bootstrapHTMLHeader.txt");
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(REPLACE_LEAGUE_NAME))
                        line = line.Replace(REPLACE_LEAGUE_NAME, Config.LEAGUE_NAME);
                    lines.Add(line);
                }
            }
            finally
            {
                if (file != null)
                    file.Close();
            }

            lines.Add(string.Format(S_HTML_EXTRA_TEXT, daysPlayedText));

            String extraText = checkForExtraText().Trim();
            if (extraText.Length > 0)
                lines.Add(string.Format(S_HTML_EXTRA_TEXT, extraText));
        }

        public void setOutputFooter()
        {
            System.IO.StreamReader file = null;
            try
            {
                string line;

                // Read the file and display it line by line.
                file = new System.IO.StreamReader(".\\resources\\bootstrapHTMLFooter.txt");
                while ((line = file.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
            System.IO.File.WriteAllLines(@"index.html", lines);
        }

        private void setBookmark(string bookmark) {
            lines.Add(String.Format("<p><h2 class=\"sub-header\" id=\"{0}\"/> </h2></p>", bookmark));
        }

        /// 
        /// STANDINGS
        /// 

        public void divisionStandingsHeader(string division)
        {
            setBookmark("standings");
            string name = division.Replace(" ", "");
            lines.Add(String.Format("<h2 class=\"sub-header\">{0} League Standings</h2> <a href=\"winpct_{1}.html\">Trend Chart!</a>", division, name.ToUpper()));
        }

        public void divisionStandingsTableHeader()
        {
            lines.Add("<div class=\"table-responsive\"><table class=\"table table-striped\"><thead><tr>");

            addTableHeaderCell("#",    15);
            addTableHeaderCell("Team", 149);
            addTableHeaderCell("Won",  48);
            addTableHeaderCell("Lost", 48);
            addTableHeaderCell("Pct.", 53);
            addTableHeaderCell("GB",   53);
            addTableHeaderCell("Last", 53, "Last Run");
            addTableHeaderCell("Pyth", 48, "Pythagerean");
            addTableHeaderCell("BAvg", 48, "Team Batting Average");
            addTableHeaderCell("HR",   48, "Team Home Runs hit");
            addTableHeaderCell("ERA",  48);

            lines.Add("</tr></thead><tbody>");
        }

        public void divisionStandingsTeamLine(int rank, Team team)
        {
            lines.Add("<tr>");
            addTableCell(rank, returnRankDifColor(rank, team.DivisionPositionPrevious),
                         15, returnRankDifToolTip(rank, team.DivisionPositionPrevious));
            addTableCell(team.Name, "#000000", 149, returnNextScheduleToolTip(team), false);
            addTableCell(team.Wins, "#000000", 48);
            addTableCell(team.Loses, "#000000", 48);
            addTableCell(team.Wpct, 3, "#000000", 53);
            if (Config.SHOW_DIV_MAGIC_NUM && team.Gb == 0 && rank == 1)
                addTableCell(calcMagicNumber(team), "#000000", 53, "Magic Number");
            else if (team.Gb == 0)
                addTableCell("--", "#000000", 53, returnGBDifToolTip(team));
            else
                addTableCell(team.Gb, 1, "#000000", 53, returnGBDifToolTip(team));
            addTableCell(team.RecordLastRun,"#000000", 53, returnRecentRecordsTip(team));

            addTableCell(team.PythagoreanTheorem, 3, setPythagoreanTheoremColor(team), 48);
            addTableCell(team.BattingAverage, 3,getTeamBattingAverageColor(team), 48);
            addTableCell(team.HomeRuns, 0, getTeamHomeRunsColor(team), 48);
            addTableCell(team.EarnedRunAvg, 2, getTeamERAColor(team), 48);
            lines.Add("</tr>");
        }

        /// 
        /// DRAFT ORDER
        /// 

        public void draftOrderHeader()
        {
            setBookmark("picks");
            lines.Add("<h2 class=\"sub-header\">Draft Pick Order</h2> <a href=\"winpct_draftorder.html\">Trend Chart!</a>");
            lines.Add(String.Format("<p>First 4 picks subject to lottery</p>"));
        }

        public void draftOrderTableHeader()
        {
            lines.Add("<div class=\"table-responsive\"><table class=\"table table-striped\"><thead><tr>");
            addTableHeaderCell("Pick #",48);
            addTableHeaderCell("Team",  149);
            addTableHeaderCell("Owner", 65);
            addTableHeaderCell("League",75);
            addTableHeaderCell("Won",   48);
            addTableHeaderCell("Lost",  48);
            addTableHeaderCell("Pct.",  53);
            addTableHeaderCell("Pyth",  53);

            lines.Add("</tr></thead><tbody>");
        }

        public void draftOrderTeamLine(int pickNum, int divPick, Team team)
        {
            lines.Add("<tr>");
            addTableCell(pickNum + 1, returnRankDifColor(pickNum, team.DraftPickPositionPrevious),
                         48, returnRankDifToolTip(pickNum, team.DraftPickPositionPrevious));
            addTableCell(team.Name, "#000000", 149, returnNextScheduleToolTip(team), false);
            addTableCell(team.Owner, "#000000", 65);
            addTableCell(team.Division, "#000000", 75);
            addTableCell(team.Wins, "#000000", 48);
            addTableCell(team.Loses, "#000000", 48);
            addTableCell(team.Wpct, 3, "#000000", 53);
            addTableCell(team.PythagoreanTheorem, 3, "#000000", 53);
            lines.Add("</tr>");
        }


        /// 
        /// RECORD BOOK
        /// 

        public void recordBookHeader(bool teamRecords)
        {
            setBookmark("records");
            lines.Add(String.Format("<h2 class=\"sub-header\">{0} Record Book</h2>", teamRecords ? "Team" : "Player"));

            lines.Add("<div class=\"table-responsive\"><table class=\"table table-striped\"><thead><tr>");

            if (teamRecords)
            {
                addTableHeaderCell("Record",200);
                addTableHeaderCell("Value", 60);
                addTableHeaderCell("Team",  60);
                addTableHeaderCell("VS",    60);
            }
            else
            {
                addTableHeaderCell("Record",     200);
                addTableHeaderCell("Value",      60);
                addTableHeaderCell("Description",300);
            }
            lines.Add("</tr></thead><tbody>");
        }

        public void recordBookItem(SOMRecord rec, int counter, bool teamRecord)
        {
            lines.Add("<tr>");
            if (teamRecord)
            {
                addTableCell(rec.Label, "#000000", 200, "", false);
                addTableCell(rec.RecordValue,  "#000000", 60, "", false);
                addTableCell(rec.Team, "#000000", 60);
                addTableCell(rec.Opponent, "#000000", 60);
            }
            else
            {
                addTableCell(rec.Label, "#000000", 200, "", false);
                addTableCell(rec.RecordValue, "#000000", 60);
                addTableCell(rec.Description, "#000000", 300, "", false);
            }
            lines.Add("</tr>");
        }

        /// 
        /// WHO IS HOT/NOT - INJURY
        /// 

        public void ShowWhosHotData(string v)
        {
            setBookmark("hot");
            lines.Add("<h2 class=\"sub-header\">Who's Hot!</h2>");

            lines.Add("<pre>" + v + "</pre>");
        }

        public void ShowInjuryData(List<string> injuredPlayers, List<string> returningPlayers)
        {
            setBookmark("injury");
            lines.Add("<h2 class=\"sub-header\">Injury Report</h2>");

            lines.Add("<h4>Currently Injured Players</h4>");
            lines.Add("<pre>" );
            foreach (String hurtPlayer in injuredPlayers)
                lines.Add( hurtPlayer );
            lines.Add("</pre>");

            lines.Add("<h4>Returning Players</h4>");

            lines.Add("<pre>");
            foreach (String returningPlayer in returningPlayers)
                lines.Add(returningPlayer);
            lines.Add("</pre>");
        }

        /// 
        /// USAGE
        /// 

        public void usageHeader(int playerCount)
        {
            setBookmark("usage");
            lines.Add("<h2 class=\"sub-header\">Over-Usage Report</h2>");

            if (playerCount == 0)
                lines.Add("<pre>No players currently meets report.</pre>");
            else
            {
                lines.Add("<div class=\"table-responsive\"><table class=\"table table-striped\"><thead><tr>");

                addTableHeaderCell("#",      30);
                addTableHeaderCell("Level",  50);
                addTableHeaderCell("Player", 100);
                addTableHeaderCell("Team",   60);
                addTableHeaderCell("Type",   60);
                addTableHeaderCell("Actual", 75, "The actual number of AB/IP on the card.");
                addTableHeaderCell("Replay", 75, "The number of AB/IP so far in the replay.<br><br>Hover over number to view prediction of final usage.");
                addTableHeaderCell("Drop Dead", 75, "<b>The Maximum AB/IP available.</b><br>Going past this will cause a penalty.<br>" +
                    "Hitters <= 101 actual at bats are allowed at 150%    (ie ab * 1.5)<br>" +
                    "Hitters > 101 actual at bats are actual at bats + 50(ie ab + 50)<br>" +
                    "Pitchers >= 60 innings is innings + 30<br>" +
                    "Pitchers < 60 is 150 % of actual(60 * 1.5)" +
                    "<br><br>" +
                    "Hover over number to view The change in AB/IP added since the previous run."
                    );
                addTableHeaderCell("REMAINING", 30, "Number of AB/IP left until penality is reached");
                addTableHeaderCell("DIFF", 30, "Number of AB/IP since the previous run");
                lines.Add("</tr></thead><tbody>");
            }
        }


        public bool usageReportItem(Player player, int counter, bool inMInors)
        {
            string usageLevel = getUsageLevel(player.Actual, player.TargetUsage, player.Replay, player.IsHitter);

            if (usageLevel.Length == 0) //Skip line if player does not fall within boundry
                return false;

            // we are not reporting Hal team until there is a violation.
            if (player.Team.Abrv[2] == 'H' && usageLevel.Contains("Violation"))
                return false;

            lines.Add("<tr>");
            addTableCell(counter, "#000000", 30);
            lines.Add(String.Format("<td>{0}</td>", usageLevel));
            addTableCell(player.Name, "#000000", 100);
            addTableCell(player.Team.Abrv, "#000000", 60);
            addTableCell(player.IsHitter ? "B" : "P", "#000000", 60);
            addTableCell(player.Actual, "#000000", 75);

            float gp = (float)player.Team.GamesPlayed;
            float gpPct = gp / 162f;
            float explodedUsage = ((float)player.Replay) / gpPct;
            addTableCell(player.Replay, "#000000", 75, String.Format("predicted final usage is {0}", (int)explodedUsage));

            String change = getRunDeltaChange(player);
            if (change.Equals("X"))
            {
                addTableCell(player.TargetUsage, "#000000", 75, "New addition to list");
            }
            else
            {
                addTableCell(player.TargetUsage, "#000000", 75, String.Format("{0} new {1} last run.",
                    getRunDeltaChange(player),
                    player.IsHitter ? "AB" : "IP"));
            }

            int remaining = player.TargetUsage - player.Replay;
            String colorRemaining = "#000000";  //black
            if ( player.IsHitter && remaining < 12)
                colorRemaining = "#FF0000";
            else if ( !player.IsHitter && remaining < 8)
                colorRemaining = "#FF0000";
            addTableCell(remaining, colorRemaining, 30);

            string postfix = inMInors ? " (M)" : "";

            if (player.PreviousReplay == 0)
                addTableCell("NEW"+ postfix, "#000000", 30);
            else
                addTableCell((player.Replay - player.PreviousReplay) + postfix, "#000000", 30);
            lines.Add("</tr>");
            return true;
        }



        private String getRunDeltaChange(Player player) {
            if (player.PreviousReplay == 0)
                return "X";

            if (player.Replay == player.PreviousReplay)
                return "no change";

            return Convert.ToString(player.Replay - player.PreviousReplay);
        }

        public void usageFooter()
        {
            endOfTable();


            lines.Add("<div class=\"panel panel-default\"><div class=\"panel-body\">");
            lines.Add("<b>Key:</b>");
            if (Config.SHOW_WARNING)
                lines.Add(String.Format("<span class=\"label label-info\">Warning</span>Above Usage Level of {0}%</br>", Config.WARNING_LEVEL * 100));
            else
                lines.Add("<span class=\"label label-info\">Warning</span>Not Shown In the Report</br>");

            if (Config.SHOW_MORAL)
                lines.Add(String.Format("<span class=\"label label-warning\">Danger</span>Getting close to Target Usage Level {0}%</br>", Config.SUGGESTION_LEVEL_PERCENT * 100));
            else
                lines.Add("<span class=\"label label-warning\">Danger</span>Above Actual Usage Not Shown</br>");

            lines.Add("<span class=\"label label-danger\">Violation</span>Above Drop Dead Level</br>");
            lines.Add("</div></div>");
        }



        /// 
        /// UNDER USAGE
        /// 
        public void underUsageHeader(int playerCount)
        {
            setBookmark("Most UnderUsed Players");
            lines.Add("<h2 class=\"sub-header\">Under-Usage Report</h2>");
            lines.Add("<pre>Hitters over 300ab or Pitchers over 100ip with less than 33% usage.</pre>");

            if (playerCount == 0)
                lines.Add("<pre>No players currently meets report.</pre>");
            else
            {
                lines.Add("<div class=\"table-responsive\"><table class=\"table table-striped\"><thead><tr>");

                addTableHeaderCell("#", 30);
                addTableHeaderCell("Hitter", 50);
                addTableHeaderCell("Team", 60);
                addTableHeaderCell("Actual", 40, "The actual number of AB/IP on the card.");
                addTableHeaderCell("Replay", 40, "The number of AB/IP so far in the replay.");
                addTableHeaderCell("Usage %", 40, "Usage Percentage.");
                addTableHeaderCell("", 10);
                addTableHeaderCell("Pitcher", 50);
                addTableHeaderCell("Team", 60);
                addTableHeaderCell("Actual", 40, "The actual number of AB/IP on the card.");
                addTableHeaderCell("Replay", 40, "The number of AB/IP so far in the replay.");
                addTableHeaderCell("Usage %", 40, "Usage Percentage.");
                lines.Add("</tr></thead><tbody>");
            }
        }

        public bool underUsageReportItem(Player player, Player pitcher, int playerCount)
        {
            lines.Add("<tr>");
            addTableCell(playerCount, "#000000", 30);

            if (player == null || player.Empty)
            {
                addTableCell("---", "#000000", 50);
                addTableCell("---", "#000000", 60);
                addTableCell("---", "#000000", 40);
                addTableCell("---", "#000000", 40);
                addTableCell("---", "#000000", 40);
            }
            else
            {
                addTableCell(player.Name, "#000000", 50);
                addTableCell(player.Team.Abrv, "#000000", 60);
                addTableCell(player.Actual, "#000000", 40);
                addTableCell(player.Replay, "#000000", 40);
                float pct = ((float)player.Replay / (float)player.Actual) * 100f;
                addTableCell(pct.ToString("0.0")+ "%", "#000000", 40);
            }

            addTableCell(playerCount, "#000000", 30);

            if (pitcher == null || pitcher.Empty)
            {
                addTableCell("---", "#000000", 50);
                addTableCell("---", "#000000", 60);
                addTableCell("---", "#000000", 40);
                addTableCell("---", "#000000", 40);
                addTableCell("---", "#000000", 40);
            }
            else
            {
                addTableCell(pitcher.Name, "#000000", 50);
                addTableCell(pitcher.Team.Abrv, "#000000", 60);
                addTableCell(pitcher.Actual, "#000000", 40);
                addTableCell(pitcher.Replay, "#000000", 40);
                float pct = ((float)pitcher.Replay / (float)pitcher.Actual) * 100f;
                addTableCell(pct.ToString("0.0") + "%", "#000000", 40);
            }

            lines.Add("</tr>");
            return true;
        }



        public void underUsageFooter()
        {
            endOfTable();
        }

        private string getUsageLevel(int actual, int target, int replay, bool isHitter)
        {
            /*
                Hitters < 120 actual at bats are allowed at 150%      (ie ab * 1.5)
                Hitters > 120 actual at bats are actual at bats + 60  (ie ab + 60)
                Hitters > 600 actual at bats are allowed at 110%      (ie ab * 1.1)

                Pitchers < 60 is 150% of actual   (59 * 1.5) = +29
                Pitchers > 118 innings is innings + 30  
                Pitchers > 199 is 115% of actual (199 * 1.15) = +29
              */

            if (isHitter)
            {

                if (replay > target)        // Penality For Hitters
                    return "<span class=\"label label-danger\">Violation</span>";

                if (replay > actual)
                {
                    float midpoint = (target - actual)/2;
                    if( replay > actual+(int)midpoint)
                        return "<span class=\"label label-warning\">Danger</span>";
                }

                float calc = ((float)actual) * ((float)Config.WARNING_LEVEL);
                if (replay > calc && Config.SHOW_WARNING)         // Danger Level
                    return "<span class=\"label label-info\">Warning</span>";
                return "";
            }
            else
            {
                if (replay > target)         // Penality For Pitchers
                    return "<span class=\"label label-danger\">Violation</span>";
                if (replay > actual)
                {
                    float midpoint = (target - actual) / 2;
                    if (replay > actual + (int)midpoint)
                        return "<span class=\"label label-warning\">Danger</span>";
                }
                float calc = ((float)actual) * ((float)Config.WARNING_LEVEL);
                if (replay > calc && Config.SHOW_WARNING)          // Danger Level
                    return "<span class=\"label label-info\">Warning</span>";
                return "";
            }
        }

        /// 
        /// WILDCARD - Not Currently Used by this league
        /// 

        public void wildCardHeader(string league)
        {
            
        }

        public void wildCardTableHeader()
        {
            
        }

        public void wildCardTeamLine(int rank, Team team, string gamesBehind)
        {
            
        }

        /// 
        /// UTILITIES
        /// 

        public void endOfTable()
        {
            lines.Add("</tbody></table></div>");
        }


        public void spacer()
        {
        }

        private string checkForExtraText()
        {
            String result = "";
            try
            {
                result = System.IO.File.ReadAllText(@"updatedText.txt");
            }
            catch (Exception) { }

            return result;
        }

        private string returnRankDifColor(int rank, int previous)
        {
            int rankDif = rank - previous;
            if (rankDif > 0)
                return "#FF0000";
            else if (rankDif < 0)
                return "#00FF00";
            else
                return "#000000";
        }

        private string returnRankDifToolTip(int rank, int previous)
        {
            int rankDif = rank - previous;
            if (rankDif == -1)
                return "Gained 1 spot";
            else if (rankDif < 0)
                return String.Format("Gained {0} spots", rankDif * -1);
            else if (rankDif == 1)
                return "Dropped 1 spot";
            else if (rankDif > 0)
                return String.Format("Dropped {0} spots", rankDif);
            return "No Change";
        }

        private string returnGBDifToolTip(Team team)
        {
            double dif = team.Gb - team.GbPrevious;
            if (dif < 0)
                return String.Format("Gained {0} games", dif * -1);
            else if (dif > 0)
                return String.Format("Dropped {0} games", dif);
            return "No Change";
        }

        private string returnNextScheduleToolTip(Team team)
        {
            return featureSchedule.getTeamSchedulesForDays(team.Abrv,
                Program.daysPlayed+1, //Want tomorrows game!
                Config.SCHEDULE_NUMBER_OF_DAYS);
        }

        private string returnRecentRecordsTip(Team team)
        {
            int gamesToReport = (team.Wins - team.WinsPrevious) + (team.Loses - team.LosesPrevious);
            List<Game> games = team.GetLastGames(gamesToReport);
            List<Pair> allSeries = new List<Pair>();
            Pair currentSeries = null;
            foreach (Game game in games)
            {
                if (currentSeries == null)
                {
                    currentSeries = new Pair(game.DisplayOpponent);
                    allSeries.Add(currentSeries);
                }
                else if (!currentSeries.MetaData().Equals(game.DisplayOpponent))
                {
                    currentSeries = new Pair(game.DisplayOpponent);
                    allSeries.Add(currentSeries);
                }

                if (game.Won)
                    currentSeries.AddWin();
                else
                    currentSeries.AddLoss();
            }

            StringBuilder sb = new StringBuilder();
            for (int i = allSeries.Count - 1; i >= 0; i--)
            {
                //      foreach(Pair series in allSeries) {
                sb.Append(allSeries[i].ToString());
                sb.Append("<br>");
            }

            return sb.ToString();
        }


        private string getTeamBattingAverageColor(Team team)
        {
            if (team.getLeader(Team.CATEGORY.BATTING_AVERAGE))
                return "#3BC63B";
            if (team.getTrailing(Team.CATEGORY.BATTING_AVERAGE))
                return "#FF0000";
            else
                return "#5E5E5E";
        }

        private string getTeamHomeRunsColor(Team team)
        {
            if (team.getLeader(Team.CATEGORY.HOME_RUNS))
                return "#3BC63B";
            if (team.getTrailing(Team.CATEGORY.HOME_RUNS))
                return "#FF0000";
            else
                return "#5E5E5E";
        }

        private string getTeamERAColor(Team team)
        {
            if (team.getLeader(Team.CATEGORY.EARNED_RUNS_AVG))
                return "#3BC63B";
            if (team.getTrailing(Team.CATEGORY.EARNED_RUNS_AVG))
                return "#FF0000";
            else
                return "#5E5E5E";
        }

        private string getBackgroundColor(int rank, bool bWildCardColors = false)
        {

            if (rank % 2 == 0)
            {
                return (bWildCardColors) ? "ccffcc" : "#BDD6EE";
            }
            else
            {
                return (bWildCardColors) ? "b3ffb3" : "#DEEAF6";
            }
        }

        private string setPythagoreanTheoremColor(Team team)
        {
            double py = team.PythagoreanTheorem;
            double wpct = team.Wpct;
            double diff = (wpct - py) * 1000;
            if (diff > -10.5 && diff < 10.5)
                return "#5E5E5E";
            else if (diff < 0)
                return "#11811C";
            else
                return "#811111";

        }


        private void addTableHeaderCell(string text, int width, string tooltip = "", bool center = true)
        {
            if (tooltip.Length > 0)
                lines.Add(String.Format("<th><span width='{0}' class=\"tooltip-bottom\" title=\"{1}\"/>{2}</th>",
                    width, tooltip, text));
            else
                lines.Add(String.Format("<th><span width='{0}'/>{1}</th>",
                    width, text));
        }

        private void addTableCell(string text, string textColor, int width, string tooltip = "", bool center = true)
        {
            if (tooltip.Length > 0)
                lines.Add(String.Format("<td><span style='color:{0}' width='{1}' html='true' class=\"tooltip-bottom\" title=\"{2}\"/><i>{3}</i></td>",
                    textColor, width, tooltip, text));
            else
                lines.Add(String.Format("<td><span style='color:{0}' width='{1}'/>{2}</td>",
                    textColor, width, text));
        }

        private void addTableCell(double number, int precision, string textColor, int width, string tooltip = "", bool center = true)
        {
            switch (precision)
            {
                case 0:
                    addTableCell(String.Format("{0}", number), textColor, width, tooltip);
                    break;
                case 1:
                    addTableCell(String.Format("{0:0.0}", number), textColor, width, tooltip);
                    break;
                case 2:
                    addTableCell(String.Format("{0:0.00}", number), textColor, width, tooltip);
                    break;
                default:
                    addTableCell(String.Format("{0:.000}", number), textColor, width, tooltip);
                    break;
            }
        }

        private void addTableCell(int number, string textColor, int width, string tooltip = "", bool center = true)
        {
            addTableCell(number.ToString(), textColor, width, tooltip);
        }

        private string calcMagicNumber(Team team)
        {
            int value = 163 - team.Wins - team.SecondPlaceTeamLosses;
            return "[" + value + "]";
        }

    }
}
