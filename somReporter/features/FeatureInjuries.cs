﻿using System;
using System.Collections.Generic;
using Microsoft.Isam.Esent.Collections.Generic;
using somReporter.output;
using System.IO;
using somReporter.util.somReporter;
using somReportUtils;

namespace somReporter.features
{
    public class FeatureInjuries : IFeature
    {
        private List<string> currentDataLines;
        private List<string> prevDataLines = new List<string>();
        private String rawData;
        
        private const String injuryFile = "previousInjury.txt";
        public Report getReport()
        {
            throw new NotImplementedException();
        }

        public void setData(string injuryData)
        {
            rawData = injuryData;

            if (File.Exists(injuryFile))
            {
                SOMReportFile somFile = new SOMReportFile(injuryFile);
                prevDataLines = somFile.readFileLinesOnly(true);
            }

            currentDataLines = parseStringIntoLines(injuryData);
        }

        public void initialize(ISOMReportFile file)
        {
            throw new NotImplementedException();
        }

        public void process(IOutput output)
        {
            List<string> formattedInjuryLines = new List<string>();
            List<string> backFromInjuryList = new List<string>();

            Dictionary<string, int> previous = new Dictionary<string, int>();

            if (prevDataLines.Count > 0 )
                previous = parseInjuryData(prevDataLines);

            Dictionary<string, int> current = parseInjuryData(currentDataLines);

            foreach( string name in current.Keys)
            {
                if(previous.Count > 0 )
                {
                    Boolean newInjury = !previous.ContainsKey(name);
                    if (newInjury)
                       formattedInjuryLines.Add(String.Format("<b>{0,-30} {1}{2} games left *NEW*</b>", name, overInjuryWaterMark(current[name]), current[name]));
                    else
                       formattedInjuryLines.Add(String.Format("{0,-30} {1}{2} games left", name, overInjuryWaterMark(current[name]), current[name]));

                    if(!newInjury)
                        previous.Remove(name);
                }
                else
                {
                    formattedInjuryLines.Add(String.Format("{0,-30} {1}{2} games left", name, overInjuryWaterMark(current[name]), current[name]));
                }
            }

            foreach (string name in previous.Keys)
            {
                backFromInjuryList.Add(String.Format("{0,-30} missed {1} games last run", name, previous[name]));
            }

            if (output == null)
            {
                  System.Console.WriteLine("INJURIES");

                foreach (String line in formattedInjuryLines)
                    System.Console.WriteLine(line);

                System.Console.WriteLine("RETURNING");

                foreach (String line in backFromInjuryList)
                    System.Console.WriteLine(line);
            }
            else
            {
                File.WriteAllText(injuryFile, rawData);
                output.ShowInjuryData(formattedInjuryLines, backFromInjuryList);
            }
        }

        private string overInjuryWaterMark(int injuryDayCount)
        {
            if (injuryDayCount > Config.MAX_INJURY_DAYS)
                return "*";
            return "";
        }

        public void setDateStore(PersistentDictionary<string, string> dictionary)
        {
            throw new NotImplementedException();
        }

        public List<String> parseStringIntoLines(String injuryData)
        {
            List<String> lines = new List<String>();

            lines.AddRange(injuryData.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
            return lines;
        }

        public Dictionary<string, int> parseInjuryData(List<String> lines)
        {
            Dictionary<string, int> map = new Dictionary<string, int>();

            foreach (String line in lines) {
                if (line.Length == 0) continue;
                if (line.Equals("No injuries to report"))
                    return map;

                String[] split = line.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                map.Add(split[0], 
                    Int32.Parse(split[1].Substring(0, split[1].IndexOf(' '))));
            }
            return map;
        }
    }
}
