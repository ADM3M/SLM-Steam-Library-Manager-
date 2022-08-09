using api.Common.DTO;
using MediatR;

namespace api.Infrastructure.Features.Accounts.Commands;

public class RegisterUserCommand : IRequest<AccountDTO>
{
    public readonly UserAuthDataDTO AuthData;

    public RegisterUserCommand(UserAuthDataDTO authData)
    {
        AuthData = authData;
    }
}