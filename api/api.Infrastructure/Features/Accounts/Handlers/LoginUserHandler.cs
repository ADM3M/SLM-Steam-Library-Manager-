using api.Common.DTO;
using api.Core.Interfaces;
using api.Infrastructure.Features.Accounts.Queries;
using MediatR;

namespace api.Infrastructure.Features.Accounts.Handlers;

public class LoginUserHandler : IRequestHandler<LoginUserQuery, AccountDTO>
{
    private readonly IAccountRepository _accountRepository;

    public LoginUserHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<AccountDTO> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _accountRepository.LoginUser(request.AuthData);
        return user;
    }
}