using MediatR;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Authentication.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<Result<AuthenticationResult>>
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
