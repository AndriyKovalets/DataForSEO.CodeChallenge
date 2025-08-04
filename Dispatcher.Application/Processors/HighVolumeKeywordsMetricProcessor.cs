using Dispatcher.Application.Abstractions.Processors;
using Dispatcher.Domain.Abstractions;
using Dispatcher.Domain.Models;

namespace Dispatcher.Application.Processors;

public class HighVolumeKeywordsMetricProcessor: IMetricProcessor
{
    public static HighVolumeKeywordsMetricProcessor Instance => new();
    
    public int MetricCount { get; private set; }
    
    public void Process(KeywordModel? model)
    {
        if (model is { KeywordInfo.SearchVolume: > 100_000 })
        {
            MetricCount++;
        }
    }
    
    public void SetMetric(IMetrics metrics)
    {
        metrics.HighVolumeKeywords = MetricCount;
    }
}