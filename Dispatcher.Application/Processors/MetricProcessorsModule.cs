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
    
    public void SetMetrics(IEnumerable<IMetricProcessor> metricProcessors, IMetrics entity)
    {
        foreach (var metricProcessor in metricProcessors)
        {
            SetMetrics(metricProcessor, entity);
        }
    }
    
    private void SetMetrics(IMetricProcessor metricProcessors, IMetrics entity)
    {
        switch (metricProcessors)
        {
            case TotalRowMetricProcessor:
                entity.CountOfRows = metricProcessors.MetricCount;
                break;
            case HighVolumeKeywordsMetricProcessor:
                entity.HighVolumeKeywords = metricProcessors.MetricCount;
                break;
            case MisspelledKeywordsMetricProcessor:
                entity.MisspelledKeywords = metricProcessors.MetricCount;
                break;
            case FailRowMetricProcessor:
                entity.CountOfFailRows = metricProcessors.MetricCount;
                break;
        }
    }
}