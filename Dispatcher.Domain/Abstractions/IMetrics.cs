namespace Dispatcher.Domain.Abstractions;

public interface IMetrics
{
    public int CountOfRows { get; set; }
    public int HighVolumeKeywords { get; set; }
    public int MisspelledKeywords { get; set; }
    public int CountOfFailRows { get; set; }
}