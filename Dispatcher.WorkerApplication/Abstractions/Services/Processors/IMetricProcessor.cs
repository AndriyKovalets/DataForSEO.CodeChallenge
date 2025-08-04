using Dispatcher.Domain.Abstractions;
using Dispatcher.Domain.Models;

namespace Dispatcher.WorkerApplication.Abstractions.Services.Processors;

public interface IMetricProcessor
{
    public int MetricCount { get; }
    
    public void Process(KeywordModel? model);
    
    public void SetMetric(IMetrics  metrics);
}