using Dispatcher.Domain.Abstractions;
using Dispatcher.Domain.Models;
using Dispatcher.WorkerApplication.Abstractions.Services.Processors;

namespace Dispatcher.WorkerApplication.Processors;

public class MisspelledKeywordsMetricProcessor:  IMetricProcessor
{
    public static MisspelledKeywordsMetricProcessor Instance => new();
    
    public int MetricCount { get; private set; }
    
    public void Process(KeywordModel? model)
    {
        if (model is { Spell: not null })
        {
            MetricCount++;
        }
    }
    
    public void SetMetric(IMetrics metrics)
    {
        metrics.MisspelledKeywords = MetricCount;
    }
}