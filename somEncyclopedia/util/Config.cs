using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace somReporter.util.somReporter
{
    public class Config
    {
        private static Config config = null;

        private const String CONFIG_FILE_NAME = "config.properties";

        public static String PRT_FILE_LOCATION = "C:\\cdrombb\\print";
        public static String LEAGUE_NAME = "2018ND";
        public static int LEAGUE_YEAR = 2018;

        public static bool SHOW_STANDINGS = true;
        public static bool SHOW_NOTES = true;
        public static bool SHOW_WHOS_HOT = true;
        public static bool SHOW_INJURY_REPORT = true;
        public static bool SHOW_DRAFT_ORDER = true;
        public static bool SHOW_RECORD_BOOK = true;
        public static bool SHOW_USAGE = true;
        public static bool SHOW_SCHEDULE = true;

        public Config()
        {
            if (File.Exists(CONFIG_FILE_NAME))
                readConfiguration(CONFIG_FILE_NAME);
            dumpValues();
        }

        private void dumpValues() {
            Console.Out.WriteLine("PRT_FILE_LOCATION = " + PRT_FILE_LOCATION);
            Console.Out.WriteLine("LEAGUE_NAME = " + LEAGUE_NAME);
            Console.Out.WriteLine("YEAR = " + LEAGUE_YEAR);
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
                        //      int tmpNValue;
                        //        Console.WriteLine(key + "=" + value);

                        if (key.Equals("PRT_FILE_LOCATION"))
                        {
                            PRT_FILE_LOCATION = value;
                        }
                        else if (key.Equals("LEAGUE_NAME"))
                        {
                            LEAGUE_NAME = value;
                        }
                        else if (key.Equals("YEAR"))
                        {
                            LEAGUE_YEAR = Convert.ToInt32(value);
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
