using Dispatcher.Domain.Entities;

namespace Dispatcher.Domain.Dtos;

public class TaskDto
{
    public TaskDto()
    {
        
    }

    public TaskDto(TaskEntity task)
    {
        Id = task.Id;
        ListUrl = task.ListUrl;
        CountOfFailSubTasks = task.CountOfFailSubTasks;

        foreach (var subTask in task.SubTasks)
        {
            SubTasks.Add(new SubTaskDto(subTask));
        }
    }
    
    public int Id { get; set; }
    
    public string ListUrl { get; set; } = null!;
    
    public int CountOfFailSubTasks { get; set; }

    public List<SubTaskDto> SubTasks { get; set; } = [];
}