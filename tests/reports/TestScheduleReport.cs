using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using somReporter.features;
using somReporter.util;
using somReporter;
using somReporter.reports.team;
using System.Text.RegularExpressions;
namespace tests.reports
{
    [TestClass]
    public class TestScheduleReport
    {
        [TestMethod]
        public void loadFeature()
        {
            IFeature feature = FeatureFactory.loadFeature(FeatureFactory.FEATURE.SCHEDULE);
            Assert.IsNotNull(feature);
        }

        [TestMethod]
        public void loadScheduleFileFail()
        {
            try
            {
                FeatureSchedule feature = (FeatureSchedule)FeatureFactory.loadFeature(FeatureFactory.FEATURE.SCHEDULE);
                feature.initialize(new SOMReportFile(Config.getConfigurationFile("SCHEDULEX.PRT")));
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
                return;
            }
            Assert.Fail("An exception should have been thrown if the file is not found");

        }

        [TestMethod]
        public void loadScheduleFileSuccess()
        {
            try
            {
                FeatureSchedule feature = (FeatureSchedule)FeatureFactory.loadFeature(FeatureFactory.FEATURE.SCHEDULE);
                feature.initialize(new SOMReportFile(Config.getConfigurationFile("SCHEDULE.PRT")));
            }
            catch (Exception)
            {
                Assert.Fail("An exception should not have been thrown because the file should exist");
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void scheduleGames_Blank()
        {
            try
            {
                ScheduleDay day = new ScheduleDay();
                day.addGames("");
            }
            catch (Exception)
            {
                Assert.Fail("An exception should not have been thrown because the file should exist");
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void scheduleGames_SingleLine()
        {
            const String SINGLE_LINE = "MLG-TXG* PTB-TBM* DTB-OKM* SDG-LAM*";

            ScheduleDay day = new ScheduleDay();
            day.addGames(SINGLE_LINE);

            String result1 = day.getTeamsGameToday("MLG");
            Assert.IsTrue(result1.Equals("at TXG<br>"));
            String result2 = day.getTeamsGameToday("TXG");
            Assert.IsTrue(result2.Equals("vs MLG<br>"));

            String result3 = day.getTeamsGameToday("SDG");
            Assert.IsTrue(result3.Equals("at LAM<br>"));
            String result4 = day.getTeamsGameToday("LAM");
            Assert.IsTrue(result4.Equals("vs SDG<br>"));
        }

        [TestMethod]
        public void scheduleGames_DoubleLine()
        {
            const String LINE_1 = "MLG-TXG  PTB-TBM  DTB-OKM  SDG-LAM* CLM-SLB* SEG-NYB* CHB-AZB*";
            const String LINE_2 = "KCM-WSG* PHM-TOG*";

            ScheduleDay day = new ScheduleDay();
            day.addGames(LINE_1);
            day.addGames(LINE_2);

            String result1 = day.getTeamsGameToday("MLG");
            Assert.IsTrue(result1.Equals("at TXG<br>"));
            String result2 = day.getTeamsGameToday("TXG");
            Assert.IsTrue(result2.Equals("vs MLG<br>"));

            String result3 = day.getTeamsGameToday("SDG");
            Assert.IsTrue(result3.Equals("at LAM<br>"));
            String result4 = day.getTeamsGameToday("LAM");
            Assert.IsTrue(result4.Equals("vs SDG<br>"));


            String result5 = day.getTeamsGameToday("KCM");
            Assert.IsTrue(result5.Equals("at WSG<br>"));
            String result6 = day.getTeamsGameToday("WSG");
            Assert.IsTrue(result6.Equals("vs KCM<br>"));
        }

        [TestMethod]
        public void scheduleGames_GetSetOfDays()
        {
            FeatureSchedule feature = (FeatureSchedule)FeatureFactory.loadFeature(FeatureFactory.FEATURE.SCHEDULE);
            feature.initialize(new SOMReportFile(Config.getConfigurationFile("SCHEDULE.PRT")));

            string schedule = feature.getTeamSchedulesForDays("OKM", 15, 5);

            Assert.IsTrue(schedule.StartsWith("day off<br>"));
            Assert.AreEqual(5, schedule.Split('<').Length, 6);

        }
    }
}
