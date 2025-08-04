using Dispatcher.Application.Abstractions.Processors;
using Dispatcher.Domain.Models;

namespace Dispatcher.Application.Processors;

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
}