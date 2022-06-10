namespace api.Entities;

public class Users
{
    public int Id { get; set; }

    public long SteamId { get; set; }
    
    public string UserName { get; set; }

    public string Password { get; set; }

    public string PhotoUrl { get; set; }

    public ICollection<UserGames> Colection { get; set; }
}