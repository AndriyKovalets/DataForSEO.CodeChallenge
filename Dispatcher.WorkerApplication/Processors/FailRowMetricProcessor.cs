using Dispatcher.Domain.Abstractions;
using Dispatcher.Domain.Models;
using Dispatcher.WorkerApplication.Abstractions.Services.Processors;

namespace Dispatcher.WorkerApplication.Processors;

public class FailRowMetricProcessor: IMetricProcessor
{
    public static FailRowMetricProcessor Instance => new();
    
    public int MetricCount { get; private set; }
    
    public void Process(KeywordModel? model)
    {
        if (model is null)
        {
            MetricCount++;
        }
    }

    public void SetMetric(IMetrics metrics)
    {
        metrics.CountOfFailRows = MetricCount;
    }
}