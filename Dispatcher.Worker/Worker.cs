using Dispatcher.Domain.Entities;
using Dispatcher.Domain.Enums;
using Dispatcher.Domain.Options;
using Dispatcher.WorkerApplication.Abstractions.Services;
using Microsoft.Extensions.Options;

namespace Dispatcher.Worker;

public class Worker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly WorkerOptions _options;
    private readonly ILogger<Worker> _logger;
    private AsyncServiceScope _scope;
    private IDispatcherWorkerService _dispatcherWorkerService;
    
    private List<SubTaskEntity> _subTasks;
    public Worker(
        IServiceScopeFactory scopeFactory,
        IOptions<WorkerOptions> options,
        ILogger<Worker> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
        _subTasks = [];
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _scope = _scopeFactory.CreateAsyncScope();
        
        _dispatcherWorkerService = _scope.ServiceProvider.GetRequiredService<IDispatcherWorkerService>();
        
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        var countOfMessages = _options.MaxDegreeOfParallelism > 0  ? _options.MaxDegreeOfParallelism : -1;
        var maxDegreeOfParallelism = _options.MaxDegreeOfParallelism > 0  ? _options.MaxDegreeOfParallelism : Environment.ProcessorCount;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var subTaskIds = await _dispatcherWorkerService.GetNotStartedSubTask(countOfMessages);

                if (!subTaskIds.Any())
                {
                    await Task.Delay(TimeSpan.FromSeconds(_options.SleepTimeInSecond), stoppingToken);
                    continue;
                }
            
                _subTasks = await _dispatcherWorkerService.GetSubTask(subTaskIds, stoppingToken);
            
                await _dispatcherWorkerService.SetInProgressSubTask(_subTasks, stoppingToken);
            
                var parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = maxDegreeOfParallelism
                };
            
                await Parallel.ForEachAsync(_subTasks, parallelOptions, async (subTask, cancellationToken) =>
                {
                    try
                    {
                        _logger.LogInformation("Processing sub task {SubTaskId}", subTask.Id);
                        await _dispatcherWorkerService.ProcessSubTask(subTask, cancellationToken);
                        subTask.Status = SubTaskStatusEnum.Completed;
                        await _dispatcherWorkerService.UpdateSubTask(subTask);
                        _logger.LogInformation("End processing sub task {SubTaskId}", subTask.Id);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                        subTask.Status = SubTaskStatusEnum.Error;
                        await _dispatcherWorkerService.UpdateSubTask(subTask);
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

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        var inprogressSubTask = _subTasks
            .Where(x => x.Status == SubTaskStatusEnum.InProgress)
            .ToList();
        
        foreach (var subTask in inprogressSubTask)
        {
            subTask.Status = SubTaskStatusEnum.NotStarted;
            await _dispatcherWorkerService.RestartSubTask(subTask.Id);
            await _dispatcherWorkerService.UpdateSubTask(subTask);
        }

        await _scope.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}