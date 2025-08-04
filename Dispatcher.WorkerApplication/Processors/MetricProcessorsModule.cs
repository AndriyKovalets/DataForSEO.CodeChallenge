using Dispatcher.WorkerApplication.Abstractions.Services.Processors;

namespace Dispatcher.WorkerApplication.Processors;

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