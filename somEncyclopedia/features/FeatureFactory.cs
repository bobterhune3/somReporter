
using somReporter.features;

namespace somEncylopedia.features
{
    public class FeatureFactory
    {
        public enum FEATURE {
            PRIMARY,
            SECONDARY,
            SPLITS
        }

        private static IFeature featurePrimary = new FeaturePrimary();
        private static IFeature featureSecondary = new FeatureSecondary();

        public static IFeature loadFeature( FEATURE feature ) {
            switch(feature) {
                case FEATURE.PRIMARY:
                    return featurePrimary;
                case FEATURE.SECONDARY:
                    return featureSecondary;

            }
            return null;
        }
    }
}
