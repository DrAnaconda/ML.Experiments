using System;

namespace ML.Experiments.API.Models.Bitcoin
{
    public class BitcoinHistoricalData
    {
        public DateTime day { get; set; }
        public float openPrice { get; set; }
        public float highestPrice { get; set; }
        public float lowerPrice { get; set; }
    }
    public class BitcoinForecastOutput
    {
        public float[] openPrice { get; set; }
        public float[] lowerPrice { get; set; }
        public float[] highestPrice { get; set; }
    }
}
