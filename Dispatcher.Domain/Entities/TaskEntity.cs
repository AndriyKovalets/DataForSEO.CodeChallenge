namespace Dispatcher.Domain.Entities;

public class TaskEntity
{
    public int Id { get; set; }
    
    public string ListUrl { get; set; } = null!;

    public List<SubTaskEntity> SubTasks { get; set; } = [];
}