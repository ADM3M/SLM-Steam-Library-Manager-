using api.Common.DTO;

namespace api.Application.Interfaces;

public interface IAccountRepository
{
    Task<AccountDTO> CreateUserAsync(UserAuthDataDTO userAuthDataDto);

    Task<AccountDTO> LoginUser(UserAuthDataDTO userAuthDataDto);

    Task<bool> DeleteUserAsync(int userId);
}