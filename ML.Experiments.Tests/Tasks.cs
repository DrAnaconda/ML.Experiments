using MathNet.Numerics.Statistics;
using Microsoft.ML;
using Microsoft.ML.TimeSeries;
using ML.Experiments.API.Models;
using ML.Experiments.API.Models.DataScienceHelpers;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ML.Experiments.Tests
{
    public class Tasks : DataAnalyzerBase
    {
        private readonly DataParser parser = new DataParser();

        [Test] [Explicit]
        public async Task splitDatasetIntoGroupsForAnalyze()
        {
            var fullDataset = await dataParser.readFillMergeData(true, false, false, false);
            var daysGroups = fullDataset.Values.GroupBy(key => new { key.year, key.month, key.day }).Select(x=>x.ToList());
            var results = new List<DataGroupCheck>(daysGroups.Count());
            foreach(var day in daysGroups)
            {
                var minutesAfterMidnight = day.Select(x => (double)x.minutesAfterMidnight);
                var hrValues = day.Select(x => (double)x.HRValue);
                var pearcman = Correlation.Pearson(minutesAfterMidnight, hrValues);
                var spearman = Correlation.Spearman(minutesAfterMidnight, hrValues);
                var currentDayObject = new DataGroupCheck() { innerData = day, pearsmanValue = pearcman, spearsmanValue = spearman };
                results.Add(currentDayObject);
                File.WriteAllText($"{day[0].year}-{day[0].month}-{day[0].day}.json", 
                    JsonConvert.SerializeObject(currentDayObject, Formatting.Indented));
            }
            var test = results.OrderBy(x=>x.spearsmanValue);
        }

        [Test(Description = "Define anomalies using ML.NET framework")] [Explicit]
        public async Task tryFoundSeasons()
        {
            var mlContext = new MLContext();
            var dataset = await parser.readFillMergeData(true, false, false, false);
            var input = dataset.Values.Select(x => new HearthRecordInput() { HRValue = x.HRValue, timestampML = x.timestamp })
                .OrderBy(x=>x.timestampML).ToArray();
            var data = mlContext.Data.LoadFromEnumerable(input);
            var periods = mlContext.AnomalyDetection.DetectSeasonality(data, nameof(HeartRecord.HRValue));
            var outputDataView = mlContext.AnomalyDetection.DetectEntireAnomalyBySrCnn(
                data, nameof(HeartRecordPrediction.Prediction), nameof(HeartRecord.HRValue), 0.15, input.Length / 4,
                99, SrCnnDetectMode.AnomalyAndExpectedValue);
            var predictions = mlContext.Data.CreateEnumerable<HeartRecordPrediction>(outputDataView, reuseRowObject: false).ToArray();
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            int printEnabled = 0;
            for (int indx = 0; indx < predictions.Count(); indx++)
            {
                if (indx + 1 < predictions.Count())
                {
                    if (predictions[indx + 1].Prediction[0] == 1)
                    {
                        printEnabled = 3;
                    }
                }
                if (printEnabled-- >= 0)
                {
                    Console.WriteLine("{0}: {2} at {3} {4} —— {1}", indx, string.Join(" | ", predictions[indx].Prediction),
                        input[indx].HRValue, dtDateTime.AddSeconds(input[indx].timestampML).ToLongDateString(),
                        dtDateTime.AddSeconds(input[indx].timestampML).ToLongTimeString());
                    if (printEnabled == -1) Console.WriteLine();
                }
            }

            // Not bad with default options
            // Not bad x2 with 0.15, 12, 99
            // 0.1, 12, 99 is nice for sleeping phase trigger, physical stress etc

            // 0.05, 12, 99 produces shitty data => seems very big precision and low window is bad combination for this case
            // same for 120 window and for 512, 1024, 0.25 of original size too
            // 2048 better, but not enought

            // 0.15, 1/8, 99 produces good results
            // same thing with 0.15, 1/4, 99
        }
    }
}