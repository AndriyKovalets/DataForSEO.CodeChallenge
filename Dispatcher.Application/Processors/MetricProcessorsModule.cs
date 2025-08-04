using System.Reflection.Metadata;
using Dispatcher.Application.Abstractions.Processors;
using Dispatcher.Domain.Abstractions;

namespace Dispatcher.Application.Processors;

public class MetricProcessorsModule: IMetricProcessorsModule
{
    public List<IMetricProcessor> GetMetricProcessors()
    {
        return
        [
            FailRowMetricProcessor.Instance,
            TotalRowMetricProcessor.Instance,
            HighVolumeKeywordsMetricProcessor.Instance,
            MisspelledKeywordsMetricProcessor.Instance,
        ];
    }
    
}