using MediatR;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Employees.Commands.ArchiveEmployee
{
    public class ArchiveEmployeeByIdCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public string PerformedBy { get; set; } = string.Empty;
        public int PerformedById { get; set; }
    }
}
