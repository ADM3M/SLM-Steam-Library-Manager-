using System.ComponentModel.DataAnnotations;

namespace api.common.DTO;

public class UserAuthDataDTO
{
    [MinLength(3)]
    [MaxLength(15)]
    public string UserName { get; set; }

    [MinLength(4)]
    [MaxLength(32)]
    public string Password { get; set; }
}