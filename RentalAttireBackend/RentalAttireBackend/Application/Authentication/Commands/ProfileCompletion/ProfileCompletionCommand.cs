using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Application.Authentication.Commands.ProfileCompletion
{
    public class ProfileCompletionCommand : IRequest<Result<bool>>
    {
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Barangay { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
    }
}
