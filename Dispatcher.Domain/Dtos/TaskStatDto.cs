namespace Dispatcher.Domain.Dtos;

public class TaskStatDto
{
    public int CountOfRows { get; set; }
    public int HighVolumeKeywords { get; set; }
    public int MisspelledKeywords { get; set; }
    public int CountOfFailRows { get; set; }
    public int CountOfFailFiles { get; set; }
}