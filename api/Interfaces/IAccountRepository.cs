using api.DTO;

namespace api.Interfaces;

public interface IAccountRepository
{
    Task<UserDTO> CreateUserAsync(UserBaseDataDTO userBaseDataDto);

    UserDTO LoginUser(UserBaseDataDTO userBaseDataDto);

    Task<int> DeleteUserAsync(int userId);
}