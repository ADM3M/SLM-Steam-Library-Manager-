using System.ComponentModel.DataAnnotations;

namespace api.DTO;

public class UserBaseDataDTO
{
    [MinLength(3)]
    [MaxLength(15)]
    public string UserName { get; set; }

    [MinLength(4)]
    [MaxLength(32)]
    public string Password { get; set; }
}