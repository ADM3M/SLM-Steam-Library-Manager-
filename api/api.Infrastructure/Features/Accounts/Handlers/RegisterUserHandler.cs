using api.Application.Interfaces;
using api.Common.DTO;
using api.Infrastructure.Features.Accounts.Commands;
using MediatR;

namespace api.Infrastructure.Features.Accounts.Handlers;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AccountDTO>
{
    private readonly IAccountRepository _accountRepo;

    public RegisterUserHandler(IAccountRepository accountRepo)
    {
        _accountRepo = accountRepo;
    }

    public async Task<AccountDTO> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _accountRepo.CreateUserAsync(request.AuthData);
    }
}