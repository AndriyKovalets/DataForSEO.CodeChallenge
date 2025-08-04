using Dispatcher.Domain.Enums;

namespace Dispatcher.Domain.Dtos;

public class SubTaskStatusDto
{
    public SubTaskStatusDto(SubTaskStatusEnum  status)
    {
        Status = status.ToString();
    }

    public string Status { get; set; }
}