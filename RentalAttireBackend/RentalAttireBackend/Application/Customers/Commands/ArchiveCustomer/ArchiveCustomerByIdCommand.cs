using MediatR;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Customers.Commands.ArchiveCustomer
{
    public class ArchiveCustomerByIdCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public string PerformedBy { get; set; } = string.Empty;
        public int PerformedById { get; set; }
    }
}
