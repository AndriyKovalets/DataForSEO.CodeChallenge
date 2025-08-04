using Dispatcher.Application.Abstractions.Processors;
using Dispatcher.Domain.Abstractions;
using Dispatcher.Domain.Models;

namespace Dispatcher.Application.Processors;

public class TotalRowMetricProcessor: IMetricProcessor
{
    public static TotalRowMetricProcessor Instance => new();
    
    public int MetricCount { get; private set; }
    
    public void Process(KeywordModel? model)
    {
        MetricCount++;
    }
    
    public void SetMetric(IMetrics metrics)
    {
        metrics.CountOfRows = MetricCount;
    }
}