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
        private const string S_TABLE_HEADER_CELL = "<td width={0} style='border:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt;background:{1};height:13.8pt'><p align={2}><span style='color:{3}'>&nbsp;{4}</span></p></td>";
        private const string S_TABLE_HEADER_CELL_TOOLTIP = "<td width={0} style='border:solid windowtext 1.0pt; border-right:solid windowtext 1.0pt;background:{1};height:13.8pt'><p align={2}><span class='tooltip' style='color:{3}'>&nbsp;{4}<span class='tooltiptext'>{5}</span></span></p></td>";

        List<String> lines = new List<String>();

        public HTMLOutput() {
        }


        public void setOutputHeader(string title)
        {
            lines.Add(String.Format(S_DOC_HTML_TITLE, title, getToolTipScript()));
            lines.Add(String.Format(S_DOC_HEADER, title));
            lines.Add(String.Format(S_LINK_TO_SOM_PAGE, title));
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
            addHeaderCell("PICK#", "#5B9BD5", "#F8CBAD", 48);
            addHeaderCell("TEAM", "#5B9BD5", "#F8CBAD", 149);
            addHeaderCell("WON", "#5B9BD5", "#F8CBAD", 48);
            addHeaderCell("LOST", "#5B9BD5", "#F8CBAD", 48);
            addHeaderCell("PCT.", "#5B9BD5", "#F8CBAD", 53);
            addHeaderCell("PYG", "#5B9BD5", "#F8CBAD", 53);
            lines.Add("</tr>");
        }

        public void draftOrderTeamLine(int pickNum, Team team)
        {
            string bgColor = getBackgroundColor(pickNum);
            lines.Add("<table style='margin-left:50px;' border=0 cellspacing=0 cellpadding=0 style='border-collapse:collapse;border:none'><tr>");
 
            addHeaderCell(pickNum, "#5B9BD5", "#FFFFFF", 48, true, returnRankDifToolTip(pickNum,team.DraftPickPositionPrevious));
            addHeaderCell(team.Name, "#5B9BD5", "#FFFFFF", 149, false);
            addHeaderCell(team.Wins, bgColor, "#000000", 48);
            addHeaderCell(team.Loses, bgColor, "#000000", 48);
            addHeaderCell(team.Wpct, bgColor, "#000000", 53);
            addHeaderCell(team.PythagoreanTheorem, bgColor, "#000000", 53, false);
            lines.Add("</tr>");
        }

        private String returnRankDifToolTip(int rank, int previous)
        {
            int rankDif = rank - previous;
            if (rankDif == -1)
                return String.Format("Dropped 1 spot", rankDif);
            else if( rankDif < 0)
                return String.Format("Dropped {0} spots", rankDif*-1);
            else if (rankDif == 1)
                return String.Format("Gained 1 spot", rankDif);
            else if (rankDif > 0)
                return String.Format("Gained {0} spots", rankDif * -1);
            return "No Change";
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
            addHeaderCell("#", "#5B9BD5", "#F8CBAD", 15);
            addHeaderCell("AL EAST", "#5B9BD5", "#F8CBAD", 149);
            addHeaderCell("WON", "#5B9BD5", "#F8CBAD", 48);
            addHeaderCell("LOST", "#5B9BD5", "#F8CBAD", 48);
            addHeaderCell("PCT.", "#5B9BD5", "#F8CBAD", 53);
            addHeaderCell("GB", "#5B9BD5", "#F8CBAD", 53);
            addHeaderCell("LAST", "#5B9BD5", "#F8CBAD", 53);
            addHeaderCell("", "#F8CBAD", "#F8CBAD", 10);
            lines.Add("</tr>");
        }

        public void wildCardTeamLine(int rank, Team team, string gamesBehind)
        {
            string bgColor = getBackgroundColor(rank);
            lines.Add("<table style='margin-left:50px;' border=0 cellspacing=0 cellpadding=0 style='border-collapse:collapse;border:none'><tr>");
            addHeaderCell(rank, "#5B9BD5", "#FFFFFF", 15);
            addHeaderCell(team.Name, "#5B9BD5", "#FFFFFF", 149, false);
            addHeaderCell(team.Wins, bgColor, "#000000", 48);
            addHeaderCell(team.Loses, bgColor, "#000000", 48);
            addHeaderCell(team.Wpct, bgColor, "#000000", 53);
            addHeaderCell(gamesBehind, bgColor, "#000000", 53);
            addHeaderCell(team.RecordLastRun, bgColor, "#000000", 53);
            addHeaderCell("", "#F8CBAD", "#F8CBAD", 10);
            lines.Add("</tr>");
        }

        private string getBackgroundColor(int rank)
        {
          return (rank % 2 == 0) ? "#BDD6EE" : "#DEEAF6";
        }

        private void addHeaderCell(string text, string bgColor, string textColor, int width, bool center = true, string tooltip = "")
        {
            if(tooltip.Length > 0)
                lines.Add(String.Format(S_TABLE_HEADER_CELL_TOOLTIP, width, bgColor, center ? "center" : "left", textColor, text, tooltip));
            else
                lines.Add(String.Format(S_TABLE_HEADER_CELL, width, bgColor, center? "center" : "left",textColor, text));
            
        }

        private void addHeaderCell(double number, string bgColor, string textColor, int width, bool center = true, string tooltip = "")
        {
            addHeaderCell(number.ToString(), bgColor, textColor, width, center, tooltip);
        }

        private void addHeaderCell(int number, string bgColor, string textColor, int width, bool center = true, string tooltip = "")
        {
            addHeaderCell(number.ToString(), bgColor, textColor, width, center, tooltip);
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
    }
}
