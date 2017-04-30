using somReporter.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using somReporter.team;

namespace somReporter.output
{
    class BootstrapHTMLOutput : IOutput
    {
        List<String> lines = new List<String>();
        private const string S_HTML_EXTRA_TEXT = "<div class=\"alert alert-info\" role=\"alert\"><a class=\"alert-link\">{0}</a></div>";
                  
        public BootstrapHTMLOutput()
        {
        }

        public void setOutputHeader(string title)
        {
            System.IO.StreamReader file = null;
            try
            {
                string line;

                // Read the file and display it line by line.
                file = new System.IO.StreamReader(".\\resources\\bootstrapHTMLHeader.txt");
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


            String extraText = checkForExtraText();
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
            addTableCell(team.Name, "#000000", 149, "", false);
            addTableCell(team.Wins, "#000000", 48);
            addTableCell(team.Loses, "#000000", 48);
            addTableCell(team.Wpct, 3, "#000000", 53);
            if (team.Gb == 0)
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
            addTableCell(team.Name, "#000000", 149, "", false);
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

        public void ShowInjuryData(string v)
        {
            setBookmark("injury");
            lines.Add("<h2 class=\"sub-header\">Current Injuries</h2>");

            lines.Add("<pre>" + v + "</pre>");
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
                addTableHeaderCell("Replay", 75, "The number of AB/IP so far in the replay.");
                addTableHeaderCell("Drop Dead", 75, "The Maximum AB/IP available.  Going past this will cause a penalty.");
                lines.Add("</tr></thead><tbody>");
            }
        }


        public bool usageReportItem(Player player, int counter)
        {
            string usageLevel = getUsageLevel(player.Actual, player.TargetUsage, player.Replay, player.IsHitter);

            if (usageLevel.Length == 0) //Skip line if player does not fall within boundry
                return false;

            lines.Add("<tr>");
            addTableCell(counter, "#000000", 30);
            lines.Add(String.Format("<td>{0}</td>", usageLevel));
            addTableCell(player.Name, "#000000", 100);
            addTableCell(prettyTeamName(player.Team), "#000000", 60);
            addTableCell(player.IsHitter ? "B" : "P", "#000000", 60);
            addTableCell(player.Actual, "#000000", 75);
            addTableCell(player.Replay, "#000000", 75);
            addTableCell(player.TargetUsage, "#000000", 75);

            lines.Add("</tr>");
            return true;
        }


        public void usageFooter()
        {
            endOfTable();


            lines.Add("<div class=\"panel panel-default\"><div class=\"panel-body\">");
            lines.Add("<b>Key:</b></br>");
            if (Config.SHOW_WARNING)
                lines.Add(String.Format("<span class=\"label label-info\">Warning</span>Above Usage Level of {0}%</br>", Config.WARNING_LEVEL * 100));
            else
                lines.Add("<span class=\"label label-info\">Warning</span>Not Shown In the Report</br>");

            if (Config.SHOW_MORAL)
                lines.Add(String.Format("<span class=\"label label-warning\">Danger</span>Above Suggested Usage Warning Level {0}%</br>", Config.SUGGESTION_LEVEL_PERCENT * 100));
            else
                lines.Add("<span class=\"label label-warning\">Danger</span>Above Suggested Usage Not Shown</br>");

            lines.Add("<span class=\"label label-danger\">Violation</span>Above Drop Dead Level</br>");
            lines.Add("</div></div>");
        }

        private string getUsageLevel(int actual, int target, int replay, bool isHitter)
        {
            /*
                Hitters <= 101 actual at bats are allowed at 150%    (ie ab * 1.5)
                Hitters > 101 actual at bats are actual at bats + 50  (ie ab + 50)

                Pitchers >= 100 innings is innings + 30  
                Pitchers < 100 is 110% of actual   (99 * 1.1) = +29
              */

            if (isHitter)
            {

                if (replay > target)        // Penality For Hitters
                    return "<span class=\"label label-danger\">Violation</span>";
                if (replay > (((float)actual) * ((float)Config.SUGGESTION_LEVEL_PERCENT)) && Config.SHOW_MORAL)        // Moral Ceiling
                    return "<span class=\"label label-warning\">Danger</span>";
                if (replay > (((float)actual) * ((float)Config.WARNING_LEVEL)) && Config.SHOW_WARNING)         // Danger Level
                    return "<span class=\"label label-info\">Warning</span>";
                return "";
            }
            else
            {
                if (replay > target)         // Penality For Pitchers
                    return "<span class=\"label label-danger\">Violation</span>";
                if (replay > ((float)actual * (float)Config.SUGGESTION_LEVEL_PERCENT) && Config.SHOW_MORAL)         // Moral Ceiling
                    return "<span class=\"label label-warning\">Danger</span>";
                if (replay > ((float)actual * (float)Config.WARNING_LEVEL) && Config.SHOW_WARNING)          // Danger Level
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
            catch (Exception ex) { }

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
                return String.Format("Gained 1 spot", rankDif);
            else if (rankDif < 0)
                return String.Format("Gained {0} spots", rankDif * -1);
            else if (rankDif == 1)
                return String.Format("Dropped 1 spot", rankDif);
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
                lines.Add(String.Format("<td><span style='color:{0}' width='{1}' class=\"tooltip-bottom\" title=\"{2}\"/>{3}</td>",
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

        private string prettyTeamName(string teamName)
        {
            if (teamName.Equals("Anaheim Ange")) return "ANS";
            if (teamName.Equals("Arizona Diam")) return "AZB";
            if (teamName.Equals("Chicago Cubs")) return "CHB";
            if (teamName.Equals("Cleveland In")) return "CLM";
            if (teamName.Equals("Detroit Tige")) return "DTB";
            if (teamName.Equals("Kansas City")) return "KCM";
            if (teamName.Equals("Los Angeles")) return "LAM";
            if (teamName.Equals("Miami Marlin")) return "MMS";
            if (teamName.Equals("Milwaukee Br")) return "MLG";
            if (teamName.Equals("New York Yan")) return "NYB";
            if (teamName.Equals("Oakland Athl")) return "OKM";
            if (teamName.Equals("Philadelphia")) return "PHM";
            if (teamName.Equals("Pittsburgh P")) return "PTB";
            if (teamName.Equals("San Diego Pa")) return "SDG";
            if (teamName.Equals("San Francisc")) return "SFJ";
            if (teamName.Equals("Seattle Mari")) return "SEG";
            if (teamName.Equals("St. Louis Ca")) return "SLB";
            if (teamName.Equals("Tampa Bay Ra")) return "TBM";
            if (teamName.Equals("Texas Ranger")) return "TXG";
            if (teamName.Equals("Toronto Blue")) return "TOG";
            if (teamName.Equals("Washington N")) return "WSG";
            return "UNK";
        }
    }
}
