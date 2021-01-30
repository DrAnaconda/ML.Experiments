namespace ML.Experiments.API.Abstractions
{
    public interface IMainRecord : IDateRecord, ISQLiteEntity
    {
        public int Steps { get; set; }
        public int Calories { get; set; }
    }
}
