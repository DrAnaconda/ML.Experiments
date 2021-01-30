namespace ML.Experiments.API.Abstractions
{
    public interface ISleepSession : IDateRecord, IDurable, ISQLiteEntity
    {
        public short DeepDurability { get; set; }
    }
}
