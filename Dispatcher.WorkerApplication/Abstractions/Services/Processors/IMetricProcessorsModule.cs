namespace Dispatcher.WorkerApplication.Abstractions.Services.Processors;

public interface IMetricProcessorsModule
{
    List<IMetricProcessor> GetMetricProcessors();
}