namespace api.Entities;

public class Games
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string? ImageUrl { get; set; }

    public string? IconUrl { get; set; }

    public ICollection<UserGames>? collection { get; set; }
}