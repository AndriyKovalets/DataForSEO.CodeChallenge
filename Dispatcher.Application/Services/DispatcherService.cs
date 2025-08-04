using System.Text.RegularExpressions;
using Dispatcher.Application.Abstractions.Persistence;
using Dispatcher.Application.Abstractions.QueueService;
using Dispatcher.Application.Abstractions.Services;
using Dispatcher.Domain.Dtos;
using Dispatcher.Domain.Entities;
using Dispatcher.Domain.Enums;
using Dispatcher.Domain.Exceptions;
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
            throw new BadRequestHttpException($"Can't get list of urls. Url: {createTask.ListUrl}");
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
    
    public async Task<TaskDto> GetTask(int taskId, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .Include(x => x.SubTasks)
            .FirstOrDefaultAsync(x => x.Id == taskId, cancellationToken);

        if (task == null)
        {
            throw new NotFoundHttpException($"Can't find task with id: {taskId}");
        }
        
        return new TaskDto(task);
    }
    
    public async Task<SubTaskDto> GetSubTask(int subTaskId, CancellationToken cancellationToken = default)
    {
        var subTask = await _context.SubTasks
            .FirstOrDefaultAsync(x => x.Id == subTaskId, cancellationToken);

        if (subTask == null)
        {
            throw new NotFoundHttpException($"Can't find subtask with id: {subTaskId}");
        }
        
        return new SubTaskDto(subTask);
    }
    
    public async Task RestartSubTask(RestartSubTaskDto subTaskDto, CancellationToken cancellationToken = default)
    {
        var subTask = await _context.SubTasks
            .FirstOrDefaultAsync(x => x.Id == subTaskDto.Id, cancellationToken);

        if (subTask == null)
        {
            throw new NotFoundHttpException($"Can't find subtask with id: {subTaskDto.Id}");
        }
        
        await _queueService.AddSubTaskToQueue(subTaskDto.Id);
    }
    
    public async Task<Dictionary<string, int>> GetTaskStatus(int taskId, CancellationToken cancellationToken = default)
    {
        var taskExist =  await _context.Tasks.AnyAsync(x => x.Id == taskId, cancellationToken);
        
        if (!taskExist)
        {
            throw new NotFoundHttpException($"Can't find task with id: {taskId}");
        }
        
        var statuses = await _context.SubTasks
            .Where(x => x.TaskId == taskId)
            .GroupBy(x => x.Status)
            .ToDictionaryAsync(x =>x.Key.ToString(), y => y.Count(), cancellationToken);

        return statuses;
    }
    
    public async Task<SubTaskStatusDto> GetSubTaskStatus(int subTaskId, CancellationToken cancellationToken = default)
    {
        var subTask = await _context.SubTasks
            .FirstOrDefaultAsync(x => x.Id == subTaskId, cancellationToken);

        if (subTask == null)
        {
            throw new NotFoundHttpException($"Can't find subtask with id: {subTaskId}");
        }

        return new SubTaskStatusDto(subTask.Status);
    }
    
    public async Task<TaskStatDto> GetTaskStat(int taskId, CancellationToken cancellationToken = default)
    {
        var taskExist =  await _context.Tasks.AnyAsync(x => x.Id == taskId, cancellationToken);
        
        if (!taskExist)
        {
            throw new NotFoundHttpException($"Can't find task with id: {taskId}");
        }

        var stat = await _context.SubTasks
            .Where(x => x.TaskId == taskId)
            .GroupBy(_ => 1)
            .Select(x => new TaskStatDto
            {
                CountOfFailRows = x.Sum(y => y.CountOfFailRows),
                CountOfRows = x.Sum(y => y.CountOfRows),
                HighVolumeKeywords = x.Sum(y => y.HighVolumeKeywords),
                MisspelledKeywords = x.Sum(y => y.MisspelledKeywords),
                CountOfFailFiles = x.Count(y => y.Status == SubTaskStatusEnum.FailFile)
            }).FirstOrDefaultAsync(cancellationToken);
        
        return stat ?? new TaskStatDto();
    }
}