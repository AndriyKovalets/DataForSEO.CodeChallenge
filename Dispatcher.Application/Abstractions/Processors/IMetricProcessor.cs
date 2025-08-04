using Dispatcher.Domain.Abstractions;
using Dispatcher.Domain.Models;

namespace Dispatcher.Application.Abstractions.Processors;

public interface IMetricProcessor
{
    public int MetricCount { get; }
    
    public void Process(KeywordModel? model);
    
    public void SetMetric(IMetrics  metrics);
}