using somReporter.output;
using Microsoft.Isam.Esent.Collections.Generic;

namespace somReporter.features
{
    public interface IFeature
    {
        void process(IOutput output);
        void initialize(SOMReportFile file);
        Report getReport();
        void setDateStore(PersistentDictionary<string, string> dictionary);
    }
}