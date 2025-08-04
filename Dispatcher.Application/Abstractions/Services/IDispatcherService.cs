using Dispatcher.Domain.Dtos;

namespace Dispatcher.Application.Abstractions.Services;

public interface IDispatcherService
{
    Task<TaskDto> CreateTask(CreateTaskDto createTask, CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetTaskStatus(int taskId, CancellationToken cancellationToken = default);
    Task<TaskDto> GetTask(int taskId, CancellationToken cancellationToken = default);
    Task<SubTaskDto> GetSubTask(int subTaskId, CancellationToken cancellationToken = default);
    Task<SubTaskStatusDto> GetSubTaskStatus(int subTaskId, CancellationToken cancellationToken = default);
    Task RestartSubTask(RestartSubTaskDto subTaskDto, CancellationToken cancellationToken = default);
}