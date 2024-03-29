using api.Common.Enums;

namespace api.Common.DTO;

public class UserGameDTO
{
    public int GameId { get; set; } = -1;
    public int AppId { get; set; }
    
    public double UserPlayTime { get; set; }

    public GameStatus Status { get; set; }

    public string Name { get; set; }

    public string? ImageUrl { get; set; }

    public string? IconUrl { get; set; }

    public DateTime dateTime { get; set; }
}