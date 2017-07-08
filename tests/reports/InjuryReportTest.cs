using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using somReporter.features;
using System.Collections.Generic;
using System.IO;

namespace tests.reports
{
    [TestClass]
    public class InjuryReportTest
    {
        private FeatureInjuries feature;

        [TestInitialize()]
        public void Initialize()
        {
            feature = new FeatureInjuries();
        }

        [TestMethod]
        public void testLineParser()
        {
            List<string> lines = feature.parseStringIntoLines(SAMPLE_INJURY_DATA);
            Assert.AreEqual(33, lines.Count);
        }

        [TestMethod]
        public void testPlayerLister()
        {
            feature.setData(SAMPLE_INJURY_DATA);

            List<string> lines = feature.parseStringIntoLines(SAMPLE_INJURY_DATA);
            Dictionary<string, int> map = feature.parseInjuryData(lines);

            int adamsGames = map["M.Adams, Arizona"];
            Assert.AreEqual(29, adamsGames);

            int vanGames = map["S.Van Slyke, Philadelphia"];
            Assert.AreEqual(60, vanGames);

            int blancoGames = map["G.Blanco, Toronto"];
            Assert.AreEqual(1, blancoGames);
        }

        [TestMethod]
        public void testReportWithoutPreviousData()
        {
            feature.setData(SAMPLE_INJURY_DATA);

            feature.process(null);
        }

        [TestMethod]
        public void testReportWithPreviousData()
        {
            File.WriteAllText("previousInjury.txt", PREVIOUS_INJURY_DATA);
            feature.setData(SAMPLE_INJURY_DATA);

            feature.process(null);
            File.Delete("previousInjury.txt");
        }


        private const String PREVIOUS_INJURY_DATA =
                "M.Adams, Arizona - 39 more games\r\n" +
                "W.Peralta, Chicago - 26 more games\r\n" +
                "K.Terhune, Portland - 2 more games\r\n" +
                "J.Ross, Cleveland - 88 more games\r\n" +
                "M.Foltynewicz, Detroit - 10 more games\r\n" +
                "S.Wright, Los Angeles - 12 more games\r\n" +
                "J.Lamb, Milwaukee - 24 more games\r\n" +
                "E.Hernandez, New York - 53 more games\r\n" +
                "C.Bethancourt, Oakland - 45 more games\r\n" +
                "S.Van Slyke, Philadelphia - 60 more games\r\n" +
                "H.Pence, San Diego - 10 more games\r\n" +
                "R.Stripling, San Diego - 39 more games\r\n" +
                "V.Worley, Texas - 41 more games\r\n" +
                "S.Vogt, Washington - 15 more games\r\n" +
                "B.Terhune, Portland - 5 more games\r\n" +
                "B.Holt, Kansas City - 18 more games\r\n";

        private const String SAMPLE_INJURY_DATA =
            "M.Adams, Arizona - 29 more games\r\n" +
            "W.Peralta, Chicago - 16 more games\r\n" +
            "M.Harvey, Cleveland - 7 more games\r\n" +
            "J.Ross, Cleveland - 58 more games\r\n" +
            "B.Finnegan, Detroit - 3 more games\r\n" +
            "M.Foltynewicz, Detroit - 10 more games\r\n" +
            "B.Drury, Los Angeles - 3 more games\r\n" +
            "T.La Stella, Los Angeles - 2 more games\r\n" +
            "J.Rickard, Los Angeles - 1 more games\r\n" +
            "S.Wright, Los Angeles - 12 more games\r\n" +
            "J.Lamb, Milwaukee - 24 more games\r\n" +
            "E.Hernandez, New York - 53 more games\r\n" +
            "J.Paredes, New York - 3 more games\r\n" +
            "C.Bethancourt, Oakland - 45 more games\r\n" +
            "J.Upton, Philadelphia - 3 more games\r\n" +
            "S.Van Slyke, Philadelphia - 60 more games\r\n" +
            "X.Cedeno, Philadelphia - 3 more games\r\n" +
            "H.Pence, San Diego - 10 more games\r\n" +
            "H.Kim, San Diego - 3 more games\r\n" +
            "R.Stripling, San Diego - 39 more games\r\n" +
            "Y.Cespedes, Seattle - 2 more games\r\n" +
            "R.Lopez, Seattle - 6 more games\r\n" +
            "M.Cabrera, St.Louis - 3 more games\r\n" +
            "F.Hernandez, St.Louis - 2 more games\r\n" +
            "L.McCullers, St.Louis - 4 more games\r\n" +
            "P.Hughes, Texas - 2 more games\r\n" +
            "V.Worley, Texas - 41 more games\r\n" +
            "N.Wittgren, Texas - 2 more games\r\n" +
            "G.Blanco, Toronto - 1 more games\r\n" +
            "G.Stanton, Washington - 8 more games\r\n" +
            "S.Vogt, Washington - 15 more games\r\n" +
            "B.Holt, Kansas City - 18 more games\r\n" +
            "S.Manaea, Kansas City - 2 more games";
    }
}
