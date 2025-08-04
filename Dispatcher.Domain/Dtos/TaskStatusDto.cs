namespace Dispatcher.Domain.Dtos;

public class TaskStatusDto
{
    public int NotStarted { get; set; }
    
    public int InProgress { get; set; }
    
    public int Completed { get; set; }
    
    public int Error { get; set; }
}