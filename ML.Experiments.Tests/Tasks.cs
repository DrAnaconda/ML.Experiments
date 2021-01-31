using MathNet.Numerics.Statistics;
using Microsoft.ML;
using Microsoft.ML.TimeSeries;
using ML.Experiments.API.Abstractions;
using ML.Experiments.API.Enums;
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

        [Test(Description = "Define anomalies using ML.NET framework (date to value)")] [Explicit]
        public async Task detectAnomalies_DateToValue()
        {
            var dataset = await parser.readFillMergeData(true, false, false, false);
            var input = dataset.Values
                .Select(x => new HearthRecordInputTimeToValue() { HRValue = x.HRValue, timestampML = x.timestamp })
                .OrderBy(x=>x.timestampML).ToArray();
            var predictions = analyzeInput(input);
            printPredictions(predictions, input);
            // Not bad with default options
            // Not bad x2 with 0.15, 12, 99
            // 0.1, 12, 99 is nice for sleeping phase trigger, physical stress etc

            // 0.05, 12, 99 produces shitty data => seems very big precision and low window is bad combination for this case
            // same for 120 window and for 512, 1024, 0.25 of original size too
            // 2048 better, but not enought

            // 0.15, 1/8, 99 produces good results
            // same thing with 0.15, 1/4, 99
            // algo couldn`t determine periodically
        }

        [Test(Description = "Define anomalies using ml framework (group to value)")]
        public async Task detectAnomalies_ValueToGroup()
        {
            var dataset = await parser.readFillMergeData(true, false, false, false);
            var input = dataset.Values
                .Where(x=>x.AnalyticType >= HearthIntervalAnalyticType.Steady && x.AnalyticType <= HearthIntervalAnalyticType.Sleeping)
                .OrderBy(x => x.Date)
                .Select(x => new HearthRecordInputValueToGroup() { HRValue = x.HRValue, typeGroup = (int)x.AnalyticType, timestampML = x.timestamp })
                .ToArray();
            var predictions = analyzeInput(input);
            printPredictions(predictions, input);
            // poor data for 0.15, 0.25os, 99
            // better for 0.3, 1024, 99
            // nice for 0.75 -/-, but collection is obsious poor
            // not bad for 0.05, 0.25os, 99
            // very well for 0.1, 0.25os, but manulally readjustings/filters is required
            // not bad for 0.05, 0.25os, but filtering required => 0.21638973969800418 / 0.15029399166547008 (10000%)
        }

        private HeartRecordPrediction[] analyzeInput<DataType>(DataType[] input) where DataType: class
        {
            var mlContext = new MLContext();
            var data = mlContext.Data.LoadFromEnumerable(input);
            var periods = mlContext.AnomalyDetection.DetectSeasonality(data, nameof(HeartRecord.HRValue));

            var outputDataView = mlContext.AnomalyDetection.DetectEntireAnomalyBySrCnn(
                data, nameof(HeartRecordPrediction.Prediction), nameof(HeartRecord.HRValue), 0.05, input.Length / 4,
                99, SrCnnDetectMode.AnomalyAndExpectedValue);

            return mlContext.Data.CreateEnumerable<HeartRecordPrediction>(outputDataView, reuseRowObject: false).ToArray();
        }

        private void printPredictions<DataType>(HeartRecordPrediction[] predictions, DataType[] input) 
            where DataType: IHearthRecordML
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            int printEnabled = 0;
            for (int indx = 0; indx < predictions.Length; indx++)
            {
                if (indx + 1 < predictions.Length)
                {
                    if (predictions[indx + 1].Prediction[0] == 1)
                    {
                        printEnabled = 3;
                    }
                }
                if (printEnabled-- >= 0)
                {
                    Console.WriteLine("{0}: {2} at {3} \t at group {4} \t {1}", 
                        indx, string.Join(" | ", predictions[indx].Prediction),
                        input[indx].HRValue, dtDateTime.AddSeconds(input[indx].timestampML).ToLongDateString(),
                        (HearthIntervalAnalyticType)input[indx].typeGroup);
                    if (printEnabled == -1) Console.WriteLine();
                }
            }
        }
    }
}