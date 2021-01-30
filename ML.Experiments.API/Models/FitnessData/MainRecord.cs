using ML.Experiments.API.Abstractions;


namespace ML.Experiments.API.Models
{
    public class MainRecord : IMainRecord
    {
        public int Steps { get; set; }
        public int Calories { get; set; }
        public long Date { get; set; }
        public long ID { get; set; }
    }
}
