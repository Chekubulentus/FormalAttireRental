using MediatR;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Suppliers.Commands.ArchiveSupplier
{
    public class ArchiveSupplierByIdCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
    }
}
