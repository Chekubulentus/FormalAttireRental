using MediatR;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Authentication.Commands.Login
{
    public class LoginCommand : IRequest<Result<AuthenticationResult>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
