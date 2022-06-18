using api.DTO;

namespace api.Helpers;

public class PagedList : List<UserGameDTO>
{
    public PagedList(IEnumerable<UserGameDTO> items, int count, int pageNumber, int pageSize)
    {
        CurrentPage = pageNumber;
        TotalPages = (int) Math.Ceiling((double) count / (double) pageSize);
        PageSize = pageSize;
        TotalCount = count;
        AddRange(items);
    }
    
    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }
    
    public static PagedList Create(IEnumerable<UserGameDTO> source, DisplayParams dp)
    {
        var count = source.Count();

        if (dp.PageNumber == -1)
        {
            return new PagedList(source, count, -1, count);
        }

        source = source
            .Where(u => dp.StatusesToShow.Contains(((int)u.Status).ToString()))
            .Skip((dp.PageNumber - 1) * dp.PageSize).Take(dp.PageSize);
            

        return new PagedList(source, count, dp.PageNumber, dp.PageSize);
    }
}