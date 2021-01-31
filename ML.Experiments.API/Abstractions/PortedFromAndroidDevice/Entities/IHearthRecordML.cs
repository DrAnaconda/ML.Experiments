namespace ML.Experiments.API.Abstractions
{
    public interface IHearthRecordML
    {
        public double HRValue { get; set; }
        public int typeGroup { get; set; }
        public long timestampML { get; set; }
    }
}
