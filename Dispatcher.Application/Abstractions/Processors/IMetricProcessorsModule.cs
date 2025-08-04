using Dispatcher.Domain.Abstractions;

namespace Dispatcher.Application.Abstractions.Processors;

public interface IMetricProcessorsModule
{
    List<IMetricProcessor> GetMetricProcessors();
    void SetMetrics(IEnumerable<IMetricProcessor> metricProcessors, IMetrics entity);
}