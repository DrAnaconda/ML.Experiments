using ML.Experiments.API.Abstractions;
using ML.Experiments.API.Enums;


namespace ML.Experiments.API.Models
{
    public class HeartRecord : IHeartRecord
    {
        public byte HRValue { get; set; }
        public HearthIntervalAnalyticType AnalyticType { get; set; }
        public long Date { get; set; }
        public long ID { get; set; }
    }
}
