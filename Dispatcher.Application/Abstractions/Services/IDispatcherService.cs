using Dispatcher.Domain.Dtos;

namespace Dispatcher.Application.Abstractions.Services;

public interface IDispatcherService
{
    Task<TaskDto> CreateTask(CreateTaskDto createTask, CancellationToken cancellationToken = default);
    Task<TaskStatusDto> TaskStatus(int taskId, CancellationToken cancellationToken = default);
}