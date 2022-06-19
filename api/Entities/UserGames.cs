using api.Enums;

namespace api.Entities;

public class UserGames
{
    public UserGames() {}
    
    public UserGames(Users user, Games game, double playTime, GameStatus status)
    {
        User = user;
        Game = game;
        UserPlayTime = playTime;
        Status = status;
    }
    
    public double UserPlayTime { get; set; }

    public GameStatus Status { get; set; }
    
    public int UserId { get; set; }

    public int GameId { get; set; }

    public Users User { get; set; }

    public Games Game { get; set; }
}