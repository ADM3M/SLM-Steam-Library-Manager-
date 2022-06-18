namespace api.Helpers;

public class DisplayParams : PaginationParams
{
    public string? Search { get; set; } = "";
    public string StatusesToShow { get; set; } = "";
    public string OrderBy { get; set; } = "timePlayedReverse";
}