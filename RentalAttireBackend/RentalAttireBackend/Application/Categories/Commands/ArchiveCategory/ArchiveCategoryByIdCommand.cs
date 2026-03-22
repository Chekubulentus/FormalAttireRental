using MediatR;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Categories.Commands.ArchiveCategory
{
    public class ArchiveCategoryByIdCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public string PerformedBy { get; set; } = string.Empty;
        public int PerformedById { get; set; }
    }
}
