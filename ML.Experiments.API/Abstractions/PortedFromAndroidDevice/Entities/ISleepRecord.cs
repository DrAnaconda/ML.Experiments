using ML.Experiments.API.Enums;

namespace ML.Experiments.API.Abstractions
{
    public interface ISleepRecord : IDateRecord, IDurable, ISQLiteEntity
    {
        /// <summary>
        /// Wired up with `ISleepSession`
        /// </summary>
        public long idss { get; set; }
        public SleepRecordType Type { get; set; }
    }
}
