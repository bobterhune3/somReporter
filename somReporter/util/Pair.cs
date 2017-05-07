using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace somReporter.util
{
    public class Pair
    {
        int win =0;
        int lost=0;
        string metadata = "";

        public Pair(string metaData) {
            this.metadata = metaData;
        }

        public Pair(string metaData, int x, int y) {
            this.metadata = metaData;
            this.win = x;
            this.lost = y;
        }

        public int Wins {
            get { return win; }
            set { this.win = value; }
        }

        public int Lost {
            get { return lost; }
            set { this.lost = value; }
        }

        public void AddWin() { win++; }
        public void AddLoss() { lost++; }
        public string MetaData() { return metadata; }

        public override string ToString() {
            return String.Format("{0} {1}-{2}", metadata, win, lost);
        }
    }
}
