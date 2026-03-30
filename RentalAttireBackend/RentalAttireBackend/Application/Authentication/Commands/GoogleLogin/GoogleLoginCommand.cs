using MediatR;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Authentication.Commands.GoogleLogin
{
    public class GoogleLoginCommand : IRequest<Result<AuthenticationResult>>
    {
        public string IdToken { get; set; } = string.Empty;
    }
}
