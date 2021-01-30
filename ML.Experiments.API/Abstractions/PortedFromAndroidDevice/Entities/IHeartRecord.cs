using ML.Experiments.API.Enums;

namespace ML.Experiments.API.Abstractions
{
    public interface IHeartRecord : IDateRecord, ISQLiteEntity
    {
        public byte HRValue { get; set; }
        public HearthIntervalAnalyticType AnalyticType { get; set; }
    }
}
