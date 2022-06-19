using System.Text.Json.Serialization;

namespace api.Entities;

public class Users
{
    public int Id { get; set; }

    public string? SteamId { get; set; }
    
    public string UserName { get; set; }

    public string Password { get; set; }

    public string? PhotoUrl { get; set; }

    [JsonIgnore]
    public ICollection<UserGames>? Collection { get; set; }
}