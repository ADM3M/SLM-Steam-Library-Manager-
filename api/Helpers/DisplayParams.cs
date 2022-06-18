namespace api.Helpers;

public class DisplayParams : PaginationParams
{

    
    public string? Search { get; set; } = "";
    public string StatusesToShow { get; set; } = "0123";
    public string OrderBy { get; set; } = "timePlayedReverse";
}