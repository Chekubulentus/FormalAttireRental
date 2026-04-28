using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Customers.DTOs;

namespace RentalAttireBackend.Application.Customers.Commands.CustomerRegistration
{
    public class CustomerRegistrationCommand : CustomerDTO, IRequest<Result<AuthenticationResult>>
    {
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public int PerformedById { get; set; }
    }
}
