using ML.Experiments.API.Models;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ML.Experiments.Tests
{
    public class DataAnalyzer : DataAnalyzerBase
    {
        #region Pearsman and Spearman Correlations Tests. Shitty tries

        [Test(Description = "Analyzing Correlation Beetween Cycade Rythm and Hearth Rate")]
        public async Task investigateCorrelationTheory_HourMinuteBeetweenHearthRate_SecondsScaled()
        {
            var fullDataset = await dataParser.readFillMergeData(true, false, false, false);
            var timeVector = fullDataset.Values.Select(x => (double)x.secondsAfterMidnight).ToArray();
            var hrVector = fullDataset.Values.Select(x => (double)x.HRValue).ToArray();
            checkCorrelationComplex(timeVector, hrVector);
            // failed 0.34
        }

        [Test(Description = "Analyzing Correlation Beetween Cycade Rythm and Hearth Rate Filtered by steady state")]
        public async Task investigateCorrelationTheory_OnlySteady_SecondsScaled()
        {
            var fullDataset = await dataParser.readFillMergeData(true, false, false, false);
            var steadyValues = fullDataset.Values.Where(x => x.AnalyticType == API.Enums.HearthIntervalAnalyticType.Steady);
            var timeVector = steadyValues.Select(x => (double)x.secondsAfterMidnight).ToArray();
            var hrVector = steadyValues.Select(x => (double)x.HRValue).ToArray();
            checkCorrelationComplex(timeVector, hrVector);
            // failed 0.35
        }

        [Test(Description = "Analyzing Correlation Beetween Cycade Rythm and Hearth Rate. Minute scaled")]
        public async Task investigateCorrelationTheory_HourMinuteBeetweenHearthRate_MinuteScaled()
        {
            var fullDataset = await dataParser.readFillMergeData(true, false, false, false);
            var timeVector = fullDataset.Values.Select(x => (double)x.minutesAfterMidnight).ToArray();
            var hrVector = fullDataset.Values.Select(x => (double)x.HRValue).ToArray();
            checkCorrelationComplex(timeVector, hrVector);
            // 0.35 scaled
        }

        [Test(Description = "Analyzing Correlation Beetween Cycade Rythm and Hearth Rate Filtered by steady state. Minute scaled")]
        public async Task investigateCorrelationTheory_OnlySteady_MinuteScaled()
        {
            var fullDataset = await dataParser.readFillMergeData(true, false, false, false);
            var steadyValues = fullDataset.Values.Where(x => x.AnalyticType == API.Enums.HearthIntervalAnalyticType.Steady);
            var timeVector = steadyValues.Select(x => (double)x.minutesAfterMidnight).ToArray();
            var hrVector = steadyValues.Select(x => (double)x.HRValue).ToArray();
            checkCorrelationComplex(timeVector, hrVector);
            // 0.35 scaled
        }

        [Test(Description = "Analyzing Correlation Beetween Cycade Rythm and Hearth Rate. Hour scaled")]
        public async Task investigateCorrelationTheory_HourMinuteBeetweenHearthRate_HourScaled()
        {
            var fullDataset = await dataParser.readFillMergeData(true, false, false, false);
            var timeVector = fullDataset.Values.Select(x => (double)x.hour).ToArray();
            var hrVector = fullDataset.Values.Select(x => (double)x.HRValue).ToArray();
            checkCorrelationComplex(timeVector, hrVector);
            // 0.35 scaled
        }

        [Test(Description = "Analyzing Correlation Beetween Cycade Rythm and Hearth Rate Filtered by steady state. Hour scaled")]
        public async Task investigateCorrelationTheory_OnlySteady_HourScaled()
        {
            var fullDataset = await dataParser.readFillMergeData(true, false, false, false);
            var steadyValues = fullDataset.Values.Where(x => x.AnalyticType == API.Enums.HearthIntervalAnalyticType.Steady);
            var timeVector = steadyValues.Select(x => (double)x.hour).ToArray();
            var hrVector = steadyValues.Select(x => (double)x.HRValue).ToArray();
            checkCorrelationComplex(timeVector, hrVector);
            // 0.35 scaled
        }

        #endregion

        [Test(Description = "Checks if analytic type which I implemented is useless")]
        public async Task validateAnalyticType()
        {
            var fullDataset = await dataParser.readFillMergeData(true, false, false, false);
            var localDataset = fullDataset.Values.Where(x => x.AnalyticType >= API.Enums.HearthIntervalAnalyticType.Steady
                && x.AnalyticType <= API.Enums.HearthIntervalAnalyticType.MediumPhysical);
            var analyticAxis = localDataset.Select(x => (double)x.secondsAfterMidnight).ToArray();
            var hrVector = localDataset.Select(x => (double)x.HRValue).ToArray();
            checkCorrelationComplex(analyticAxis, hrVector);
        }

        [Test(Description = "Try correlate random day")]
        public async Task correlateRandomDayWithoutGrouping()
        {
            IEnumerable<TotallyAggregatedRecord> localDataset;
            const int maxRecordsEvery5Min = (24 * 60) / 5;
            do {
                var year = 2020; var month = randomizer.Next(1, 12); var randomDay = randomizer.Next(1, 25);
                var fullDataset = await dataParser.readFillMergeData(true, false, false, false);
                localDataset = fullDataset.Values.Where(x => x.year == year && x.month == month && x.day == randomDay);
                //Assume.That(localDataset.Count() > maxRecordsEvery5Min / 2, $"Poor dataset @ {year}.{month}.{randomDay}");
            } while (localDataset.Count() < maxRecordsEvery5Min * 0.75);
            var timeVector = localDataset.Select(x => (double)x.minutesAfterMidnight).ToArray();
            var hrVector = localDataset.Select(x => (double)x.HRValue).ToArray();
            checkCorrelationComplex(timeVector, hrVector);
        }
    }
}

// Conclusion 1: Use correlation on grouped data is shitty idea