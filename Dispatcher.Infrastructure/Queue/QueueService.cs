using Dispatcher.Domain.Options;
using Dispatcher.SharedApplication.Abstractions.Queue;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Dispatcher.Infrastructure.Queue;

internal class QueueService: IQueueService
{
    private readonly QueueNameOptions _options;
    private readonly IDatabase _db;
    
    public QueueService(
        IConnectionMultiplexer redis,
        IOptions<QueueNameOptions>  options)
    {
        _options = options.Value;
        _db = redis.GetDatabase();
    }

    public async Task AddSubTaskToQueue(int subTaskId)
    {
        await _db.ListRightPushAsync(_options.SubTasksQueueName,  subTaskId);
    }

    public async Task<List<int>> GetSubTaskFromQueue(int count = -1)
    {
        var subTasks = new List<int>();
        var countOfMessages = 0;
        
        do
        {
            var value = await _db.ListRightPopAsync(_options.SubTasksQueueName);
            if (!value.HasValue)
            {
                break;
            }

            if (int.TryParse(value, out var subTaskId))
            {
                subTasks.Add(subTaskId);
            }
                
            countOfMessages++;
        } while (count > countOfMessages || count == -1 );
        
        return subTasks;
    }
}