using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            USAGE
        }

        private static IFeature featureNotes = new FeatureNotes();
        private static IFeature featureStandings = new FeatureStandings();
        private static IFeature featureWhosHot = new FeatureWhosHot();
        private static IFeature featureInjuries = new FeatureInjuries();
        private static IFeature featureDraftOrder = new FeatureDraftOrder();
        private static IFeature featureRecordBook = new FeatureRecordBook();
        private static IFeature featureUsage = new FeatureUsage();

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
                    return featureDraftOrder;
                case FEATURE.RECORD_BOOK:
                    return featureRecordBook;
                case FEATURE.USAGE:
                    return featureUsage;
            }
            return null;
        }

/*                    private LeagueStandingsReport leagueStandingsReport;
        private LeagueGrandTotalsReport leaguePrimaryStatReport;
        private LineScoreReport lineScoreReport;
        private NewspaperStyleReport newspaperStyleReport;
        private RecordBookReport recordBookReport;
        private ComparisonReport teamComparisonReport;
        */
    
    }
}
