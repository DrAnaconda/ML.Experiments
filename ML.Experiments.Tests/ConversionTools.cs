using ML.Experiments.API.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace ML.Experiments.Tests
{
    public class ConversionTools
    {
        [Test]
        public void validateConvert()
        {
            var year = 2142; var month = 66; var day = 88;
            var hour = 66;  var minute = 33;
            var testObject = new TotallyAggregatedRecord()
            {
                Date = 214266886633
            };
            Assert.AreEqual(year, testObject.year);
            Assert.AreEqual(month, testObject.month);
            Assert.AreEqual(day, testObject.day);
            Assert.AreEqual(hour, testObject.hour);
            Assert.AreEqual(minute, testObject.minute);
        }
    }
}