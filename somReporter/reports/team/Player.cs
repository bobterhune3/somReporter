using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace somReporter.team
{
    public class Player
    {
        private string name = "";
        private int actual = 0;
        private int replay = 0;
        private int targetUsage = 0;
        private Team team;
        private bool hitter = false;

        public Player( ) { }

        public string Name {
            get { return name; }
            set { name = value; }
        }

        public int Actual
        {
            get { return actual; }
            set { actual = value; }
        }

        public int Replay
        {
            get { return replay; }
            set { replay = value; }
        }

        public int TargetUsage
        {
            get { return targetUsage; }
            set { targetUsage = value; }
        }

        public Team Team
        {
            get { return team; }
            set { team = value; }
        }

        public bool IsHitter
        {
            get { return hitter; }
            set { hitter = value; }
        }

        public double Usage
        {
            get
            {
                if (Actual == 0)
                    Actual = 1;
                return Report.RoundToSignificantDigits((double)Replay / (double)Actual, 3);
            }
        }
    }
}
