using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Persons.DTO;

namespace RentalAttireBackend.Application.Customers.Commands.UpdateCustomerProfile
{
    public class UpdateCustomerProfileCommand : PersonDTO, IRequest<Result<bool>>
    {
        public string NewUserName { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
        public bool isPasswordMatched => string.Equals(NewPassword, ConfirmPassword);
        public IFormFile? NewProfileImage { get; set; }
    }
}
