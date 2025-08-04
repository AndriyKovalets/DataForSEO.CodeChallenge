namespace Dispatcher.Application.Abstractions.QueueService;

public interface IQueueService
{
    Task AddSubTaskToQueue(int subTaskId);
    Task<List<int>> GetSubTaskFromQueue(int count = -1);
}