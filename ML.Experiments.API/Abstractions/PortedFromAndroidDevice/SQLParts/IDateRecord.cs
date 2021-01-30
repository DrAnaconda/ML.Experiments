namespace ML.Experiments.API.Abstractions
{
    public interface IDateRecord
    {
        /// <summary>
        /// YYYYMMDDHHmm format
        /// </summary>
        public long Date { get; set; }
    }
}
