namespace Dispatcher.SharedApplication.Abstractions.Queue;

public interface IQueueService
{
    Task AddSubTaskToQueue(int subTaskId);
    Task<List<int>> GetSubTaskFromQueue(int count = -1);
}