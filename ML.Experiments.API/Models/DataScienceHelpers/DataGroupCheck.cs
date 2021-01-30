using MathNet.Numerics.Statistics;
using System.Collections.Generic;
using System.Linq;

namespace ML.Experiments.API.Models.DataScienceHelpers
{
    public class DataGroupCheck
    {
        public IEnumerable<TotallyAggregatedRecord> innerData { get; set; }
        public double spearsmanValue { get; set; }
        public double pearsmanValue { get; set; }
        public int dataSize { get { return innerData.Count(); } }
        public IEnumerable<int> axisX { get { return innerData.Select(x => x.minutesAfterMidnight); } }
        public IEnumerable<byte> axisY { get { return innerData.Select(x => x.HRValue); } }
        public IEnumerable<double> autocorrelation { get { return Correlation.Auto(innerData.Select(x => (double)x.HRValue).ToArray()); } }
    }
}
