using somReporter.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace somReporter.output
{
    class HTMLOutput : IOutput
    {
        private const string S_DOC_HTML_TITLE = "<html><head><title>{0} Strat-O-Matic League</title></head><style>{1}</style><body>\r\n";
        private const string S_DOC_HEADER = "<h1 align = center style='text-align:center'>{0} - Strat-O-Matic League</h1>\r\n";
        private const string S_LINK_TO_SOM_PAGE = "<p><a href = \"../../../../../../2015ND/index.html\" > Click HERE for full details</a></p>\r\n";
        private const string S_HTML_EXTRA_TEXT = "<pre>{0}</pre>";
        private const string S_TABLE_HEADER_CELL         = "<td width={0} style='border:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt;background:{1};height:13.8pt'><p align={2}><span style='color:{3}'>&nbsp;{4}</span></p></td>";
        private const string S_TABLE_HEADER_CELL_TOOLTIP = "<td width={0} style='border:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt;background:{1};height:13.8pt'><p align={2}><span class='tooltip' style='color:{3}'>&nbsp;{4}<span class='tooltiptext'>{5}</span></span></p></td>";

        List<String> lines = new List<String>();

        public HTMLOutput() {
        }


        public void setOutputHeader(string title)
        {
            lines.Add(String.Format(S_DOC_HTML_TITLE, title, getToolTipScript()));
            lines.Add(String.Format(S_DOC_HEADER, title));
            lines.Add(String.Format(S_LINK_TO_SOM_PAGE, title));

            String extraText = checkForExtraText();
            if (extraText.Length > 0)
                lines.Add(string.Format(S_HTML_EXTRA_TEXT, extraText));
        }

        private string checkForExtraText()
        {
            String result = "";
            try {
                result = System.IO.File.ReadAllText(@"updatedText.txt");
            }
            catch(Exception ex) { }

            return result;
        }

        public void setOutputFooter()
        {
            lines.Add("</body></html>\r\n");
            System.IO.File.WriteAllLines(@"index.html", lines);
        }

        public void draftOrderHeader()
        {
            lines.Add(String.Format("<h3>DRAFT PICK ORDER</h3>"));
        }

        public void draftOrderTableHeader()
        {
            lines.Add("<table style='margin-left:50px;' border=0 cellspacing=0 cellpadding=0 style='border-collapse:collapse;border:none'><tr>");
            addTableCell("PICK#", "#5B9BD5", "#F8CBAD", 48);
            addTableCell("TEAM", "#5B9BD5", "#F8CBAD", 149);
            addTableCell("OWNER", "#5B9BD5", "#F8CBAD", 65);
            addTableCell("WON", "#5B9BD5", "#F8CBAD", 48);
            addTableCell("LOST", "#5B9BD5", "#F8CBAD", 48);
            addTableCell("PCT.", "#5B9BD5", "#F8CBAD", 53);
            addTableCell("PYG", "#5B9BD5", "#F8CBAD", 53);
            lines.Add("</tr>");
        }

        public void draftOrderTeamLine(int pickNum, Team team)
        {
            string bgColor = getBackgroundColor(pickNum);
            lines.Add("<table style='margin-left:50px;' border=0 cellspacing=0 cellpadding=0 style='border-collapse:collapse;border:none'><tr>");
 
            addTableCell(pickNum, "#5B9BD5", returnRankDifColor(pickNum, team.DraftPickPositionPrevious), 
                         48, true, returnRankDifToolTip(pickNum,team.DraftPickPositionPrevious));
            addTableCell(team.Name, "#5B9BD5", "#FFFFFF", 149, false);
            addTableCell(team.Owner, bgColor, "#000000", 65);
            addTableCell(team.Wins, bgColor, "#000000", 48);
            addTableCell(team.Loses, bgColor, "#000000", 48);
            addTableCell(team.Wpct, 3, bgColor, "#000000", 53);
            addTableCell(team.PythagoreanTheorem, 3, bgColor, "#000000", 53);
            lines.Add("</tr>");
        }

        private string returnRankDifColor(int rank, int previous)
        {
            int rankDif = rank - previous;
            if (rankDif < 0)
                return "#FF0000";
            else if (rankDif > 0)
                return "#00FF00";
            else
                return "#FFFFFF";
        }

        private string returnRankDifToolTip(int rank, int previous)
        {
            int rankDif = rank - previous;
            if (rankDif == -1)
                return String.Format("Dropped 1 spot", rankDif);
            else if( rankDif < 0)
                return String.Format("Dropped {0} spots", rankDif*-1);
            else if (rankDif == 1)
                return String.Format("Gained 1 spot", rankDif);
            else if (rankDif > 0)
                return String.Format("Gained {0} spots", rankDif);
            return "No Change";
        }

        private string returnGBDifToolTip( Team team) {
            double dif = team.Gb - team.GbPrevious;
            if (dif < 0)
                return String.Format("Gained {0} games", dif * -1);
            else if (dif > 0)
                return String.Format("Dropped {0} games", dif);
            return "No Change";
        }


        private string returnRecentRecordsTip( Team team ) {
            int gamesToReport = (team.Wins-team.WinsPrevious) + (team.Loses- team.LosesPrevious);
            List<Game> games = team.GetLastGames(gamesToReport);
            List<Pair> allSeries = new List<Pair>();
            Pair currentSeries = null;
            foreach(Game game in games) {
                if (currentSeries == null) { 
                    currentSeries = new Pair(game.DisplayOpponent);
                    allSeries.Add(currentSeries);
                }
                else if( !currentSeries.MetaData().Equals(game.DisplayOpponent)) {
                    currentSeries = new Pair(game.DisplayOpponent);
                    allSeries.Add(currentSeries);
                }

                if (game.Won)
                    currentSeries.AddWin();
                else
                    currentSeries.AddLoss();
            }

            StringBuilder sb = new StringBuilder();
            for( int i= allSeries.Count-1; i>=0; i--) { 
      //      foreach(Pair series in allSeries) {
                sb.Append(allSeries[i].ToString());
                sb.Append("<br>");
            }

            return sb.ToString();
        }

        public void spacer()
        {
            lines.Add("<p><b><span style='color:white'>&nbsp;</span></b></p>");
        }

        public void wildCardHeader(string league)
        {
            lines.Add(String.Format("<h3>{0} WILD CARD STANDINGS</h3>", league));
        }

        public void wildCardTableHeader()
        {
            lines.Add("<table style='margin-left:50px;' border=0 cellspacing=0 cellpadding=0 style='border-collapse:collapse;border:none'><tr>");
            addTableCell("#", "#339966", "#F8CBAD", 15);
            addTableCell("TEAM", "#339966", "#F8CBAD", 149);
            addTableCell("WON", "#339966", "#F8CBAD", 48);
            addTableCell("LOST", "#339966", "#F8CBAD", 48);
            addTableCell("PCT.", "#339966", "#F8CBAD", 53);
            addTableCell("GB", "#339966", "#F8CBAD", 53);
            addTableCell("LAST", "#339966", "#F8CBAD", 53);
            addTableCell("", "#339966", "#F8CBAD", 10);
            lines.Add("</tr>");
        }

        public void wildCardTeamLine(int rank, Team team, string gamesBehind)
        {
            string bgColor = getBackgroundColor(rank, true);
            lines.Add("<table style='margin-left:50px;' border=0 cellspacing=0 cellpadding=0 style='border-collapse:collapse;border:none'><tr>");
            addTableCell(rank, "#339966", "#FFFFFF", 15);
            addTableCell(team.Name, "#339966", "#FFFFFF", 149, false);
            addTableCell(team.Wins, bgColor, "#000000", 48);
            addTableCell(team.Loses, bgColor, "#000000", 48);
            addTableCell(team.Wpct, 3, bgColor, "#000000", 53);
            addTableCell(gamesBehind, bgColor, "#000000", 53);
            addTableCell(team.RecordLastRun, bgColor, "#000000", 53);
            addTableCell("", "#F8CBAD", "#F8CBAD", 10);
            lines.Add("</tr>");
        }

        public void divisionStandingsHeader(string division)
        {
            lines.Add(String.Format("<h3>{0} STANDINGS</h3>", division)); 
        }

        public void divisionStandingsTableHeader()
        {
            lines.Add("<table style='margin-left:50px;' border=0 cellspacing=0 cellpadding=0 style='border-collapse:collapse;border:none'><tr>");
            addTableCell("#", "#5B9BD5", "#F8CBAD", 15);
            addTableCell("TEAM", "#5B9BD5", "#F8CBAD", 149);
            addTableCell("WON", "#5B9BD5", "#F8CBAD", 48);
            addTableCell("LOST", "#5B9BD5", "#F8CBAD", 48);
            addTableCell("PCT.", "#5B9BD5", "#F8CBAD", 53);
            addTableCell("GB", "#5B9BD5", "#F8CBAD", 53);
            addTableCell("LAST", "#5B9BD5", "#F8CBAD", 53);
            addTableCell("", "#F8CBAD", "#F8CBAD", 10);
            addTableCell("PThry", "#5B9BD5", "#F8CBAD", 48);
            addTableCell("AVG", "#5B9BD5", "#F8CBAD", 48);
            addTableCell("HR", "#5B9BD5", "#F8CBAD", 48);
            addTableCell("ERA", "#5B9BD5", "#F8CBAD", 48);
            lines.Add("</tr>");
        }

        public void divisionStandingsTeamLine(int rank, Team team)
        {
            string bgColor = getBackgroundColor(rank);
            lines.Add("<table style='margin-left:50px;' border=0 cellspacing=0 cellpadding=0 style='border-collapse:collapse;border:none'><tr>");
            addTableCell(rank, "#5B9BD5", returnRankDifColor(rank, team.DivisionPositionPrevious), 
                         15, true, returnRankDifToolTip(rank, team.DivisionPositionPrevious));
            addTableCell(team.Name, "#5B9BD5", "#FFFFFF", 149, false);
            addTableCell(team.Wins, bgColor, "#000000", 48);
            addTableCell(team.Loses, bgColor, "#000000", 48);
            addTableCell(team.Wpct, 3, bgColor, "#000000", 53);
            if( team.Gb == 0)
                addTableCell("--", bgColor, "#000000", 53, true, returnGBDifToolTip(team));
            else
                addTableCell(team.Gb, 1, bgColor, "#000000", 53, true, returnGBDifToolTip(team));
            addTableCell(team.RecordLastRun, bgColor, "#000000", 53, true, returnRecentRecordsTip(team));

            addTableCell("", "#F8CBAD", "#F8CBAD", 10);

            addTableCell(team.PythagoreanTheorem, 3, bgColor, setPythagoreanTheoremColor(team), 48);
            addTableCell(team.BattingAverage, 3, bgColor, getTeamBattingAverageColor(team), 48);
            addTableCell(team.HomeRuns, 0, bgColor, getTeamHomeRunsColor(team), 48);
            addTableCell(team.EarnedRunAvg, 2, bgColor, getTeamERAColor(team), 48);
            lines.Add("</tr>");
        }

        private string getTeamBattingAverageColor(Team team) {
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
            
            if(rank % 2 == 0)
            {
                return (bWildCardColors) ? "ccffcc" : "#BDD6EE";
            }
            else {
                return (bWildCardColors) ? "b3ffb3" : "#DEEAF6";
            }
        }

        private string setPythagoreanTheoremColor(Team team) {
            double py = team.PythagoreanTheorem;
            double wpct = team.Wpct;
            double diff = (wpct - py)*1000;
            if (diff > -10.5 && diff < 10.5)
                return "#5E5E5E";
            else if (diff < 0)
                return "#11811C";
            else
                return "#811111";
        
        }


        private void addTableCell(string text, string bgColor, string textColor, int width, bool center = true, string tooltip = "")
        {
            if (tooltip.Length > 0)
                lines.Add(String.Format(S_TABLE_HEADER_CELL_TOOLTIP, width, bgColor, center ? "center" : "left", textColor, text, tooltip));
            else
                lines.Add(String.Format(S_TABLE_HEADER_CELL, width, bgColor, center ? "center" : "left", textColor, text));

        }

        private void addTableCell(double number, int precision, string bgColor, string textColor, int width, bool center = true, string tooltip = "")
        {
            switch(precision) {
                case 0:
                    addTableCell(String.Format("{0}", number), bgColor, textColor, width, center, tooltip);
                    break;
                case 1:
                    addTableCell(String.Format("{0:0.0}", number), bgColor, textColor, width, center, tooltip);
                    break;
                case 2:
                    addTableCell(String.Format("{0:0.00}", number), bgColor, textColor, width, center, tooltip);
                    break;
                default:
                    addTableCell(String.Format("{0:.000}", number), bgColor, textColor, width, center, tooltip);
                    break;
            }
        }

        private void addTableCell(int number, string bgColor, string textColor, int width, bool center = true, string tooltip = "")
        {
            addTableCell(number.ToString(), bgColor, textColor, width, center, tooltip);
        }

        private string getToolTipScript()
        {
            string scriptLine1 = ".tooltip {position: relative;display: inline-block;border-bottom: 1px dotted black;}\r\n";
            string scriptLine2 = ".tooltip .tooltiptext {visibility: hidden;width: 120px;background-color: #555;color: #fff;text-align: center;border-radius: 6px;padding: 5px 0;position: absolute;z-index: 1;bottom: 125%;left: 50%;margin-left: -60px;opacity: 0;transition: opacity 1s;}\r\n";
            string scriptLine3 = ".tooltip .tooltiptext::after {content: \"\";position: absolute;top: 100%;left: 50%;margin-left: -5px;border-width: 5px;border-style: solid;border-color: #555 transparent transparent transparent;}\r\n";
            string scriptLine4 = ".tooltip:hover .tooltiptext {visibility: visible;opacity: 1;}\r\n";

            return scriptLine1 + scriptLine2 + scriptLine3 + scriptLine4;

        }

        public void endOfTable()
        {
            lines.Add("</table>");
        }

        public void ShowWhosHotData(string v)
        {
            lines.Add("<br/><pre>"+v+"</pre>");
        }
    }
}
