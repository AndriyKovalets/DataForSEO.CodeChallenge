using Dispatcher.Application.Abstractions.Services;
using Dispatcher.Domain.Enums;
using Dispatcher.Domain.Options;
using Microsoft.Extensions.Options;

namespace Dispatcher.Worker;

public class Worker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly WorkerOptions _options;
    private readonly ILogger<Worker> _logger;

    public Worker(
        IServiceScopeFactory scopeFactory,
        IOptions<WorkerOptions> options,
        ILogger<Worker> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        
        var dispatcherWorkerService = scope.ServiceProvider.GetRequiredService<IDispatcherWorkerService>();
        
        var countOfMessages = _options.MaxDegreeOfParallelism > 0  ? _options.MaxDegreeOfParallelism : -1;
        var maxDegreeOfParallelism = _options.MaxDegreeOfParallelism > 0  ? _options.MaxDegreeOfParallelism : Environment.ProcessorCount;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var subTaskIds = await dispatcherWorkerService.GetNotStartedSubTask(countOfMessages);

                if (!subTaskIds.Any())
                {
                    await Task.Delay(TimeSpan.FromSeconds(_options.SleepTimeInSecond), stoppingToken);
                    continue;
                }
            
                var subTasks = await dispatcherWorkerService.GetSubTask(subTaskIds, stoppingToken);
            
                await dispatcherWorkerService.SetInProgressSubTask(subTasks, stoppingToken);
            
                var parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = maxDegreeOfParallelism
                };
            
                await Parallel.ForEachAsync(subTasks, parallelOptions, async (subTask, cancellationToken) =>
                {
                    try
                    {
                        await dispatcherWorkerService.ProcessSubTask(subTask, cancellationToken);
                        subTask.Status = SubTaskStatusEnum.Completed;
                        await dispatcherWorkerService.UpdateSubTask(subTask, cancellationToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                        subTask.Status = SubTaskStatusEnum.Error;
                        await dispatcherWorkerService.UpdateSubTask(subTask, cancellationToken);
                    }
                });
            }
            catch (Exception e)
            {
               _logger.LogError(e.Message);
            }
            
            await Task.Delay(TimeSpan.FromSeconds(_options.SleepTimeInSecond), stoppingToken);
        }
    }
}