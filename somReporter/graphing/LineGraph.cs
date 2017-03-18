using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Script.Serialization;  //System.Web.Extensions

namespace somReporter
{
    public class LineGraph
    {
        private string rawData = "";
        public LineGraph()
        {
            rawData = File.ReadAllText("line-legend.template");
        }

         public void setGraphData(String title, List<Team> teams, bool draftOrder) {
            Data data = buildJSONObjects(teams, draftOrder);

            string json = new JavaScriptSerializer().Serialize(data);
            json = json.Replace("\"labels\"", "labels");
            json = json.Replace("\"datasets\"", "datasets");
            json = json.Replace("\"label\"", "label");
            json = json.Replace("\"borderColor\"", "borderColor");
            json = json.Replace("\"data\"", "data");
            json = json.Replace("\"lineTension\"", "lineTension");
            json = json.Replace("\"fill\"", "fill");
            rawData = rawData.Replace("[PUT_DATA_HERE]", json);
            rawData = rawData.Replace("[PUT_TITLE_HERE]", title);
        }

        public void save(string htmlFile)
        {
            File.WriteAllText(htmlFile, rawData);
        }

        private Data buildJSONObjects(List<Team> teams, bool draftOrderReport)
        {
            Data data = new Data();
            List<DataSets> datasets = new List<DataSets>();

            data.setLabels(teams[0]);
            foreach( Team team in teams ) {
                if( !draftOrderReport || 
                    (draftOrderReport && team.Wpct < .450 )) {
                        DataSets ds = new DataSets();
                        ds.label = team.Abrv;
                        ds.borderColor = getTeamColor(team.Abrv);
                        ds.setData(team);
                        datasets.Add(ds);
                }

            }
            data.datasets = datasets.ToArray();

            return data;
        }

        public string getTeamColor(string abv)
        {
            return "rgba(" + lookupColor(abv) + ",.5)";
        }

        private string lookupColor(string abv) {
            if( abv.Equals("BLJ")) return "242,197,56";
            if( abv.Equals("BSS")) return "196,59,67";
            if( abv.Equals("CLM")) return "21,26,46";
            if( abv.Equals("DTB")) return "212,94,46";
            if( abv.Equals("NYB")) return "193,28,22";
            if( abv.Equals("TOG")) return "27,45,117";
            if( abv.Equals("TBM")) return "166,195,225";

            if( abv.Equals("CXS")) return "40,41,36";
            if( abv.Equals("KCM")) return "7,42,101";
            if( abv.Equals("ANS")) return "198,61,29";
            if( abv.Equals("MNB")) return "193,28,22";
            if( abv.Equals("OKM")) return "4,71,60";
            if( abv.Equals("SEG")) return "4,23,68";
            if( abv.Equals("TXG")) return "28,46,118";

            if( abv.Equals("CHB")) return "7,23,83";
            if( abv.Equals("MMS")) return "32,142,139";
            if( abv.Equals("MLG")) return "73,95,155";
            if( abv.Equals("PHM")) return "198,58,67";
            if( abv.Equals("PTB")) return "252,177,13";
            if( abv.Equals("SLB")) return "212,4,36";
            if( abv.Equals("WSG")) return "4,37,92";

            if( abv.Equals("AZB")) return "193,28,22";
            if( abv.Equals("ATS")) return "7,23,83";
            if( abv.Equals("CRM")) return "76,66,127";
            if( abv.Equals("HSJ")) return "204,78,87";
            if( abv.Equals("LAM")) return "21,103,163";
            if( abv.Equals("SDG")) return "56,38,24";
            if (abv.Equals("SFG")) return "238,181,142";
            return "0,0,0";

        }
    }

    public class Data
    {
        public string[] labels;
        public DataSets[] datasets;

        public void setLabels(Team team)
        {
            List<String> myLabels = new List<String>();
            int counter = 1;
            foreach( float f in team.WinPctHistoryData)
            {
                String s = "Run "+counter;
                counter++;
                myLabels.Add(s);
            }
            labels = myLabels.ToArray();
        }
    }

    public class DataSets
    {
        public string label;
        public string borderColor;
        public double[] data;
        public double lineTension = .2;
        public bool fill = false;

        public void setData(Team team)
        {
            List<double> list = new List<double>();
            foreach (float f in team.WinPctHistoryData)
            {
                list.Add(Math.Round(f, 3));
            }
            data = list.ToArray();
        }
    }



}
