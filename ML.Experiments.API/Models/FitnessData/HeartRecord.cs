using Microsoft.ML.Data;
using ML.Experiments.API.Abstractions;
using ML.Experiments.API.Enums;


namespace ML.Experiments.API.Models
{
    public class HeartRecord : DateBase, IHeartRecord
    {
        public byte HRValue { get; set; }
        public HearthIntervalAnalyticType AnalyticType { get; set; }
        public long ID { get; set; }
        public long timestampML { get { return timestamp; } }
    }
    public class HearthRecordInputTimeToValue : IHearthRecordML
    {
        [LoadColumn(1)]
        public double HRValue { get; set; }
        [LoadColumn(0)]
        public long timestampML { get; set; }
        public int typeGroup { get; set; }
    }
    public class HearthRecordInputValueToGroup : IHearthRecordML
    {
        [LoadColumn(1)]
        public double HRValue { get; set; }
        [LoadColumn(0)]
        public int typeGroup { get; set; }
        public long timestampML { get; set; }
    }
    public class HeartRecordPrediction
    {
        [VectorType(7)]
        public double[] Prediction { get; set; }
    }
}
