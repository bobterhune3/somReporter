using somReporter.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace somReporter.features
{
    public interface IFeature
    {
        void process(IOutput output);
        void initialize(SOMReportFile file);
        Report getReport();
    }
}
