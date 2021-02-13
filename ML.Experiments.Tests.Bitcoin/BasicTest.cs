using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using ML.Experiments.API.Models.Bitcoin;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace ML.Experiments.Tests.Bitcoin
{
    public class Tests
    {
        [Test]
        public void bitcoinHistoryForecastTimeSeriesCheck()
        {
            var fileName = @"D:\downloads\Windir\csvjson.json";
            var json = File.ReadAllText(fileName);
            var dataset = JsonConvert.DeserializeObject<BitcoinHistoricalData[]>(json);
            var mlContext = new MLContext();

            var trainingDataset = dataset.Where(x =>
                x.day >= new DateTime(2013, 1, 1) && x.day <= new DateTime(2017, 12, 30));
            var testDataset = dataset.Where(x =>
                x.day >= new DateTime(2018, 1, 1) && x.day <= new DateTime(2020, 12, 30));

            //var loadedTrainingDataset = mlContext.Data.LoadFromEnumerable(trainingDataset);
            //var loadedTestDataset = mlContext.Data.LoadFromEnumerable(testDataset);
            var loadedFullDataset = mlContext.Data.LoadFromEnumerable(dataset);

            var seriesLen = 30; 
            var testMonthes = 3;
            var testCollectionWindow = seriesLen * testMonthes; // 30 days * 7 month
            var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: nameof(BitcoinHistoricalData.openPrice),
                    inputColumnName: nameof(BitcoinHistoricalData.openPrice),
                    windowSize: 30, // ???
                    seriesLength: 365, // one year
                    trainSize: trainingDataset.Count()-testCollectionWindow,
                    horizon: 7,
                    confidenceLevel: 0.95f,
                    confidenceLowerBoundColumn: nameof(BitcoinHistoricalData.lowerPrice),
                    confidenceUpperBoundColumn: nameof(BitcoinHistoricalData.highestPrice));
            //var forecaster = forecastingPipeline.Fit(loadedTrainingDataset);
            var forecaster = forecastingPipeline.Fit(loadedFullDataset);

            IDataView predictions = forecaster.Transform(loadedFullDataset);
            var actual = mlContext.Data.CreateEnumerable<BitcoinHistoricalData>(loadedFullDataset, false)
                .Select(observed => observed.openPrice);
            var forecast = mlContext.Data.CreateEnumerable<BitcoinForecastOutput>(predictions, true)
                    .Select(prediction => prediction.openPrice[0]).ToArray();

            var metrics = actual.Zip(forecast, (actualValue, forecastValue) => actualValue - forecastValue);
            var MAE = metrics.Average(error => Math.Abs(error)); // Mean Absolute Error
            var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(error, 2))); // Root Mean Squared Error

            var forecastEngine = forecaster.CreateTimeSeriesEngine<
                BitcoinHistoricalData, BitcoinForecastOutput>(mlContext);
            forecastEngine.CheckPoint(mlContext, "bitcoinModel.zip");

            var enginePredictions = forecastEngine.Predict(testMonthes);

        }
    }
}