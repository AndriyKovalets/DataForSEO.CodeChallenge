using Dispatcher.Domain.Entities;

namespace Dispatcher.WorkerApplication.Abstractions.Services;

public interface IDispatcherWorkerService
{
    Task<List<int>> GetNotStartedSubTask(int count = -1);
    Task<List<SubTaskEntity>> GetSubTask(IEnumerable<int> subTaskIds, CancellationToken cancellationToken = default);
    Task SetInProgressSubTask(IEnumerable<SubTaskEntity> subTasks, CancellationToken cancellationToken = default);
    Task UpdateSubTask(SubTaskEntity subTask);
    Task ProcessSubTask(SubTaskEntity subTask, CancellationToken cancellationToken = default);

    Task RestartSubTask(int subTaskId);
}