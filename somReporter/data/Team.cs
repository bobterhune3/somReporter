using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace somReporter
{
    public class Team 
    {
        private String name = "";
        private String abrv = "";
        private int wins = 0;
        private int loses = 0;
        private double wpct = .0;
        private double gb = .0;
        private String league = "";
        private String division = "";
        private String full_div = "";
        private String owner = "";
        private int runsScored = 0;
        private double average = .0;
        private double era = .0;
        private double ip = .0;
        private int runsAllowed = 0;
        private double pythagoreanTheorem = .0;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Abrv
        {
            get
            {
                return abrv;
            }

            set
            {
                abrv = value;
                owner = abrv.Substring(2,1);
            }
        }

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
        public int Wins
        {
            get
            {
                return wins;
            }

            set
            {
                wins = value;
            }
        }

        public int Loses
        {
            get
            {
                return loses;
            }

            set
            {
                loses = value;
            }
        }

        public double Wpct
        {
            get
            {
              //  return wpct;
                return Report.RoundToSignificantDigits((double)Wins / ((double)Wins + (double)Loses),3);
            }

 
        }

         public double Gb
        {
            get
            {
                return gb;
            }

            set
            {
                gb = value;
            }
        }

        public string Full_div
        {
            get
            {
                return full_div;
            }
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

        private void calculatePythagoreanTheorem() {

            if( runsAllowed > 0 && runsScored > 0) {
                pythagoreanTheorem = (Math.Pow((double)runsScored,2) / 
                                    (Math.Pow((double)runsScored,2) + Math.Pow((double)runsAllowed,2)));
            }
        }

        public Team(String div) {
           league = div.Substring(0,2);
           division = div.Substring(3);
           full_div = div;
        }
    }
}
