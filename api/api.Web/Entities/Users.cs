using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace api.Entities;

public class Users : IdentityUser<int>
{

    public string? SteamId { get; set; }

    public string? PhotoUrl { get; set; }

    [JsonIgnore]
    public ICollection<UserGames>? Collection { get; set; }

    public ICollection<AppUserRole> UserRoles { get; set; }
    
    public ICollection<Messages> MessagesSent { get; set; }

    public ICollection<Messages> MessagesReceived { get; set; }
}