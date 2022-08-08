using Microsoft.AspNetCore.Identity;

namespace api.Entities;

public class AppUserRole : IdentityUserRole<int>
{
    public Users User { get; set; }

    public AppRole Role { get; set; }
}