using api.Common.DTO;
using MediatR;

namespace api.Infrastructure.Features.Accounts.Queries;

public class LoginUserQuery : IRequest<AccountDTO>
{
    public readonly UserAuthDataDTO AuthData;

    public LoginUserQuery(UserAuthDataDTO authData)
    {
        AuthData = authData;
    }
}