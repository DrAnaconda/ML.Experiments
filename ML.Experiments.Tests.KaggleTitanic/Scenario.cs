using CsvHelper;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ML.Experiments.Tests.KaggleTitanic
{
    public class Tests
    {
        private readonly MLContext mlContext = new MLContext();

        private IEnumerable<OriginalType> parserJsonFile<OriginalType>(string filename) 
            where OriginalType: TitanicValuableModelTrainSet
        {
            string json = File.ReadAllText(filename);
            var array = JsonConvert.DeserializeObject<IEnumerable<OriginalType>>(json,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            foreach (var val in array)
            {
                if (val.Age <= 1) val.Age = float.NaN;
            }
            return array;
        }
        private IDataView parseDataset<OriginalType>(string filename) 
            where OriginalType: TitanicValuableModelTrainSet
        {
            var array = parserJsonFile<OriginalType>(filename);
            var result = mlContext.Data.LoadFromEnumerable(array);
            return result;
        }

        private IDataView parseDataNormalizeMissedDataWithAverage(IDataView data)
        {
            var replacementEstimator = mlContext.Transforms.ReplaceMissingValues(
                nameof(TitanicValuableModelTrainSet.Age), 
                replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean);
            ITransformer replacementTransformer = replacementEstimator.Fit(data);
            return replacementTransformer.Transform(data);
        }

        private IDataView parseDataDiscardMissedData<OriginalType>(string filename)
            where OriginalType : TitanicValuableModelTrainSet
        {
            var array = parserJsonFile<OriginalType>(filename);
            var filteredArray = array.Where(x => x.Age > 3).ToArray();
            return mlContext.Data.LoadFromEnumerable(filteredArray);
        }

        private EstimatorChain<NormalizingTransformer> BuildTrainingPipeline()
        {
            // Data process configuration with pipeline data transformations 
            var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey("Survived", "Survived")
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(
                new[] { new InputOutputColumnPair("Sex", "Sex") }))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(
                    new[] { new InputOutputColumnPair("Sex", "Sex") }))
                .Append(mlContext.Transforms.Categorical.OneHotHashEncoding(
                    new[] { new InputOutputColumnPair("Pclass", "Pclass"), new InputOutputColumnPair("SibSp", "SibSp"), new InputOutputColumnPair("Parch", "Parch") }))
                .Append(mlContext.Transforms.Concatenate("Features", 
                new[] { "Sex", "Pclass", "SibSp", "Parch", "Age", "Fare" }))
                .Append(mlContext.Transforms.NormalizeMinMax("Features", "Features"))
                .AppendCacheCheckpoint(mlContext);
            // Set the training algorithm 

            return dataProcessPipeline;
        }

        private void generateAnswers(string testFilename, string comment,
            PredictionEngine<TitanicValuableModelTrainSet, ModelOutput> predicter)
        {
            var answers = new ConcurrentBag<TitanicTestModel>();
            var testData = parserJsonFile<TitanicTestModel>(testFilename);
            foreach(var data in testData)
            {
                var answer = predicter.Predict(data);
                if (answer.Prediction.ToLower() == "true")
                {
                    answers.Add(new TitanicTestModel()
                    {
                        PassengerId = data.PassengerId,
                        Survived = "true"
                    });
                }
                else
                {
                    answers.Add(new TitanicTestModel()
                    {
                        PassengerId = data.PassengerId,
                        Survived = "false"
                    });
                }
            }
            File.WriteAllText(@$"D:\downloads\Windir\titanic\{comment}.json",
                JsonConvert.SerializeObject(answers, Formatting.Indented));
        }

        private PredictionEngine<TitanicValuableModelTrainSet, ModelOutput>
            createOneVersusAllPredicter(IDataView trainingData)
        {
            var pipeline = BuildTrainingPipeline();
            var trainer = mlContext.MulticlassClassification.Trainers.OneVersusAll(
                mlContext.BinaryClassification.Trainers.AveragedPerceptron(labelColumnName: @"Survived", numberOfIterations: 50, featureColumnName: "Features"), labelColumnName: @"Survived")
                          .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
            var trainingPipeline = pipeline.Append(trainer);
            var fitter = trainingPipeline.Fit(trainingData);
            var predicter = mlContext.Model.CreatePredictionEngine
                <TitanicValuableModelTrainSet, ModelOutput>(fitter);
            return predicter;
        }

        /// <summary>
        /// Total shit
        /// </summary>
        /// <param name="trainingData"></param>
        /// <returns></returns>
        private PredictionEngine<TitanicValuableModelTrainSet, ModelOutput>
    createPairwiseCouplerWithLinearSvm(IDataView trainingData)
        {
            var pipeline = BuildTrainingPipeline();
            var trainer = mlContext.MulticlassClassification.Trainers.PairwiseCoupling(
                mlContext.BinaryClassification.Trainers.LinearSvm(labelColumnName: @"Survived", featureColumnName: "Features", numberOfIterations: 10000), labelColumnName: @"Survived")
                          .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
            var trainingPipeline = pipeline.Append(trainer);
            var fitter = trainingPipeline.Fit(trainingData);
            var predicter = mlContext.Model.CreatePredictionEngine
                <TitanicValuableModelTrainSet, ModelOutput>(fitter);
            return predicter;
        }

        private PredictionEngine<TitanicValuableModelTrainSet, ModelOutput> createWithLdSvm(IDataView trainingData)
        {
            var pipeline = BuildTrainingPipeline();
            var trainer = mlContext.MulticlassClassification.Trainers.SdcaNonCalibrated(
                    labelColumnName: @"Survived", featureColumnName: "Features")
                    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
            var trainingPipeline = pipeline.Append(trainer);
            var fitter = trainingPipeline.Fit(trainingData);
            var predicter = mlContext.Model.CreatePredictionEngine
                <TitanicValuableModelTrainSet, ModelOutput>(fitter);
            return predicter;
        }

        [TestCase(@"D:\downloads\Windir\titanic\train.json", @"D:\downloads\Windir\titanic\test.json")]
        public void UseAverageAgeInMissedData_OneVersusAll(string trainFilename, string testFilename)
        {
            var data = parseDataset<TitanicValuableModelTrainSet>(trainFilename);
            var filledData = parseDataNormalizeMissedDataWithAverage(data);
            var predicter = createOneVersusAllPredicter(filledData);
            
            generateAnswers(testFilename, "age-oneVersusAll", predicter);
        }

        /// <summary>
        /// 65 % of accuracy
        /// </summary>
        /// <param name="trainFilename"></param>
        /// <param name="testFilename"></param>
        [TestCase(@"D:\downloads\Windir\titanic\train.json", @"D:\downloads\Windir\titanic\test.json")]
        public void DiscardMissedData_OneVersusAll(string trainFilename, string testFilename)
        {
            var trainData = parseDataDiscardMissedData<TitanicValuableModelTrainSet>(trainFilename);
            var predicter = createOneVersusAllPredicter(trainData);
            
            generateAnswers(testFilename, "discarded-age-oneVersusAll.json", predicter);
        }

        /// <summary>
        /// Total shit
        /// </summary>
        /// <param name="trainFilename"></param>
        /// <param name="testFilename"></param>
        [TestCase(@"D:\downloads\Windir\titanic\train.json", @"D:\downloads\Windir\titanic\test.json")]
        public void DiscardMissedData_PairwiseCoupler(string trainFilename, string testFilename)
        {
            var trainData = parseDataDiscardMissedData<TitanicValuableModelTrainSet>(trainFilename);
            var predicter = createPairwiseCouplerWithLinearSvm(trainData);

            generateAnswers(testFilename, "discarded-age-pairwiseCoupler.csv", predicter);
        }

        [TestCase(@"D:\downloads\Windir\titanic\train.json", @"D:\downloads\Windir\titanic\test.json")]
        public void DiscardMissedData_Sdaca(string trainFilename, string testFilename)
        {
            var trainData = parseDataDiscardMissedData<TitanicValuableModelTrainSet>(trainFilename);
            var predicter = createWithLdSvm(trainData);

            generateAnswers(testFilename, "discarded-age-Sdaca", predicter);
        }
    }
    public class ModelOutput
    {
        [ColumnName("PredictedLabel")]
        public string Prediction { get; set; }
        public float[] Score { get; set; }
    }
    public class TitanicValuableModelTrainSet
    {
        [ColumnName("Survived"), LoadColumn(5)]
        //[ColumnName("Label")]
        public string Survived { get; set; }
        [ColumnName("Pclass"), LoadColumn(0)] 
        public byte Pclass { get; set; }
        [ColumnName("Age"), LoadColumn(1)]
        public float Age { get; set; }
        [ColumnName("SibSp"), LoadColumn(2)]
        public byte SibSp { get; set; }
        [ColumnName("Parch"), LoadColumn(3)]
        public byte Parch { get; set; }
        [ColumnName("Fare"), LoadColumn(4)]
        public float Fare { get; set; }
        [ColumnName("Sex"), LoadColumn(6)]
        public string Sex { get; set; }
    }
    public class TitanicTestModel : TitanicValuableModelTrainSet
    {
        public string PassengerId { get; set; }
    }
}