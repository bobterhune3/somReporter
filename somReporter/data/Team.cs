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
                return (double)Wins / ((double)Wins + (double)Loses);
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

        public string Division
        {
            get
            {
                return division;
            }
        }

        public Team(String div) {
           league = div.Substring(0,2);
           division = div.Substring(3);
           full_div = div;
         
        }


    }
}
