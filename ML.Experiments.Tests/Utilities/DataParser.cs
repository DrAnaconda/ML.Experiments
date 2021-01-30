using ML.Experiments.API.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Threading;
using System.Collections.Generic;
using ML.Experiments.API.Abstractions;

namespace ML.Experiments.Tests
{
    public class DataParser
    {
        public ConcurrentDictionary<long, TotallyAggregatedRecord> fullDataset = new ConcurrentDictionary<long, TotallyAggregatedRecord>();

        public const string pathToMainRecordsDatasetUngrouped = @"D:\downloads\Windir\MainRecordsUngrouped.csv";
        public const string pathToHearthRecords = @"D:\downloads\Windir\HearthRecords.csv";
        public const string pathToHearthRecordsCleaned = @"D:\downloads\Windir\HearthRecordsCleaned.csv";
        public const string pathToSleepingDataRecords = @"D:\downloads\Windir\SleepRecord.csv";


        private IAsyncEnumerable<RequiredType> parseCsv<RequiredType>(string pathToFile)
        {
            TextReader reader = File.OpenText(pathToFile);
            var configuration = new CsvConfiguration(Thread.CurrentThread.CurrentCulture)
            {
                MissingFieldFound = null
            };
            CsvReader csv = new CsvReader(reader, configuration);
            return csv.GetRecordsAsync<RequiredType>();
        }

        private async Task readAndPushToDictionary<RequiredType>(string pathToData) where RequiredType : IDateRecord
        {
            await foreach (var item in parseCsv<RequiredType>(pathToData))
            {
                lock (fullDataset)
                {
                    var result = fullDataset.TryGetValue(item.Date, out var record);
                    if (result)
                        mergeObjects(record, item);
                    else
                    {
                        var newItem = new TotallyAggregatedRecord();
                        mergeObjects(newItem, item);
                        fullDataset.TryAdd(item.Date, newItem);
                    }
                }
            }
        }

        private void mergeObjects<SourceType, ParentType>(SourceType sourceInstanse, ParentType parentInstanse)
        {
            var parentProperies = typeof(ParentType).GetProperties();
            foreach (var property in parentProperies)
            {
                var sourceProperty = typeof(SourceType).GetProperty(property.Name);
                if (sourceProperty != null && sourceProperty.CanWrite)
                {
                    sourceProperty.SetValue(sourceInstanse, property.GetValue(parentInstanse));
                }
            }
        }

        public async Task<ConcurrentDictionary<long, TotallyAggregatedRecord>> readFillMergeData(
            bool includeHearthData, bool includeMainDataUngrouped, bool includeMainDataGrouped, bool includeSleepingData)
        {
            var tasks = new List<Task>();
            if (includeHearthData)
                tasks.Add(readAndPushToDictionary<HeartRecord>(pathToHearthRecords));
            if (includeMainDataUngrouped)
                tasks.Add(readAndPushToDictionary<MainRecord>(pathToMainRecordsDatasetUngrouped));
            if (includeSleepingData)
                tasks.Add(readAndPushToDictionary<MainRecord>(pathToSleepingDataRecords));
            await Task.WhenAll(tasks);
            return fullDataset;
        }
    }
}

// Conclusion 1: Use correlation on grouped data is shitty idea