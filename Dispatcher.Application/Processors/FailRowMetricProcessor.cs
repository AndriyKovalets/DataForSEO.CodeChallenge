using Dispatcher.Application.Abstractions.Processors;
using Dispatcher.Domain.Abstractions;
using Dispatcher.Domain.Models;

namespace Dispatcher.Application.Processors;

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