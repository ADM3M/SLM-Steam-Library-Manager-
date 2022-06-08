using api.Enums;

namespace api.Entities;

public class GameInfo
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsFavourite { get; set; }
    
    public int SteamId { get; set; }

    public string IconUrl { get; set; }

    public string ImgUrl { get; set; }

    public double TotalPlayTime { get; set; } = 0d;

    public GameStatus Status { get; set; } = GameStatus.NotSet;
}