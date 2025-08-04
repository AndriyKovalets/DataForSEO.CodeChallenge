namespace Dispatcher.Domain.Options;

public class QueueNameOptions
{
    public static string SectionName => "QueueNameOptions";

    public required string SubTasksQueueName { get; set; }
}