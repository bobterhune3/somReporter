using System;
using System.Collections.Generic;

namespace somReporter
{
    public class Team 
    {
        private const String BASELINE_TEAM = "DTB";
        private String name = "";
        private String abrv = "";
        private int wins = 0;
        private int loses = 0;
        private int winsPrevious = 0;
        private int losesPrevious = 0;
        private double gb = .0;
        private double gbPrevious = .0;
        private String league = "";
        private String division = "";
        private String full_div = "";
        private String owner = "";
        private int runsScored = 0;
        private double average = .0;
        private double era = .0;
        private double ip = .0;
        private double homeruns = 0;
        private double homerunsAllowed = 0;
        private int runsAllowed = 0;
        private double pythagoreanTheorem = .0;
        private int divisionPositionCurrent = -1;
        private int draftPickPositionCurrent = -1;
        private int divisionPositionPrevious = -1;
        private int draftPickPositionPrevious = -1;
        private int wildCardPositionCurrent = -1;
        private int wildCardPositionPrevious = -1;
        private List<double> wpctHistory;
        private int winPctHistoryGameCount = 0;

        public static int TOTAL_GAMES = 0;

        List<Game> linescores = new List<Game>();

        Dictionary<CATEGORY, Boolean> hLeaders = new Dictionary<CATEGORY, Boolean>();
        Dictionary<CATEGORY, Boolean> hTrailing = new Dictionary<CATEGORY, Boolean>();

        public enum CATEGORY { BATTING_AVERAGE, EARNED_RUNS_AVG, HOME_RUNS, NONE };

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Abrv
        {
            get { return abrv; }
            set {
                abrv = value;
                owner = abrv.Substring(2,1);
            }
        }


        public override bool Equals(Object obj) {
            return ((Team)obj).Abrv.Equals(Abrv);
        }

        public override int GetHashCode( )
        {
            return 808;
        }

        internal void addLineScore(int teama_score, string teamb_abv, int teamb_score, bool homeTeam)
        {
           linescores.Insert(0, new Game(teama_score, teamb_abv, teamb_score, homeTeam));
        }

        public List<Game> LineScores { get { return linescores; } }

        public string Owner
        {
            get
            {
                return owner;
            }

            set
            {
                owner = value;
            }
        }

        public int GamesPlayed {
            get { return Wins + Loses; }
        }

        public int Wins {
            get { return wins; }
            set { wins = value; }
        }

        public int WinsPrevious
        {
            get { return winsPrevious; }
            set { winsPrevious = value; }
        }

        public int Loses {
            get { return loses; }
            set {
                loses = value;
                if (Abrv.Equals(BASELINE_TEAM))
                    TOTAL_GAMES = Wins + loses;
            }
        }

        public int LosesPrevious
        {
            get { return losesPrevious; }
            set { losesPrevious = value; }
        }

        public double Wpct
        {
            get
            {
              //  return wpct;
                return Report.RoundToSignificantDigits((double)Wins / ((double)Wins + (double)Loses),3);
            }

 
        }

        public double Gb {
            get { return gb; }
            set { gb = value;  }
        }

        public double GbPrevious
        {
            get { return gbPrevious; }
            set { gbPrevious = value; }
        }

        public string Full_div
        {
            get
            {
                return full_div;
            }
        }

        public int WinPctHistoryGameCount {
            get { return winPctHistoryGameCount; }
            set { winPctHistoryGameCount = value; }
        }

        public string League
        {
            get
            {
                return league;
            }
        }

        public double PythagoreanTheorem
        {
            get {
                return Report.RoundToSignificantDigits(pythagoreanTheorem,3);
            }
        }

        public string Division
        {
            get
            {
                return division;
            }
        }

        public int RunsScored
        {
            get
            {
                return runsScored;
            }

            set
            {
                runsScored = value;
                calculatePythagoreanTheorem();
            }
        }

        public double Average
        {
            get
            {
                return average;
            }

            set
            {
                average = value;
            }
        }

        public double ERA
        {
            get
            {
                return era;
            }

            set
            {
                era = value;
            }
        }


        public double IP
        {
            get
            {
                return ip;
            }

            set
            {
                ip = value;
            }
        }

        public int RunsAllowed
        {
            get
            {
                return runsAllowed;
            }

            set
            {
                runsAllowed = value;
                calculatePythagoreanTheorem();
            }
        }

        public int DivisionPositionCurrent
        {
            get { return divisionPositionCurrent; }
            set { divisionPositionCurrent = value; }
        }

        public List<Game> GetLastGames(int gamesToReport)
        {
            return linescores.GetRange(0, gamesToReport);
        }

        public int DraftPickPositionCurrent
        {
            get { return draftPickPositionCurrent; }
            set { draftPickPositionCurrent = value; }
        }
        public int DivisionPositionPrevious
        {
            get { return divisionPositionPrevious; }
            set { divisionPositionPrevious  = value; }
        }
        public int DraftPickPositionPrevious
        {
            get { return draftPickPositionPrevious; }
            set { draftPickPositionPrevious  = value; }
        }
        public int WildCardPositionPrevious
        {
            get { return wildCardPositionPrevious; }
            set { wildCardPositionPrevious = value; }
        }
        public int WildCardPositionCurrent
        {
            get { return wildCardPositionCurrent; }
            set { wildCardPositionCurrent = value; }
        }

        private void calculatePythagoreanTheorem() {

            if( runsAllowed > 0 && runsScored > 0) {
                pythagoreanTheorem = (Math.Pow((double)runsScored,2) / 
                                    (Math.Pow((double)runsScored,2) + Math.Pow((double)runsAllowed,2)));
            }
        }

        public double calculateGamesBehind(Team leader) {
            double a1 = ((double)leader.Wins - (double)this.Wins);
            double a2 = ((double)this.Loses - (double)leader.Loses);
            return (a1 + a2) / 2.0;
        }

        public Team(String div) {
            if (Program.LEAGUES[0].Length > 0) { 
                league = div.Substring(0, 2);
                // This fixes a typo in the league
                if (league.Equals("NE"))
                    league = "NL";
                division = div.Substring(3);
            }
            else { 
                league = "";
                division = div;
            }


            full_div = div;
        }

        public string buildStorageData()
        {

            String data = String.Format("Wins={0}|Loses={1}|GB={2}|DIVPos={3}|DPickPos={4}|WCardPos={5}",
                Wins, Loses, this.Gb, this.DivisionPositionCurrent, this.DraftPickPositionCurrent,
                this.WildCardPositionCurrent);
            return data;
        }

        public string RecordLastRun {
            get {
                int wins = this.Wins - this.WinsPrevious;
                int loses = this.Loses - this.LosesPrevious;
                return String.Format("{0}-{1}", wins, loses);
            }
        }

        public double BattingAverage { get { return average; }
                                       set { this.average = value; } }
        public double HomeRuns { get { return homeruns; }
                                 set { this.homeruns = value; }}
        public double HomeRunsAllowed
        {
            get { return homerunsAllowed; }
            set { this.homerunsAllowed = value; }
        }
        
        public double EarnedRunAvg { get { return era; }
                                     set { this.era = value; } }

        public bool getLeader( CATEGORY cat ) {
            bool bResult = false;
            if (hLeaders.Count == 0) return bResult;
            hLeaders.TryGetValue(cat, out bResult);
            return bResult;
        }

        public bool getTrailing(CATEGORY cat) {
            bool bResult = false;
            if (hTrailing.Count == 0) return bResult;
            hTrailing.TryGetValue(cat, out bResult);
            return bResult;             
        }

        public void setLeader(CATEGORY cat ) {
            hLeaders.Add(cat, true);

        }

        public void setTrailing(CATEGORY cat) {
            hTrailing.Add(cat, true);
        }

        public List<double> WinPctHistoryData
        {
            get { return wpctHistory; }
            set { wpctHistory = new List<double>(value); }
        }

        public void addWinPctHistoryData(double value)
        {
            if(wpctHistory == null )
                wpctHistory = new List<double>();
            wpctHistory.Add(value);
        }

        public static string prettyTeamName(string teamName)
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


    public class Game {
        int scoreus;
        String abv;
        int scorethem;
        bool homeTeam;

        public Game(int scoreus, String abv, int scorethem, bool homeTeam) {
            this.scoreus= scoreus;
            this.abv = abv;
            this.scorethem = scorethem;
            this.homeTeam = homeTeam;
        }

        public bool Won { get { return scoreus > scorethem; } }
        public string Opponent { get { return abv; } }

        public bool HomeTeam { get { return homeTeam; } }

        public string DisplayOpponent {
            get {
                return (homeTeam ? "" : "@") + Opponent;
            }
        }
    }
}
