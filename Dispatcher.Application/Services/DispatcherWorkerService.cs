using Dispatcher.Application.Abstractions.Persistence;
using Dispatcher.Application.Abstractions.Processors;
using Dispatcher.Application.Abstractions.QueueService;
using Dispatcher.Application.Abstractions.Services;
using Dispatcher.Application.Extensions;
using Dispatcher.Application.Parsers;
using Dispatcher.Domain.Entities;
using Dispatcher.Domain.Enums;
using Dispatcher.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Dispatcher.Application.Services;

public class DispatcherWorkerService: IDispatcherWorkerService
{
    private readonly IApplicationDbContext _context;
    private readonly IMetricProcessorsModule _metricProcessorsModule;
    private readonly IQueueService _queueService;
    private readonly HttpClient _httpClient;

    public DispatcherWorkerService(
        IApplicationDbContext  context,
        IHttpClientFactory clientFactory,
        IMetricProcessorsModule metricProcessorsModule,
        IQueueService  queueService)
    {
        _context = context;
        _metricProcessorsModule = metricProcessorsModule;
        _queueService = queueService;
        _httpClient = clientFactory.CreateClient();
        _httpClient.Timeout = TimeSpan.FromHours(1);
    }

    public async Task<List<int>> GetNotStartedSubTask(int count =-1)
    {
        return await _queueService.GetSubTaskFromQueue(count);
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
    
    public async Task UpdateSubTask(SubTaskEntity subTask, CancellationToken cancellationToken = default)
    {
        _context.Attach(subTask);

        await _context.Save(cancellationToken);
    }
    
    public async Task ProcessSubTask(SubTaskEntity subTask, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(subTask.Url) || !subTask.Url.IsValidUrl())
        {
            subTask.Status = SubTaskStatusEnum.Error;
            return;
        }
        
        var httpResponse = await _httpClient.GetAsync(subTask.Url, cancellationToken);
        
        if (!httpResponse.IsSuccessStatusCode)
        {
            subTask.Status = SubTaskStatusEnum.Error;
            
            return;
        }
        
        var parserType = ParserFactory.GetTypeOfParser(subTask.Url);
        var parser = ParserFactory.CreateParser<KeywordModel>(parserType);

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
        
        _metricProcessorsModule.SetMetrics(metricProcessorList, subTask);
        subTask.Status = SubTaskStatusEnum.Completed;
    }
}