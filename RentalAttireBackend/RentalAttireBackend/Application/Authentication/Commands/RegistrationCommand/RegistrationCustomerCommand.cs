using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Customers.DTOs;

namespace RentalAttireBackend.Application.Authentication.Commands.RegistrationCommand
{
    public class RegistrationCustomerCommand : CustomerDTO, IRequest<Result<AuthenticationResult>>
    {
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
