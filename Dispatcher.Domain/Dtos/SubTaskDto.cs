using Dispatcher.Domain.Entities;
using Dispatcher.Domain.Enums;

namespace Dispatcher.Domain.Dtos;

public class SubTaskDto
{
    public SubTaskDto()
    {
        
    }

    public SubTaskDto(SubTaskEntity subTask)
    {
        Id = subTask.Id;
        Url = subTask.Url;
        Status = subTask.Status.ToString();
        CountOfRows = subTask.CountOfRows;
        HighVolumeKeywords =  subTask.HighVolumeKeywords;
        MisspelledKeywords =  subTask.MisspelledKeywords;
        CountOfFailRows =  subTask.CountOfFailRows;
    }
    
    public int Id { get; set; }
    public string Url { get; set; } = null!;
    public string Status { get; set; }
    public int CountOfRows { get; set; }
    public int HighVolumeKeywords { get; set; }
    public int MisspelledKeywords { get; set; }
    public int CountOfFailRows { get; set; }
}