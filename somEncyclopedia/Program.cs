using somEncyclopedia.team;
using somEncylopedia.features;
using somReporter;
using somReporter.features;
using somReporter.util.somReporter;
using System;
using System.Collections.Generic;
using System.Text;

namespace somEncyclopedia
{
    class Program
    {
        private SOMReportFile teamReportFile;

        public static IFeature featurePrimary = null;
        public static IFeature featureSecondary = null;

        public Program()
        {

        }

        static void Main(string[] args)
        {
            Program program = new Program();
            Console.WriteLine("Intializing...");
            program.initialize();
        }

        public void initialize()
        {
            Report.DATABASE.reset();

            Console.WriteLine("  Loading League Report File ...");
            teamReportFile = new SOMReportFile(Config.getConfigurationFile("TEAM_ALL_REPORTS.PRT"));
            teamReportFile.parseTeamFile();

            featurePrimary = FeatureFactory.loadFeature(FeatureFactory.FEATURE.PRIMARY);
            featurePrimary.initialize(teamReportFile);
            List<PrimaryReport> primaryReports = ((FeaturePrimary)featurePrimary).getReports();

            featureSecondary = FeatureFactory.loadFeature(FeatureFactory.FEATURE.SECONDARY);
            featureSecondary.initialize(teamReportFile);
            List<SecondaryReport> secondaryReports = ((FeatureSecondary)featureSecondary).getReports();

            buildOutBatterEncycloCSV(primaryReports, secondaryReports);
            buildOutPitcherEncycloCSV(primaryReports);
        }

        private void buildOutBatterEncycloCSV(List<PrimaryReport> featurePrimary, List<SecondaryReport> featureSecondary)
        {
            List<String> lines = new List<String>();
            lines.Add("F,L,B,Yr,Tm,RepTm,Inj,G,AB,H,2B,3B,HR,RBI,R,BB,IBB,K,SF,SH,SAt,SB,CS,E,HP,GDP,GS,Stk,LStk,CI,PH,PAB,PHR,LAB,LH,L2B,L3B,LHR,LRBI,LBB,LIBB,LK,LPH,LPAB,LPHR,RAB,RH,R2B,R3B,RHR,RRBI,RBB,RIBB,RK,RPH,RPAB,RPHR,HRHm,HRAw,XTC,XTO,XPC,XPO,Pos,PB,OSB,OCS,TE");

            for (int i = 0; i < featurePrimary.Count; i++)
            {
                List<EncPlayer> primPlayers = featurePrimary[i].getPlayers();
                List<EncPlayer> secPlayers = featureSecondary[i].getPlayers();

                foreach (EncPlayer player in primPlayers)
                {
                    EncPlayer secPlayer = findSecondaryMatchingPlayer(player, secPlayers);
                    if (secPlayer == null)
                    {
                        System.Console.WriteLine("Error, Secondary player object not found in list");
                        return;
                    }
                    if(player.Pos > 1)
                        lines.Add(buildBatterline(player, secPlayer));
                }
            }

            System.IO.StreamWriter file = null;
            try
            {

                // Read the file and display it line by line.
                file = new System.IO.StreamWriter("battersEncyclopedia.csv");
                foreach(String line in lines)
                {
                    file.WriteLine(line);
                }
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }

        private void buildOutPitcherEncycloCSV(List<PrimaryReport> featurePrimary)
        {
            List<String> lines = new List<String>();

            lines.Add("F,L,T,Yr,Tm,RepTm,Inj,G,GS,CG,GF,W,L,IP,Thirds,R,ER,Sho,Sv,SvOp,IR,IRSP,IRS,OSB,OCS,Bk,WP,RnSup,AB,H,HR,BB,IBB,K,OtherBFP,LAB,LH,LHR,LBB,LIBB,LK,LOtherBFP,RAB,RH,RHR,RBB,RIBB,RK,ROtherBFP,HRHm,HRAw,PAB,PH,PHR,PSac,XC,XO,E");

            for (int i = 0; i < featurePrimary.Count; i++)
            {
                List<EncPlayer> primPlayers = featurePrimary[i].getPlayers();

                foreach (EncPlayer player in primPlayers)
                {
                    if( player.Pos == 1)
                    lines.Add(buildPitcherline(player));
                }
            }

            System.IO.StreamWriter file = null;
            try
            {

                // Read the file and display it line by line.
                file = new System.IO.StreamWriter("pitchersEncyclopedia.csv");
                foreach (String line in lines)
                {
                    file.WriteLine(line);
                }
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }

        private EncPlayer findSecondaryMatchingPlayer(EncPlayer player, List<EncPlayer> secPlayers)
        {
            foreach (EncPlayer sec in secPlayers)
            {
                if (sec.Name.Equals(player.Name) && sec.FName.Equals(player.FName))
                    return sec;
            }
            return null;
        }

        private String buildPitcherline(EncPlayer p)
        {
//            lines.Add("F,L,T,Yr,Tm,RepTm,Inj,G,GS,CG,GF,W,L,IP,Thirds,R,ER,Sho,Sv,SvOp,IR,IRSP,IRS,OSB,OCS,Bk,WP,RnSup,AB,H,HR,BB,IBB,K,OtherBFP,LAB,LH,LHR,LBB,LIBB,LK,LOtherBFP,RAB,RH,RHR,RBB,RIBB,RK,ROtherBFP,HRHm,HRAw,PAB,PH,PHR,PSac,XC,XO,E");
            StringBuilder sb = new StringBuilder();

            sb.Append(p.FName + ",");
            sb.Append(p.Name + ",");
            sb.Append("?,");
            sb.Append(p.YR + ",");
            if (p.Team != null)
            {
                sb.Append(p.Team.Abrv + ",");
                sb.Append(p.Team.Abrv + ",");
                sb.Append("0,");
                sb.Append(p.G + ",");
                sb.Append(p.GS + ",");
                sb.Append(p.CG + ",");
                sb.Append("0,");
                sb.Append(p.W + ",");
                sb.Append(p.L + ",");
                sb.Append(p.IP + ",");
                sb.Append(p.Thirds + ",");
                sb.Append(p.R + ",");
                sb.Append(p.ER + ",");
                sb.Append(p.SH + ",");
                sb.Append(p.SV + ",");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append(p.AB + ",");
                sb.Append(p.H + ",");
                sb.Append(p.HR + ",");
                sb.Append(p.BB + ",");
                sb.Append(p.IBB + ",");
                sb.Append(p.K + ",");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
                sb.Append("0,");
            }
            else
            {
                sb.Append("XXX,XXX,");
            }


            return sb.ToString();
        }

        private String buildBatterline(EncPlayer p1, EncPlayer p2)
        {
//            lines.Add("F,L,B,Yr,Tm,RepTm,Inj,G,AB,H,2B,3B,HR,RBI,R,BB,IBB,K,SF,SH,SAt,SB,CS,E,HP,GDP,GS,Stk,LStk,CI,PH,PAB,PHR,LAB,LH,L2B,L3B,LHR,LRBI,LBB,LIBB,LK,LPH,LPAB,LPHR,RAB,RH,R2B,R3B,RHR,RRBI,RBB,RIBB,RK,RPH,RPAB,RPHR,HRHm,HRAw,XTC,XTO,XPC,XPO,Pos,PB,OSB,OCS,TE");

            StringBuilder sb = new StringBuilder();

            sb.Append(p1.FName+",");
            sb.Append(p1.Name + ",");
            sb.Append("?,");
            sb.Append(p1.YR + ",");
            if (p1.Team != null)
            {
                sb.Append(p1.Team.Abrv + ",");
                sb.Append(p1.Team.Abrv + ",");
            }
            else
            {
                sb.Append("XXX,XXX,");
            }
            sb.Append("0,");
            sb.Append(p1.G + ",");
            sb.Append(p1.AB + ",");
            sb.Append(p1.H + ",");
            sb.Append(p1.T2B + ",");
            sb.Append(p1.T3B + ",");
            sb.Append(p1.HR + ",");
            sb.Append(p1.RBI + ",");
            sb.Append(p1.R + ",");
            sb.Append(p1.BB + ",");
            sb.Append(p1.IBB + ",");
            sb.Append(p1.K + ",");
            sb.Append(p2.SF + ",");
            sb.Append(p1.SH + ",");
            sb.Append(p2.SAt + ",");
            sb.Append(p1.SB + ",");
            sb.Append(p1.CS + ",");
            sb.Append(p1.E + ",");
            sb.Append(p1.HP + ",");
            sb.Append(p1.GDP + ",");
            sb.Append(p1.GS + ",");
            sb.Append(p2.Stk + ",");
            sb.Append(p2.LStk + ",");
            sb.Append(p2.CI + ",");
            sb.Append(p2.PH + ",");
            sb.Append(p2.PAB + ",");
            sb.Append(p2.PHR + ",");
            sb.Append(p1.LAB + ",");
            sb.Append(p1.LH + ",");
            sb.Append(p1.L2B + ",");
            sb.Append(p1.L3B + ",");
            sb.Append(p1.LHR + ",");
            sb.Append(p1.LRBI + ",");
            sb.Append(p1.LBB + ",");
            sb.Append(p1.LIBB + ",");
            sb.Append(p1.LK + ",");
            sb.Append(p1.LPAB + ",");
            sb.Append(p1.LPHR + ",");
            sb.Append(p1.RAB + ",");
            sb.Append(p1.RH + ",");
            sb.Append(p1.R2B + ",");
            sb.Append(p1.R3B + ",");
            sb.Append(p1.RHR + ",");
            sb.Append(p1.RRBI + ",");
            sb.Append(p1.RBB + ",");
            sb.Append(p1.RIBB + ",");
            sb.Append(p1.RK + ",");
            sb.Append(p1.RPAB + ",");
            sb.Append(p1.RPHR + ",");
            sb.Append("0,");
            sb.Append("0,");
            sb.Append("0,");
            sb.Append("0,");
            sb.Append("0,");
            sb.Append("0,");
            sb.Append("0,");
            sb.Append("0,");
            sb.Append("0,");
            sb.Append("0,");
            sb.Append("0");

            return sb.ToString();


        }
    }
}
