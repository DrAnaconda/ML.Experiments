using NUnit.Framework;
using MathNet.Numerics.Statistics;

namespace ML.Experiments.Tests
{
    public class CorrelationTests
    {
        [Test]
        public void validateObviousData()
        {
            var axisX = new double[] { 1, 2, 3, 4, 5};
            var axisY = new double[] { 2, 4, 9, 16, 25 };
            var correlationByPearsman = Correlation.Pearson(axisX, axisY);
            var correlationBySpearsman = Correlation.Spearman(axisX, axisY);
            Assert.IsTrue(correlationByPearsman > 0.9, $"Shitty library: pearsman validation failed");
            Assert.IsTrue(correlationBySpearsman > 0.9, $"Shitty library: spearsman validation failed");
            Warn.If(correlationByPearsman < 0.9 && correlationByPearsman > 0.8, $"Better, but shitty, pearsman correlation");
            Warn.If(correlationBySpearsman < 0.9 && correlationBySpearsman > 0.8, $"Better, but shitty, spearsman correlation");
        }
    }
}