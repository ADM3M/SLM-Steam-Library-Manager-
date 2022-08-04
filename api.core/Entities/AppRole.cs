using Microsoft.AspNetCore.Identity;

namespace api.core.Entities;

public class AppRole : IdentityRole<int>
{
    public ICollection<AppUserRole> UserRoles { get; set; }
}