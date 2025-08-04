using System.ComponentModel;

namespace Dispatcher.Domain.Dtos;

public class CreateTaskDto
{
    //add default for testing
    [DefaultValue("https://downloads.dataforseo.com/dfs_test/1caaa605-6f79-40ed-a6d1-4fe4754c6274/list.txt")]
    public string ListUrl { get; set; } = null!;
}