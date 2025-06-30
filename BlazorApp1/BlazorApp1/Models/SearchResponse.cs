using System.Collections.Generic;

namespace BlazorApp1.Models;

public class SearchResponse
{
    public List<QueueInfo> Records { get; set; } = new();
    public int TotalCount { get; set; }
}
