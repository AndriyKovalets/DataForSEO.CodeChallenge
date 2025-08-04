namespace Dispatcher.Domain.Entities;

public class TaskEntity
{
    public int Id { get; set; }
    
    public string ListUrl { get; set; } = null!;
    
    public int CountOfFailSubTasks { get; set; }

    public List<SubTaskEntity> SubTasks { get; set; } = [];
}