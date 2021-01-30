using ML.Experiments.API.Abstractions;


namespace ML.Experiments.API.Models
{
    public class SleepSession : ISleepSession
    {
        public short DeepDurability { get; set; }
        public long Date { get; set; }
        public short Durability { get; set; }
        public long ID { get; set; }
    }
}
