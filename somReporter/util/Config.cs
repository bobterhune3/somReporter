﻿using System;
using System.IO;
using somReportUtils;

namespace somReporter.util.somReporter
{
    public class Config : IConfig
    {
        public static Config config = null;

        private const String CONFIG_FILE_NAME = "config.properties";

        public static bool HAS_WILDCARD = false;
        public static bool RANK_STATS_BY_DIVISION = true;
        public static bool STRAIGHT_DRAFT_ORDER = false;
        public static bool SHOW_WARNING = false;
        public static bool SHOW_MORAL = false;
        public static float WARNING_LEVEL = 0.8f;
        public static float SUGGESTION_LEVEL_PERCENT = 1.1f;
        public static String PRT_FILE_LOCATION = "F:\\cdrombb\\print";
        public static String LEAGUE_NAME = "2020ND";

        public static bool SHOW_STANDINGS = true;
        public static bool SHOW_NOTES = true;
        public static bool SHOW_WHOS_HOT = true;
        public static bool SHOW_INJURY_REPORT = true;
        public static bool SHOW_DRAFT_ORDER = true;
        public static bool SHOW_RECORD_BOOK = true;
        public static bool SHOW_USAGE = true;
        public static bool SHOW_SCHEDULE = true;
        public static bool SHOW_DIV_MAGIC_NUM = false;

        public static int MAX_INJURY_DAYS = 20;
        public static int SCHEDULE_NUMBER_OF_DAYS = 5;
        private static float ESTIMATE_AB_MULTIPLIER = 1f;
        private static float ESTIMATE_IP_MULTIPLIER = 1f;
        private static int AB_MINIMUM = 50;
        private static int IP_MINIMUM = 30;

        public Config()
        {
            if (File.Exists(CONFIG_FILE_NAME))
                readConfiguration(CONFIG_FILE_NAME);
            dumpValues();
        }


        public float getABMultiplier()        {            return ESTIMATE_AB_MULTIPLIER;        }
        public float getIPMultiplier()        {            return ESTIMATE_IP_MULTIPLIER;        }
        public float getMinABAllowed()        {            return AB_MINIMUM;        }
        public float getMinIPAllowed()        {            return IP_MINIMUM;        }

        private void dumpValues() {
            Console.Out.WriteLine("HAS_WILDCARD = " + HAS_WILDCARD);
            Console.Out.WriteLine("RANK_STATS_BY_DIVISION = " + RANK_STATS_BY_DIVISION);
            Console.Out.WriteLine("STRAIGHT_DRAFT_ORDER = " + STRAIGHT_DRAFT_ORDER);
            Console.Out.WriteLine("SHOW_MORAL = " + SHOW_MORAL);
            Console.Out.WriteLine("WARNING_LEVEL = " + WARNING_LEVEL);
            Console.Out.WriteLine("SUGGESTION_LEVEL_PERCENT = " + SUGGESTION_LEVEL_PERCENT);
            Console.Out.WriteLine("PRT_FILE_LOCATION = " + PRT_FILE_LOCATION);
            Console.Out.WriteLine("LEAGUE_NAME = " + LEAGUE_NAME);
            Console.Out.WriteLine("SHOW_STANDINGS = " + SHOW_STANDINGS);
            Console.Out.WriteLine("SHOW_NOTES = " + SHOW_NOTES);
            Console.Out.WriteLine("SHOW_WHOS_HOT = " + SHOW_WHOS_HOT);
            Console.Out.WriteLine("SHOW_INJURY_REPORT = " + SHOW_INJURY_REPORT);
            Console.Out.WriteLine("SHOW_DRAFT_ORDER = " + SHOW_DRAFT_ORDER);
            Console.Out.WriteLine("SHOW_RECORD_BOOK = " + SHOW_RECORD_BOOK);
            Console.Out.WriteLine("SHOW_USAGE = " + SHOW_USAGE);
            Console.Out.WriteLine("SHOW_SCHEDULE = " + SHOW_SCHEDULE);
            Console.Out.WriteLine("SCHEDULE_NUMBER_OF_DAYS = " + SCHEDULE_NUMBER_OF_DAYS);
            Console.Out.WriteLine("SHOW_DIV_MAGIC_NUM = " + SHOW_DIV_MAGIC_NUM);
            Console.Out.WriteLine("MAX_INJURY_DAYS = " + MAX_INJURY_DAYS);
            Console.Out.WriteLine("ESTIMATE_AB_MULTIPLIER = " + ESTIMATE_AB_MULTIPLIER);
            Console.Out.WriteLine("ESTIMATE_IP_MULTIPLIER = " + ESTIMATE_IP_MULTIPLIER);
            Console.Out.WriteLine("AB_MINIMUM = " + AB_MINIMUM);
            Console.Out.WriteLine("IP_MINIMUM = " + IP_MINIMUM);
        }

        public void readConfiguration(String configFileName)
        {
            System.IO.StreamReader file = null;
            try
            {
                string line;

                // Read the file and display it line by line.
                file = new System.IO.StreamReader(configFileName);
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {

                        string[] values = line.Split('=');
                        string key = values[0];
                        string value = values[1];

                        bool tmpValue;
                        int tmpNValue;
                        float tmpFValue;
                        //        Console.WriteLine(key + "=" + value);

                        if (key.Equals("HAS_WILDCARD"))
                        {
                            Boolean.TryParse(value, out tmpValue);
                            HAS_WILDCARD = tmpValue;
                        }
                        else if (key.Equals("RANK_STATS_BY_DIVISION"))
                        {
                            Boolean.TryParse(value, out tmpValue);
                            RANK_STATS_BY_DIVISION = tmpValue;
                        }
                        else if (key.Equals("STRAIGHT_DRAFT_ORDER"))
                        {
                            Boolean.TryParse(value, out tmpValue);
                            STRAIGHT_DRAFT_ORDER = tmpValue;
                        }

                        else if (key.Equals("SHOW_USAGE_WARNINGS"))
                        {
                            Boolean.TryParse(value, out tmpValue);
                            SHOW_WARNING = tmpValue;
                        }
                        else if (key.Equals("SHOW_SUGGESTION_USAGE_WARNINGS"))
                        {
                            Boolean.TryParse(value, out tmpValue);
                            SHOW_MORAL = tmpValue;
                        }
                        else if (key.Equals("WARNING_LEVEL_PERCENT"))
                        {
                            System.Int32.TryParse(value, out tmpNValue);
                            WARNING_LEVEL = (((float)tmpNValue) / 100f);
                        }
                        else if (key.Equals("SUGGESTION_LEVEL_PERCENT"))
                        {
                            System.Int32.TryParse(value, out tmpNValue);
                            SUGGESTION_LEVEL_PERCENT = (((float)tmpNValue) / 100f);
                        }
                        else if (key.Equals("PRT_FILE_LOCATION"))
                        {
                            PRT_FILE_LOCATION = value;
                        }
                        else if (key.Equals("LEAGUE_NAME"))
                        {
                            LEAGUE_NAME = value;
                        }
                        else if (key.Equals("SHOW_STANDINGS"))
                        {
                            Boolean.TryParse(value, out tmpValue);
                            SHOW_STANDINGS = tmpValue;
                        }
                        else if (key.Equals("SHOW_NOTES"))
                        {
                            Boolean.TryParse(value, out tmpValue);
                            SHOW_NOTES = tmpValue;
                        }
                        else if (key.Equals("SHOW_WHOS_HOT"))
                        {
                            Boolean.TryParse(value, out tmpValue);
                            SHOW_WHOS_HOT = tmpValue;
                        }
                        else if (key.Equals("SHOW_INJURY_REPORT"))
                        {
                            Boolean.TryParse(value, out tmpValue);
                            SHOW_INJURY_REPORT = tmpValue;
                        }
                        else if (key.Equals("SHOW_DRAFT_ORDER"))
                        {
                            Boolean.TryParse(value, out tmpValue);
                            SHOW_DRAFT_ORDER = tmpValue;
                        }
                        else if (key.Equals("SHOW_RECORD_BOOK"))
                        {
                            Boolean.TryParse(value, out tmpValue);
                            SHOW_RECORD_BOOK = tmpValue;
                        }
                        else if (key.Equals("SHOW_USAGE"))
                        {
                            Boolean.TryParse(value, out tmpValue);
                            SHOW_USAGE = tmpValue;
                        }
                        else if (key.Equals("SHOW_SCHEDULE"))
                        {
                            Boolean.TryParse(value, out tmpValue);
                            SHOW_SCHEDULE = tmpValue;
                        }
                        else if (key.Equals("SCHEDULE_NUMBER_OF_DAYS"))
                        {
                            SCHEDULE_NUMBER_OF_DAYS = Int32.Parse(value);
                        }
                        else if( key.Equals("SHOW_DIV_MAGIC_NUM"))
                        {
                            Boolean.TryParse(value, out tmpValue);
                            SHOW_DIV_MAGIC_NUM = tmpValue;
                        }
                        else if (key.Equals("MAX_INJURY_DAYS"))
                        {
                            Int32.TryParse(value, out tmpNValue);
                            MAX_INJURY_DAYS = tmpNValue;
                        }
                        else if (key.Equals("ESTIMATE_AB_MULTIPLIER"))
                        {
                            float.TryParse(value, out tmpFValue);
                            ESTIMATE_AB_MULTIPLIER = tmpFValue;
                        }
                        else if (key.Equals("ESTIMATE_IP_MULTIPLIER"))
                        {
                            float.TryParse(value, out tmpFValue);
                            ESTIMATE_IP_MULTIPLIER = tmpFValue;
                        }
                        else if (key.Equals("AB_MINIMUM"))
                        {
                            Int32.TryParse(value, out tmpNValue);
                            AB_MINIMUM = tmpNValue;
                        }
                        else if (key.Equals("IP_MINIMUM"))
                        {
                            Int32.TryParse(value, out tmpNValue);
                            IP_MINIMUM = tmpNValue;
                        }
                    }
                 }
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }


        public static String getConfigurationFile(String filename) {
            if(config == null )
                config = new Config();

            return Path.Combine(PRT_FILE_LOCATION, LEAGUE_NAME, filename);
        }

    }
}
