using Dispatcher.Domain.Abstractions;
using Dispatcher.Domain.Enums;

namespace Dispatcher.Domain.Entities;

public class SubTaskEntity: IMetrics
{
    public int Id { get; set; }
    public string Url { get; set; } = null!;
    public SubTaskStatusEnum Status { get; set; }
    public int CountOfRows { get; set; }
    public int HighVolumeKeywords { get; set; }
    public int MisspelledKeywords { get; set; }
    public int CountOfFailRows { get; set; }
    public int TaskId { get; set; }
    public TaskEntity Task { get; set; } = null!;
}