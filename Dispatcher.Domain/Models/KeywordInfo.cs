using System.Text.Json.Serialization;

namespace Dispatcher.Domain.Models;

public class KeywordInfo
{
    [JsonPropertyName("search_volume")]
    public int? SearchVolume { get; set; }
    
    [JsonPropertyName("cpc")]
    public decimal? Cpc { get; set; }
    
    [JsonPropertyName("competition")]
    public decimal? Competition { get; set; }
    
    [JsonPropertyName("competition_level")]
    public string? CompetitionLevel { get; set; }
    
    [JsonPropertyName("low_top_of_page_bid")]
    public decimal? LowTopOfPageBid { get; set; }
    
    [JsonPropertyName("high_top_of_page_bid")]
    public decimal? HighTopOfPageBid { get; set; }
    
    [JsonPropertyName("time_update")]
    public DateTime TimeUpdate { get; set; }
    
    [JsonPropertyName("categories")]
    public List<int>? Categories { get; set; }
    
    [JsonPropertyName("history")]
    public Dictionary<string, int?>? History { get; set; }
}