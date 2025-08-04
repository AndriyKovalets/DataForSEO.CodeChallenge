using System.Text.Json.Serialization;

namespace Dispatcher.Domain.Models;

public class KeywordModel
{
    [JsonPropertyName("keyword")]
    public string? Keyword { get; set; }
    
    [JsonPropertyName("location")]
    public int Location { get; set; }
    
    [JsonPropertyName("language")]
    public string? Language { get; set; }
    
    [JsonPropertyName("spell")]
    public string? Spell { get; set; }
    
    [JsonPropertyName("spell_type")]
    public string? SpellType { get; set; }
    
    [JsonPropertyName("keyword_info")]
    public KeywordInfo? KeywordInfo { get; set; }
    
    [JsonPropertyName("extra")]
    public Extra? Extra { get; set; }
    
    [JsonPropertyName("search_intent_info")]
    public SearchIntentInfo? SearchIntentInfo { get; set; }
}
