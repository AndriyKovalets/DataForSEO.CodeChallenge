using System.Text.Json.Serialization;

namespace Dispatcher.Domain.Models;

public class Extra
{
    [JsonPropertyName("core_keyword")]
    public string? CoreKeyword { get; set; }
    
    [JsonPropertyName("synonym_clustering_algorithm")]
    public string? SynonymClusteringAlgorithm { get; set; }
    
    [JsonPropertyName("detected_language")]
    public string? DetectedLanguage { get; set; }
    
    [JsonPropertyName("keyword_difficulty")]
    public decimal? KeywordDifficulty { get; set; }
}