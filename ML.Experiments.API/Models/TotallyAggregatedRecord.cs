using ML.Experiments.API.Abstractions;
using ML.Experiments.API.Enums;


namespace ML.Experiments.API.Models
{
    public class TotallyAggregatedRecord : IHeartRecord, ISleepRecord, IMainRecord
    {
        public byte HRValue { get; set; }
        public HearthIntervalAnalyticType AnalyticType { get; set; }
        public long Date { get; set; }
        public long ID { get; set; }
        public long idss { get; set; }
        public SleepRecordType Type { get; set; }
        public short Durability { get; set; }
        public int Steps { get; set; }
        public int Calories { get; set; }
    }
}
