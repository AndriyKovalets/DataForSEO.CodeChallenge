namespace Dispatcher.Domain.Options;

public class WorkerOptions
{
    public static string SectionName => "WorkerOptions";

    public required int SleepTimeInSecond { get; set; }
    
    public int MaxDegreeOfParallelism { get; set; }
}