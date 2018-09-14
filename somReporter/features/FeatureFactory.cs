using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using somReporter.util.somReporter;

namespace somReporter.features
{
    public class FeatureFactory
    {
        public enum FEATURE {
            NOTES,
            STANDINGS,
            WHOS_HOT,
            INJURY_REPORT,
            DRAFT_ORDER,
            RECORD_BOOK,
            USAGE,
            SCHEDULE
        }

        private static IFeature featureNotes = new FeatureNotes();
        private static IFeature featureStandings = new FeatureStandings();
        private static IFeature featureWhosHot = new FeatureWhosHot();
        private static IFeature featureInjuries = new FeatureInjuries();
        private static IFeature featureDraftOrderTierd = new FeatureDraftOrderTierd();
        private static IFeature featureDraftOrderStraight = new FeatureDraftOrderStraight();
        private static IFeature featureRecordBook = new FeatureRecordBook();
        private static IFeature featureUsage = new FeatureUsage();
        private static IFeature FeatureSchedule = new FeatureSchedule();

        public static IFeature loadFeature( FEATURE feature ) {
            switch(feature) {
                case FEATURE.NOTES:
                    return featureNotes;
                case FEATURE.STANDINGS:
                    return featureStandings;
                case FEATURE.WHOS_HOT:
                    return featureWhosHot;
                case FEATURE.INJURY_REPORT:
                    return featureInjuries;
                case FEATURE.DRAFT_ORDER:
                    if (Config.STRAIGHT_DRAFT_ORDER)
                       return featureDraftOrderStraight;
                    else
                        return featureDraftOrderTierd;
                case FEATURE.RECORD_BOOK:
                    return featureRecordBook;
                case FEATURE.USAGE:
                    return featureUsage;
                case FEATURE.SCHEDULE:
                    return FeatureSchedule;
            }
            return null;
        }
    }
}
