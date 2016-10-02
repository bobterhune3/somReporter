﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace somReporter.output
{
    interface IOutput
    {
        void draftOrderHeader();
        void draftOrderTableHeader();
        void draftOrderTeamLine(int pickNum, Team team);
        void wildCardHeader(string league);
        void wildCardTableHeader();
        void wildCardTeamLine(int rank, Team team, string gamesBehind);
        void spacer();
    }
}