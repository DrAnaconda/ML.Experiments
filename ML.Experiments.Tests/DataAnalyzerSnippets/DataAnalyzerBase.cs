using NUnit.Framework;
using MathNet.Numerics.Statistics;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System;

namespace ML.Experiments.Tests
{
    public abstract class DataAnalyzerBase
    {
        protected readonly Random randomizer = new Random();
        protected readonly DataParser dataParser = new DataParser();

        protected void checkCorrelationComplex(IEnumerable<double> vectorA, IEnumerable<double> vectorB)
        {
            var correlationByPearsman = Math.Abs(Correlation.Pearson(vectorA, vectorB));
            var correlationBySpearman = Math.Abs(Correlation.Spearman(vectorA, vectorB));
            var autoCorrelationValue = Correlation.Auto(vectorB.ToArray());
            Warn.If(correlationByPearsman < 0.7, $"Weak correlation by pearsman, was {correlationByPearsman}");
            Warn.If(correlationBySpearman < 0.7, $"Weak correlation by Spearman, was {correlationBySpearman}");
            Console.WriteLine($"{nameof(correlationByPearsman)}: {correlationByPearsman}");
            Console.WriteLine($"{nameof(correlationBySpearman)}: {correlationBySpearman}");
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            Console.WriteLine($"Complex correlation check for {sf.GetMethod()} done");
        }
    }
}

// Conclusion 1: Use correlation on grouped data is shitty idea