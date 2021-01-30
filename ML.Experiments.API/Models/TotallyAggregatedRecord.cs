using Microsoft.ML.Data;
using ML.Experiments.API.Abstractions;
using ML.Experiments.API.Enums;

namespace ML.Experiments.API.Models
{
    public class TotallyAggregatedRecord : DateBase, IHeartRecord, ISleepRecord, IMainRecord
    {
        #region Data aggregations

        [LoadColumn(0)]
        public byte HRValue { get; set; }
        [LoadColumn(1)]
        public HearthIntervalAnalyticType AnalyticType { get; set; }

        public long ID { get; set; }
        public long idss { get; set; }
        public SleepRecordType Type { get; set; }
        public short Durability { get; set; }
        public int Steps { get; set; }
        public int Calories { get; set; }

        #endregion
    }
}