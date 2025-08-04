using Dispatcher.Application.Abstractions.Persistence;
using Dispatcher.Application.Abstractions.QueueService;
using Dispatcher.Application.Abstractions.Services;
using Dispatcher.Application.Extensions;
using Dispatcher.Domain.Dtos;
using Dispatcher.Domain.Entities;
using Dispatcher.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dispatcher.Application.Services;

public class DispatcherService: IDispatcherService
{
    private readonly IApplicationDbContext _context;
    private readonly IQueueService _queueService;
    private readonly ILogger<DispatcherService> _logger;
    private readonly HttpClient _httpClient;

    public DispatcherService(
        IApplicationDbContext context, 
        IHttpClientFactory httpClientFactory,
        IQueueService queueService,
        ILogger<DispatcherService>  logger)
    {
        _context = context;
        _queueService = queueService;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<TaskDto> CreateTask(CreateTaskDto createTask, CancellationToken cancellationToken = default)
    {
        var taskToAdd = new TaskEntity()
        {
            ListUrl = createTask.ListUrl,
            CountOfFailSubTasks = 0
        };
        
        var httpResponse = await _httpClient.GetAsync(createTask.ListUrl,  cancellationToken);
        
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new ApplicationException($"Can't get list of urls. Url: {createTask.ListUrl}");
        }
        
        using var reader = new StreamReader(await httpResponse.Content.ReadAsStreamAsync(cancellationToken));

        while (!reader.EndOfStream)
        {
            taskToAdd.SubTasks.Add(new SubTaskEntity()
            {
                Url = await reader.ReadLineAsync(cancellationToken) ?? "",
                Status = SubTaskStatusEnum.NotStarted
            });
        }
        
        _context.Attach(taskToAdd);
        await _context.Save(cancellationToken);

        foreach (var subTask in taskToAdd.SubTasks)
        {
            await _queueService.AddSubTaskToQueue(subTask.Id);
        }
        
        return new TaskDto(taskToAdd);
    }
    
    public async Task<TaskStatusDto> TaskStatus(int taskId, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .Include(x => x.SubTasks)
            .FirstOrDefaultAsync(x => x.Id == taskId, cancellationToken);

        if (task == null)
        {
            
        }

        var result = new TaskStatusDto()
        {
            NotStarted = task!.SubTasks.Count(x => x.Status == SubTaskStatusEnum.NotStarted),
            InProgress = task.SubTasks.Count(x => x.Status == SubTaskStatusEnum.InProgress),
            Completed = task.SubTasks.Count(x => x.Status == SubTaskStatusEnum.Completed),
            Error = task.SubTasks.Count(x => x.Status == SubTaskStatusEnum.Error),
        };

        return result;
    }
}