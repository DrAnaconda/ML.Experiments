using Microsoft.ML.Data;
using ML.Experiments.API.Abstractions;
using ML.Experiments.API.Enums;


namespace ML.Experiments.API.Models
{
    public class HeartRecord : DateBase, IHeartRecord
    {
        [LoadColumn(1)]
        public byte HRValue { get; set; }
        [LoadColumn(2)] // Unsupported in seasons detection
        public HearthIntervalAnalyticType AnalyticType { get; set; }
        public long ID { get; set; }

        [LoadColumn(0)]
        public long timestampML { get { return  timestamp; } }
    }
    public class HearthRecordInput
    {
        [LoadColumn(1)]
        public double HRValue { get; set; }
        [LoadColumn(0)]
        public long timestampML { get; set; }
    }
    public class HeartRecordPrediction
    {
        [VectorType(7)]
        public double[] Prediction { get; set; }
    }
}
