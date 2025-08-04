using Dispatcher.Application.Abstractions.Persistence;
using Dispatcher.Domain.Entities;
using Dispatcher.Domain.Enums;
using Dispatcher.Domain.Models;
using Dispatcher.SharedApplication.Abstractions.Queue;
using Dispatcher.SharedApplication.Extensions;
using Dispatcher.WorkerApplication.Abstractions.Services;
using Dispatcher.WorkerApplication.Abstractions.Services.Parsers;
using Dispatcher.WorkerApplication.Abstractions.Services.Processors;
using Dispatcher.WorkerApplication.Parsers;
using Microsoft.EntityFrameworkCore;

namespace Dispatcher.WorkerApplication.Services;

public class DispatcherWorkerService: IDispatcherWorkerService
{
    private readonly IApplicationDbContext _context;
    private readonly IMetricProcessorsModule _metricProcessorsModule;
    private readonly IQueueService _queueService;
    private readonly IParserFactory _parserFactory;
    private readonly HttpClient _httpClient;

    public DispatcherWorkerService(
        IApplicationDbContext  context,
        IHttpClientFactory clientFactory,
        IMetricProcessorsModule metricProcessorsModule,
        IQueueService  queueService,
        IParserFactory parserFactory)
    {
        _context = context;
        _metricProcessorsModule = metricProcessorsModule;
        _queueService = queueService;
        _parserFactory = parserFactory;
        _httpClient = clientFactory.CreateClient();
        _httpClient.Timeout = TimeSpan.FromHours(1);
    }

    public async Task<List<int>> GetNotStartedSubTask(int count =-1)
    {
        return await _queueService.GetSubTaskFromQueue(count);
    }
    
    public async Task RestartSubTask(int subTaskId)
    {
        await _queueService.AddSubTaskToQueue(subTaskId);
    }
    
    public async Task<List<SubTaskEntity>> GetSubTask(IEnumerable<int> subTaskIds, CancellationToken cancellationToken = default)
    {
        return await _context.SubTasks
            .Where(subTask => subTaskIds.Contains(subTask.Id))
            .ToListAsync(cancellationToken);
    }
    
    public async Task SetInProgressSubTask(IEnumerable<SubTaskEntity> subTasks, CancellationToken cancellationToken = default)
    {
        foreach (var subTask in subTasks)
        {
            subTask.Status = SubTaskStatusEnum.InProgress;
            _context.Attach(subTask);
        }

        await _context.Save(cancellationToken);
    }
    
    public async Task UpdateSubTask(SubTaskEntity subTask)
    {
        _context.Attach(subTask);

        await _context.Save();
    }
    
    public async Task ProcessSubTask(SubTaskEntity subTask, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(subTask.Url) || !subTask.Url.IsValidUrl())
        {
            subTask.Status = SubTaskStatusEnum.FailFile;
            return;
        }
        
        var httpResponse = await _httpClient.GetAsync(subTask.Url, cancellationToken);
        
        if (!httpResponse.IsSuccessStatusCode)
        {
            subTask.Status = SubTaskStatusEnum.FailFile;
            
            return;
        }
        
        var parserType = _parserFactory.GetTypeOfParser(subTask.Url);
        var parser = _parserFactory.CreateParser<KeywordModel>(parserType);

        var stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
        var data = parser.Parse(stream);

        var metricProcessorList = _metricProcessorsModule.GetMetricProcessors();
        
        await foreach (var item in data)
        {
            foreach (var metricProcessor in metricProcessorList)
            {
                metricProcessor.Process(item);
            }
        }

        foreach (var metricProcessor in metricProcessorList)
        {
            metricProcessor.SetMetric(subTask);
        }

        subTask.Status = SubTaskStatusEnum.Completed;
    }
}