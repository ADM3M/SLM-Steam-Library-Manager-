using Microsoft.AspNetCore.Identity;

namespace api.Core.Entities;

public class AppRole : IdentityRole<int>
{
    public ICollection<AppUserRole> UserRoles { get; set; }
}