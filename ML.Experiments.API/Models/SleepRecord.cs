using ML.Experiments.API.Abstractions;
using ML.Experiments.API.Enums;


namespace ML.Experiments.API.Models
{
    public class SleepRecord : ISleepRecord
    {
        public long idss { get; set; }
        public SleepRecordType Type { get; set; }
        public long Date { get; set; }
        public short Durability { get; set; }
        public long ID { get; set; }
    }
}
