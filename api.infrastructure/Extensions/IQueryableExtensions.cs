using api.core.Entities;
using api.infrastructure.Helpers;

namespace api.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<UserGames> OrderSwitch(this IQueryable<UserGames> query, DisplayParams dp)
    {
        return dp.OrderBy switch
        {
            "name" => query.OrderBy(ug => ug.Game.Name.ToLower()),
            "nameReverse" => query.OrderByDescending(ug => ug.Game.Name.ToLower()),
            "timePlayedReverse" => query.OrderByDescending(ug => ug.UserPlayTime),
            "status" => query.OrderBy(ug => ug.Status),
            "statusReverse" => query.OrderByDescending(ug => ug.Status),
            "timePlayed" => query.OrderBy(ug => ug.UserPlayTime),
            _ => query.OrderByDescending(ug => ug.UserPlayTime)
        };
    }
    
}