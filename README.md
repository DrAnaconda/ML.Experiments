## Intorduction
Just a first try to connect with  [ML.NET](https://docs.microsoft.com/en-us/dotnet/machine-learning/). For read only and evaluation purposes.
## Kaggle Titanic
[Dataset](https://www.kaggle.com/c/titanic) for beginers in ML provided by [Kaggle](https://www.kaggle.com). Attempt to predict who will survive. Only age + sex + ticket price was used as input data, other data as port, ticket number was discarded as useless.
Dataset prepare strategies:
1. Discard missed age data
2. Fill missed age data by mean

|  Algo | Data Preparation   | Result  |
| -------| ----| ----|
| OneVersusAll Learner + Averaged perceptron  | Missed data discarded  | 65.55% |
| OneVersusAll Learner + Averaged perceptron | Missed data filled by mean | 64.832% |
| Pairwise coupler + LinearSvm | Missed data discarded | 63.397% |
| Microsoft Model Builder (FastTree) | Missed data discarded | 50.956% |

## Bitcoin

Attempt to forecast bitcoin prices. Long story short: mean error 12%, but good prediction on big time intervals.

## MFitX

Anomaly detection algorithm in heart rate data.
Strategies/hypothesis:
1. Anomaly detection based on cycade rythm
2. Anomaly detection based on physical activity

Long story story: all strategies/hypothesis are good/comfirmed
