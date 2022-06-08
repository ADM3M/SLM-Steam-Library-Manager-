namespace api.Entities;

public class User
{
    public int Id { get; set; }

    public string PhotoUrl { get; set; }

    public IEnumerable<GameInfo> UserGames { get; set; }
}