using System.Text.Json.Serialization;

namespace Dispatcher.Domain.Models;

public class SearchIntentInfo
{
    [JsonPropertyName("main_intent")]
    public string? MainIntent { get; set; }
    
    [JsonPropertyName("foreign_intent")]
    public List<string>? ForeignIntent { get; set; }
    
    [JsonPropertyName("last_updated_time")]
    public DateTime LastUpdatedTime { get; set; }
}