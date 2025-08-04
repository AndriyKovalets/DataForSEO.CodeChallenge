using Dispatcher.Domain.Abstractions;
using Dispatcher.Domain.Models;
using Dispatcher.WorkerApplication.Abstractions.Services.Processors;

namespace Dispatcher.WorkerApplication.Processors;

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