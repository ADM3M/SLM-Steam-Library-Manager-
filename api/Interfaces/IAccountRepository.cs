using api.DTO;

namespace api.Interfaces;

public interface IAccountRepository
{
    Task<UserDTO> CreateUserAsync(UserAuthDataDTO userAuthDataDto);

    Task<UserDTO> LoginUser(UserAuthDataDTO userAuthDataDto);

    Task<int> DeleteUserAsync(int userId);
}